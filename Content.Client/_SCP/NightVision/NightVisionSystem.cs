using Content.Shared._SCP.NightVision;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client._SCP.NightVision;

public sealed class NightVisionSystem : SharedNightVisionSystem
{
    [Dependency] private readonly ILightManager _light = default!;
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ScpNightVisionComponent, LocalPlayerAttachedEvent>(OnNightVisionAttached);
        SubscribeLocalEvent<ScpNightVisionComponent, LocalPlayerDetachedEvent>(OnNightVisionDetached);
    }

    private void OnNightVisionAttached(Entity<ScpNightVisionComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        NightVisionChanged(ent);
    }

    private void OnNightVisionDetached(Entity<ScpNightVisionComponent> ent, ref LocalPlayerDetachedEvent args)
    {
        Off();
    }

    protected override void NightVisionChanged(Entity<ScpNightVisionComponent> ent)
    {
        if (ent != _player.LocalEntity)
            return;

        switch (ent.Comp.State)
        {
            case NightVisionState.Off:
                Off();
                break;
            case NightVisionState.Half:
                Half(ent);
                break;
            case NightVisionState.Full:
                Full(ent);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void NightVisionRemoved(Entity<ScpNightVisionComponent> ent)
    {
        if (ent != _player.LocalEntity)
            return;

        Off();
    }

    private void Off()
    {
        _overlay.RemoveOverlay(new NightVisionOverlay());
        _light.DrawLighting = true;
    }

    private void Half(Entity<ScpNightVisionComponent> ent)
    {
        if (ent.Comp.Overlay)
            _overlay.AddOverlay(new NightVisionOverlay());

        _light.DrawLighting = true;
    }

    private void Full(Entity<ScpNightVisionComponent> ent)
    {
        if (ent.Comp.Overlay)
            _overlay.AddOverlay(new NightVisionOverlay());

        _light.DrawLighting = false;
    }
}
