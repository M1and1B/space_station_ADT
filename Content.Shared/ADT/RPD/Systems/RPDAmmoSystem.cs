using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.RCD.Components;
using Content.Shared.ADT.RPD.Components;
using Robust.Shared.Timing;

namespace Content.Shared.ADT.RPD.Systems;

public sealed class RPDAmmoSystem : EntitySystem
{
    [Dependency] private readonly SharedChargesSystem _charges = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RPDAmmoComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<RPDAmmoComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnExamine(EntityUid uid, RPDAmmoComponent comp, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        var examineMessage = Loc.GetString("rpd-ammo-component-on-examine",
            ("$charges", comp.Charges));
        args.PushText(examineMessage);
    }

    private void OnAfterInteract(EntityUid uid, RPDAmmoComponent comp, AfterInteractEvent args)
    {
        if (args.Handled || !args.CanReach || !_timing.IsFirstTimePredicted)
            return;

        if (args.Target is not { Valid: true } target ||
            !TryComp<LimitedChargesComponent>(target, out var charges))
            return;

        if (!HasComp<RCDComponent>(target) && !HasComp<RPDComponent>(target))
            return;

        ApplyRefill(uid, target, args.User, charges, comp);
        args.Handled = true;
    }

    private void ApplyRefill(EntityUid uid, EntityUid target, EntityUid user, LimitedChargesComponent charges,
        RPDAmmoComponent comp)
    {
        var refillAmount = Math.Min(charges.MaxCharges - charges.Charges, comp.Charges);
        if (refillAmount <= 0)
        {
            _popup.PopupClient(Loc.GetString("rpd-ammo-component-after-interact-full"), target, user);
            return;
        }

        _charges.AddCharges(target, refillAmount, charges);
        comp.Charges -= refillAmount;

        _popup.PopupClient(Loc.GetString("rpd-ammo-component-after-interact-refilled"), target, user);
        Dirty(uid, comp);

        if (comp.Charges <= 0)
            QueueDel(uid);
    }
}