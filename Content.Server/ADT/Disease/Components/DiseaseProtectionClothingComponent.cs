using Content.Server.ADT.Disease.Data;

namespace Content.Server.ADT.Disease.Components;

[RegisterComponent]
public sealed partial class DiseaseProtectionClothingComponent : Component
{
    [DataField]
    public Dictionary<DiseaseSpreading, float> DiseaseProtection = [];
}
