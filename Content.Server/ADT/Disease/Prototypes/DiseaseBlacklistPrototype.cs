using Robust.Shared.Prototypes;

namespace Content.Server.ADT.Disease.Prototypes;

[Prototype("diseaseBlacklistPrototype")]
public sealed class DiseaseBlacklistPrototype : IPrototype
{
    [DataField]
    public List<string> BlackListReagents { get; set; } = default!;

    [IdDataField]
    public string ID { get; set; } = default!;
}
