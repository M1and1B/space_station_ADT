using Content.Server.Body.Systems;
using Content.Shared.Damage;

namespace Content.Server.ADT.Disease.Effects;

[ImplicitDataDefinitionForInheritors]
public abstract partial class DiseaseEffect
{
    [DataField]
    public DamageSpecifier? HealthDamage;

    public virtual void OnStart(IEntityManager entityManager, EntityUid target)
    {
    }

    public virtual void Execute(IEntityManager entityManager, EntityUid target)
    {
        ApplyBasicDamage(entityManager, target);
    }

    private void ApplyBasicDamage(IEntityManager entityManager, EntityUid target)
    {
        var damageSystem = entityManager.System<DamageableSystem>();
        if (HealthDamage is { } healthDamage)
            damageSystem.TryChangeDamage(target, healthDamage);
    }

    public virtual void Dispose(IEntityManager entityManager, EntityUid target)
    {
    }
}
