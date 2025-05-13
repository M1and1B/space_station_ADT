namespace Content.Client.VendingMachines;

/// <summary>
/// Used for specifying the pda windows border colors
/// </summary>
[RegisterComponent]
public sealed partial class VendingMachineBorderColorComponent : Component
{
    [DataField("borderColor", required: true)]
    public string? BorderColor;


    [DataField("accentHColor")]
    public string? AccentHColor;


    [DataField("accentVColor")]
    public string? AccentVColor;
}
