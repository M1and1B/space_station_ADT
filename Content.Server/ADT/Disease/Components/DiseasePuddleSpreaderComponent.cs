using Content.Server.ADT.Disease.Data;

namespace Content.Server.ADT.Disease.Components;

/**
 * Распространение болезни через жидкости на земле
 */
[RegisterComponent]
public sealed partial class DiseasePuddleSpreaderComponent : Component
{
    [DataField]
    public List<DiseaseData> Diseases { get; set; } = new();
}
