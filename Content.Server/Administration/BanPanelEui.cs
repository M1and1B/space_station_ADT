using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Content.Server.Administration.Managers;
using Content.Server.Administration.Systems;
using Content.Server.ADT.Discord;
using Content.Server.ADT.Discord.Bans;
using Content.Server.ADT.Discord.Bans.PayloadGenerators;
using Content.Server.Chat.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Server.Player;
using Robust.Shared.Network;

namespace Content.Server.Administration;

public sealed class BanPanelEui : BaseEui
{
    [Dependency] private readonly IBanManager _banManager = default!;
    [Dependency] private readonly IEntityManager _entities = default!;
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;
    [Dependency] private readonly IChatManager _chat = default!;
    [Dependency] private readonly IAdminManager _admins = default!;
    [Dependency] private readonly IServerDbManager _dbManager = default!;
    [Dependency] private readonly IDiscordBanInfoSender _discordBanInfoSender = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private readonly ISawmill _sawmill;

    private NetUserId? PlayerId { get; set; }
    private string PlayerName { get; set; } = string.Empty;
    private IPAddress? LastAddress { get; set; }
    private ImmutableTypedHwid? LastHwid { get; set; }
    private const int Ipv4_CIDR = 32;
    private const int Ipv6_CIDR = 64;

    public BanPanelEui()
    {
        IoCManager.InjectDependencies(this);

        _sawmill = _log.GetSawmill("admin.bans_eui");
    }

    public override EuiStateBase GetNewState()
    {
        var hasBan = _admins.HasAdminFlag(Player, AdminFlags.Ban);
        return new BanPanelEuiState(PlayerName, hasBan);
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        switch (msg)
        {
            case BanPanelEuiStateMsg.CreateBanRequest r:
                BanPlayer(r.Player, r.IpAddress, r.UseLastIp, r.Hwid, r.UseLastHwid, r.Minutes, r.Severity, r.Reason, r.Roles, r.Erase);
                break;
            case BanPanelEuiStateMsg.GetPlayerInfoRequest r:
                ChangePlayer(r.PlayerUsername);
                break;
        }
    }

