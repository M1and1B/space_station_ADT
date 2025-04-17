using Content.Server.Temperature.Systems;

namespace Content.Server.ADT.Disease.Effects;

[DataDefinition]
public sealed partial class DiseaseTemperatureEffect : DiseaseEffect
{
    [DataField]
    public float Temperature;

    public override void Execute(IEntityManager entityManager, EntityUid target)
    {
        base.Execute(entityManager, target);

        var temperature = entityManager.System<TemperatureSystem>();
        temperature.ChangeHeat(target, Temperature);
    }
}
