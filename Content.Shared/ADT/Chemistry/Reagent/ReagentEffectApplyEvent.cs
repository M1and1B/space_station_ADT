using Content.Shared.EntityEffects;

namespace Content.Shared.Chemistry.ADT.Reagent;
[ByRefEvent]
public record struct ReagentEffectApplyEvent(EntityEffectReagentArgs Args);