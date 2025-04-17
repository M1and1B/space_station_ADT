using Content.Server.ADT.Disease.Effects;
using Robust.Shared.Prototypes;

namespace Content.Server.ADT.Disease.Prototypes;

[Prototype("symptom")]
public sealed class DiseaseSymptomPrototype : IPrototype
{
    [DataField(required: true)]
    public string Name { get; set; } = default!;

    [DataField(required: true)]
    public List<DiseaseEffect> Effects { get; set; } = default!;

    [IdDataField]
    public string ID { get; set; } = default!;
}
