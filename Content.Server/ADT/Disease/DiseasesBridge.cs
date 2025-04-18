using Content.Server.ADT.Disease.Systems;
using Content.Shared.FixedPoint;
using Content.Server.ADT.Bridges;

namespace Content.Server.ADT.Disease;

public sealed class DiseasesBridge : IDiseasesBridge
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public void TransferDiseasesContact(EntityUid recipient, EntityUid donor)
    {
        _entityManager.System<DiseaseSystem>().TransferDiseasesContact(recipient, donor);
    }

    public bool CanHealDisease(EntityUid entityUid, EntityUid target)
    {
        return _entityManager.System<DiseaseSystem>().CanHealDisease(entityUid, target);
    }

    public bool TryHealDisease(EntityUid entityUid, EntityUid target)
    {
        return _entityManager.System<DiseaseSystem>().TryHealDiseaseExternal(entityUid, target);
    }

    public void IncreaseImmunityByVitamins(EntityUid uid, FixedPoint2 quantity)
    {
        _entityManager.System<DiseasesImmunitySystem>().IncreaseImmunityByVitamins(uid, quantity);
    }
}