    private async void BanPlayer(string? target, string? ipAddressString, bool useLastIp, ImmutableTypedHwid? hwid, bool useLastHwid, uint minutes, NoteSeverity severity, string reason, IReadOnlyCollection<string>? roles, bool erase)
    {
        if (!_admins.HasAdminFlag(Player, AdminFlags.Ban))
        {
            _sawmill.Warning($"{Player.Name} ({Player.UserId}) tried to create a ban with no ban flag");
            return;
        }
        if (target == null && string.IsNullOrWhiteSpace(ipAddressString) && hwid == null)
        {
            _chat.DispatchServerMessage(Player, Loc.GetString("ban-panel-no-data"));
            return;
        }

        (IPAddress, int)? addressRange = null;
        if (ipAddressString is not null)
        {
            var hid = "0";
            var split = ipAddressString.Split('/', 2);
            ipAddressString = split[0];
            if (split.Length > 1)
                hid = split[1];

            if (!IPAddress.TryParse(ipAddressString, out var ipAddress) || !uint.TryParse(hid, out var hidInt) || hidInt > Ipv6_CIDR || hidInt > Ipv4_CIDR && ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                _chat.DispatchServerMessage(Player, Loc.GetString("ban-panel-invalid-ip"));
                return;
            }

            if (hidInt == 0)
                hidInt = (uint)(ipAddress.AddressFamily == AddressFamily.InterNetworkV6 ? Ipv6_CIDR : Ipv4_CIDR);

            addressRange = (ipAddress, (int)hidInt);
        }

        var targetUid = target is not null ? PlayerId : null;
        addressRange = useLastIp && LastAddress is not null ? (LastAddress, LastAddress.AddressFamily == AddressFamily.InterNetworkV6 ? Ipv6_CIDR : Ipv4_CIDR) : addressRange;
        var targetHWid = useLastHwid ? LastHwid : hwid;
        if (target != null && target != PlayerName || Guid.TryParse(target, out var parsed) && parsed != PlayerId)
        {
            var located = await _playerLocator.LookupIdByNameOrIdAsync(target);
            if (located == null)
            {
                _chat.DispatchServerMessage(Player, Loc.GetString("cmd-ban-player"));
                return;
            }
            targetUid = located.UserId;
            var targetAddress = located.LastAddress;
            if (useLastIp && targetAddress != null)
            {
                if (targetAddress.IsIPv4MappedToIPv6)
                    targetAddress = targetAddress.MapToIPv4();

                // Ban /64 for IPv6, /32 for IPv4.
                var hid = targetAddress.AddressFamily == AddressFamily.InterNetworkV6 ? Ipv6_CIDR : Ipv4_CIDR;
                addressRange = (targetAddress, hid);
            }
            targetHWid = useLastHwid ? located.LastHWId : hwid;
        }

        if (roles?.Count > 0)
        {
            var now = DateTimeOffset.UtcNow;
            //Start-ADT-Tweak: логи банов для диса
            var lastRoleBan = await _dbManager.GetLastServerRoleBanAsync();
            var startRoleBanId = lastRoleBan is not null ? lastRoleBan.Id + 1 : 1;
            var currentRoleBanId = startRoleBanId;
            var rolesData = new List<string>();
            //End-ADT-Tweak
            foreach (var role in roles)
            {
                rolesData.Add(string.Format("{0}:{1}", role, currentRoleBanId++)); //ADT-Tweak
                await _banManager.CreateRoleBan(targetUid, target, Player.UserId, addressRange, targetHWid, role, minutes, severity, reason, now);
            }
            //Start-ADT-Tweak: логи банов для диса
            var roleBanInfo = new BanInfo
            {
                BanId = string.Empty,
                Target = target!,
                Player = Player,
                Minutes = minutes,
                Reason = reason,
                Expires = DateTimeOffset.Now + TimeSpan.FromMinutes(minutes),
                AdditionalInfo = new() { { "roles", string.Join(", ", rolesData) } }
            };

            await _discordBanInfoSender.SendBanInfoAsync<PanelBanPayloadGenerator>(roleBanInfo);
            //End-ADT-Tweak

            Close();
            return;
        }

        if (erase &&
            targetUid != null)
        {
            try
            {
                if (_entities.TrySystem(out AdminSystem? adminSystem))
                    adminSystem.Erase(targetUid.Value);
            }
            catch (Exception e)
            {
                _sawmill.Error($"Error while erasing banned player:\n{e}");
            }
        }
        // ADT-Tweak-Start
        if (targetUid != null)
        {
            var dbData = await _dbManager.GetAdminDataForAsync(targetUid.Value);

            if (dbData != null && dbData.AdminRank != null)
            {
                var targetPermissionsFlag = AdminFlagsHelper.NamesToFlags(dbData.AdminRank.Flags.Select(p => p.Flag));

                if ((targetPermissionsFlag & AdminFlags.Permissions) == AdminFlags.Permissions)
                    return;
            }
        }
        // ADT-Tweak-End
        var lastServerBan = await _dbManager.GetLastServerBanAsync();
        var newServerBanId = lastServerBan is not null ? lastServerBan.Id + 1 : 1;

        _banManager.CreateServerBan(targetUid, target, Player.UserId, addressRange, targetHWid, minutes, severity, reason);

        var banInfo = new BanInfo
        {
            BanId = newServerBanId.ToString()!,
            Target = target!,
            Player = Player,
            Minutes = minutes,
            Reason = reason,
            Expires = DateTimeOffset.Now + TimeSpan.FromMinutes(minutes)
        };

        await _discordBanInfoSender.SendBanInfoAsync<PanelBanPayloadGenerator>(banInfo);

        Close();
    }

    public async void ChangePlayer(string playerNameOrId)
    {
        var located = await _playerLocator.LookupIdByNameOrIdAsync(playerNameOrId);
        ChangePlayer(located?.UserId, located?.Username ?? string.Empty, located?.LastAddress, located?.LastHWId);
    }

    public void ChangePlayer(NetUserId? playerId, string playerName, IPAddress? lastAddress, ImmutableTypedHwid? lastHwid)
    {
        PlayerId = playerId;
        PlayerName = playerName;
        LastAddress = lastAddress;
        LastHwid = lastHwid;
        StateDirty();
    }

    public override async void Opened()
    {
        base.Opened();
        _admins.OnPermsChanged += OnPermsChanged;
    }

    public override void Closed()
    {
        base.Closed();
        _admins.OnPermsChanged -= OnPermsChanged;
    }

    private void OnPermsChanged(AdminPermsChangedEventArgs args)
    {
        if (args.Player != Player)
        {
            return;
        }

        StateDirty();
    }
}
