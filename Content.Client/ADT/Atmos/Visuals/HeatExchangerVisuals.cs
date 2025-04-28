using Content.Shared.ADT.Atmos.Visuals;
using Robust.Shared.Serialization;
using Robust.Client.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Client.ADT.Atmos.Visuals;

public sealed class PipeVisualizer : Visualizer
{
    public override void InitializeEntity(EntityUid entity, SpriteComponent sprite, AppearanceComponent component)
    {
        sprite.LayerMapReserveBlank(PipeLayers.RadiatorOverlay);
        sprite.LayerSetVisible(PipeLayers.RadiatorOverlay, false);
    }

    public override void OnChangeData(EntityUid entity, SpriteComponent sprite, AppearanceComponent component, AppearanceChangeData changeData)
    {
        if (component.TryGetData(HeatExchangerVisuals.ConnectedToRadiator, out bool connected))
        {
            sprite.LayerSetVisible(PipeLayers.RadiatorOverlay, connected);
        }
    }
}

[Serializable, NetSerializable]
public enum PipeLayers : byte
{
    Base = 0,
    RadiatorOverlay = 1 << 1
}
