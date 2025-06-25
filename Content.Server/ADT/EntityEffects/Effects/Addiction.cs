using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.ADT.Addictions.Components;

namespace Content.Server.ADT.EntityEffects.Effects;

public sealed partial class Addiction : EntityEffect
{
    [Dependency] private readonly IRobustRandom _random = default!;

    [DataField("alcohol")]
    public bool Alcohol;

    [DataField("drugs")]
    public bool Drugs;

    [DataField("smoke")]
    public bool Smoke;

    [DataField("coffein")]
    public bool Coffein;

    [DataField("chance", required: true)]
    public int Chance = 100;

    public enum AddictionType
    {
        Alcohol,
        Drugs,
        Smoke,
        Coffein
    }

    public override void Effect(EntityEffectBaseArgs args)
    {
        if (args is not EntityEffectReagentArgs reagentArgs || reagentArgs.Source == null)
            return;

        var target = reagentArgs.TargetEntity;
        var scale = reagentArgs.Scale;

        var random = IoCManager.Resolve<IRobustRandom>();

        var effectiveChance = (int)(Chance * scale);
        if (effectiveChance <= 0)
            return;

        if (Alcohol && random.Prob(effectiveChance / 100f))
            EntityManager.EnsureComponent<AlcoholAddictionComponent>(target);

        if (Drugs && random.Prob(effectiveChance / 100f))
            EntityManager.EnsureComponent<DrugsAddictionComponent>(target);

        if (Tobacco && random.Prob(effectiveChance / 100f))
            EntityManager.EnsureComponent<TobaccoAddictionComponent>(target);

        if (Coffein && random.Prob(effectiveChance / 100f))
            EntityManager.EnsureComponent<CoffeinAddictionComponent>(target);
    }

    // Заглушка
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    => Loc.GetString("addiction-effect-guidebook", ("chance", Chance), ("types", "???"));
}