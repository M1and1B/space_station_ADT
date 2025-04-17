using Content.Server.ADT.Disease.Data;
using Content.Server.ADT.Disease.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.ADT.Disease.Components;

/**
 * Испольщуем для распространения болезней
 */
[RegisterComponent]
public sealed partial class DiseaseSpreaderComponent : Component
{
    [DataField]
    public Dictionary<DiseaseSpreading, List<ProtoId<DiseasePrototype>>> Diseases = new()
    {
        { DiseaseSpreading.Air, [] },
        { DiseaseSpreading.Contact, [] },
    };
}
