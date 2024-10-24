using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared._SCP.Scps.Oldman.Components;
using Robust.Shared.Timing;

namespace Content.Server._SCP.Scps.Oldman;

public sealed class PocketDimensionSpawnSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _time = default!;

    public bool TryGetRandomPair(PocketDimensionHolderComponent holder, out Vector2 puddleSpawn, out Vector2 playerSpawn)
    {
        puddleSpawn = Vector2.Zero;
        playerSpawn = Vector2.Zero;

        if (!holder.pocketDimensionGrid.HasValue)
            return false;

        if (GetHolderPuddles(holder.pocketDimensionGrid.Value).Count == 0)
            return false;

        puddleSpawn = GetRandomSpawn(holder);
        playerSpawn = GetRandomSpawn(holder,true);
        return true;
    }

    public bool TryGetRandomSpawn(PocketDimensionHolderComponent holder, out Vector2 spawn)
    {
        spawn = Vector2.Zero;

        if (!holder.pocketDimensionGrid.HasValue)
            return false;

        if (GetHolderPuddles(holder.pocketDimensionGrid.Value).Count == 0)
            return false;

        spawn = GetRandomSpawn(holder,true);
        return true;
    }

    private Vector2 GetRandomSpawn(PocketDimensionHolderComponent holder, bool timedUnclaim = false)
    {
        if(holder.pocketDimensionGrid == null)
            return Vector2.Zero;

        var puddles = GetHolderPuddles(holder.pocketDimensionGrid.Value);

        var random = new Random();

        return HandleMarking(puddles[random.Next(puddles.Count)],timedUnclaim);
    }

    private Vector2 HandleMarking((CorrosivePuddleSpawnComponent, TransformComponent) entry,bool timedUnclaim)
    {
        var spawn = entry.Item1;
        var transform = entry.Item2;

        spawn.claimed = true;
        spawn.timeClaimed = _time.CurTime;
        spawn.shouldUnclaim = timedUnclaim;;

        return transform.Coordinates.Position;
    }

    private List<(CorrosivePuddleSpawnComponent, TransformComponent)> GetHolderPuddles(
        EntityUid grid)
    {
        var query = EntityQuery<CorrosivePuddleSpawnComponent,TransformComponent>().ToList();

        var puddles = query.Where(p => p.Item2.GridUid == grid && p.Item1.claimed == false).ToList();

        return puddles;
    }
}
