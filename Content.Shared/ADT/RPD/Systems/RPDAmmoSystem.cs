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
            ("rpdCharges", comp.RPDCharges),
            ("rcdCharges", comp.RCDCharges));
        args.PushText(examineMessage);
    }

    private void OnAfterInteract(EntityUid uid, RPDAmmoComponent comp, AfterInteractEvent args)
    {
        if (args.Handled || !args.CanReach || !_timing.IsFirstTimePredicted)
            return;

        if (args.Target is not { Valid: true } target ||
            !TryComp<LimitedChargesComponent>(target, out var charges))
            return;

        if (HasComp<RCDComponent>(target))
        {
            ApplyRefill(uid, target, args.User, charges, comp, isRcd: true);
            args.Handled = true;
        }
        else if (HasComp<RPDComponent>(target))
        {
            ApplyRefill(uid, target, args.User, charges, comp, isRcd: false);
            args.Handled = true;
        }
    }

    private void ApplyRefill(EntityUid uid, EntityUid target, EntityUid user, LimitedChargesComponent charges,
        RPDAmmoComponent comp, bool isRcd)
    {
        int currentCharges = isRcd ? comp.RCDCharges : comp.RPDCharges;

        var count = Math.Min(charges.MaxCharges - charges.Charges, currentCharges);
        if (count <= 0)
        {
            _popup.PopupClient(Loc.GetString("rpd-ammo-component-after-interact-full"), target, user);
            return;
        }

        _popup.PopupClient(Loc.GetString("rpd-ammo-component-after-interact-refilled"), target, user);
        _charges.AddCharges(target, count, charges);

        if (isRcd)
            comp.RCDCharges -= count;
        else
            comp.RPDCharges -= count;

        Dirty(uid, comp);

        if (comp.RPDCharges <= 0 && comp.RCDCharges <= 0)
            QueueDel(uid);
    }
}