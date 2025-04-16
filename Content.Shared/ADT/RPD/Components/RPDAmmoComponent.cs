using Content.Shared.ADT.RPD.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.ADT.RPD.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(RPDAmmoSystem))]
public sealed partial class RPDAmmoComponent : Component
{
    [DataField("rpdcharges"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public int RPDCharges { get; set; } = 130;

    [DataField("rcdcharges"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public int RCDCharges { get; set; } = 30;
}