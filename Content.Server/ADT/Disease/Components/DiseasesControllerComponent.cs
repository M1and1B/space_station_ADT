using Content.Server.ADT.Disease.Data;

namespace Content.Server.ADT.Disease.Components;

[RegisterComponent]
public sealed partial class DiseasesControllerComponent : Component
{
    [DataField]
    public Dictionary<string, DiseaseCureData> DiseasesCures = [];
}
