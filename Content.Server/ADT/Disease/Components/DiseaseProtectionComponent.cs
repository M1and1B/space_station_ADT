using Content.Server.ADT.Disease.Data;

namespace Content.Server.ADT.Disease.Components;

[RegisterComponent]
public sealed partial class DiseaseProtectionComponent : Component
{
    [DataField]
    public Dictionary<DiseaseSpreading, float> ProtectedSpreads = [];
}
