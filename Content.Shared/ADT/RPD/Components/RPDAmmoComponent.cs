using Content.Shared.ADT.RPD.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.ADT.RPD.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(RPDAmmoSystem))]
public sealed partial class RPDAmmoComponent : Component
{
    [DataField("charges"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public int Charges { get; set; } = 90;
}