using Content.Shared.EntityEffects;

namespace Content.Shared.ADT.Chemistry.Reagent;
[ByRefEvent]
public record struct ReagentEffectApplyEvent(EntityEffectReagentArgs Args);