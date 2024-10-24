using System.Numerics;
using Content.Shared._SCP.Scps.Oldman.Components;
using Content.Shared.Coordinates;
using Robust.Shared.Map;
using Robust.Shared.Physics;

namespace Content.Server._SCP.Scps.Oldman;

public sealed class CorrosivePuddleSystem : EntitySystem
{
    public void CreatePuddle(EntityUid puddleOwner, PocketDimensionHolderComponent holder, Vector2 position)
    {
        if(!holder.pocketDimensionGrid.HasValue)
            return;

        var grid = holder.pocketDimensionGrid.Value;

        var puddleCoords = new EntityCoordinates(grid, position);

        var parentPuddleId = SpawnPuddle(puddleCoords, holder);
        var childPuddleId = SpawnPuddle(Comp<TransformComponent>(puddleOwner).Coordinates,holder);

        var parentPuddle = Comp<CorrosivePuddleComponent>(parentPuddleId);
        var childPuddle = Comp<CorrosivePuddleComponent>(childPuddleId);

        var puddleHolder = AddComp<CorrosivePuddleHolderComponent>(puddleOwner);

        puddleHolder.puddle = parentPuddleId;

        parentPuddle.puddleOwner = puddleOwner;
        parentPuddle.linkedPuddle = childPuddleId;
        parentPuddle.isParent = true;

        childPuddle.linkedPuddle = parentPuddleId;
    }

    public void DeletePuddle(EntityUid puddleOwner)
    {
        if (!TryComp<CorrosivePuddleHolderComponent>(puddleOwner,out var holder))
            return;

        var parentPuddle = holder.puddle;

        var childPuddle = Comp<CorrosivePuddleComponent>(parentPuddle).linkedPuddle;

        QueueDel(parentPuddle);
        QueueDel(childPuddle);
    }

    public bool TryEscape(EntityUid entity, out Vector2 position)
    {
        position = Vector2.Zero;
        if (!TryComp<CorrosivePuddleComponent>(entity, out var puddle))
            return false;

        if(!TryComp<TransformComponent>(puddle.linkedPuddle, out var transform))
            return false;

        position = transform.Coordinates.Position;
        return true;
    }

    public EntityUid GetLinkedPuddle(EntityUid uid)
    {
        if (!TryComp<CorrosivePuddleComponent>(uid, out var puddle))
            return default;

        return puddle.linkedPuddle;
    }

    private EntityUid SpawnPuddle(EntityCoordinates coords,PocketDimensionHolderComponent holder)
    {
        var puddle = SpawnAtPosition(holder.PocketPuddle, coords);
        Comp<TransformComponent>(puddle).LocalRotation = Angle.Zero;
        return puddle;
    }
}
