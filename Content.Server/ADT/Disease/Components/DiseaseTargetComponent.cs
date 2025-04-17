using Content.Server.ADT.Disease.Data;

namespace Content.Server.ADT.Disease.Components;

[RegisterComponent]
public sealed partial class DiseaseTargetComponent : Component
{
    [DataField]
    public Dictionary<string, DiseaseData> Diseases { get; set; } = new();
}
