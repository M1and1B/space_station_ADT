using Content.Server.ADT.Disease.Effects;

namespace Content.Server.ADT.Disease.Data;

[DataDefinition]
public sealed partial class DiseaseSymptom
{
    [DataField(required: true)]
    public string Name { get; set; } = default!;

    [DataField(required: true)]
    public List<DiseaseEffect> Effects { get; set; } = default!;
}
