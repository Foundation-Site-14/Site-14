using System.Data;
using System.Numerics;
using Content.Shared._SCP.Scps.Oldman.Components;
using Content.Shared.Alert;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server._SCP.Scps.Oldman;

public sealed class PocketDimensionHolderSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly MapLoaderSystem _map = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly CorrosivePuddleSystem _puddle = default!;
    [Dependency] private readonly PocketDimensionSpawnSystem _spawn = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;

    public const string pocketDimensionMapPath = "/Maps/_SCP/testpocket.yml";

    public override void Initialize()
    {
        SubscribeLocalEvent<PocketDimensionHolderComponent,ComponentStartup>(OnStartup);
        SubscribeLocalEvent<PocketDimensionHolderComponent,MeleeHitEvent>(OnSend);
        SubscribeLocalEvent<PocketDimensionHolderComponent,PocketDimensionEnterDoAfterEvent>(OnEnter);

        SubscribeLocalEvent<PocketDimensionInhabitantComponent,MobStateChangedEvent>(OnDie);
        SubscribeLocalEvent<CorrosivePuddleHolderComponent,PocketDimensionEscapeDoAfterEvent>(OnExit);
    }

    //Dimension Setup
    private void OnStartup(EntityUid owner, PocketDimensionHolderComponent comp, ComponentStartup args)
    {
        if (comp.pocketDimensionGrid == null)
        {
            var map = _mapManager.GetMapEntityId(_mapManager.CreateMap());
            _metaData.SetEntityName(map, "Pocket Dimension");

            var grids = _map.LoadMap(Comp<MapComponent>(map).MapId, pocketDimensionMapPath);
            if (grids.Count > 0)
            {
                _metaData.SetEntityName(grids[0], "Pocket Dimension Grid");
                comp.pocketDimensionGrid = grids[0];
            }
            comp.pocketDimensionMap = map;
        }

        EntityManager.EventBus.RaiseComponentEvent(owner,comp, new OldManSpawnEvent());
    }

    //Enter Dimension
    private void OnEnter(EntityUid holder, PocketDimensionHolderComponent comp, PocketDimensionEnterDoAfterEvent args)
    {
        if (comp.pocketDimensionGrid == null)
            return;

        if (!_spawn.TryGetRandomPair(comp, out var puddleSpawn, out var playerSpawn))
            return;

        _puddle.CreatePuddle(holder, comp, puddleSpawn);
        _transform.SetCoordinates(holder, new EntityCoordinates(comp.pocketDimensionGrid.Value, playerSpawn));
    }

    //Capture to Dimension
    private void OnSend(EntityUid owner, PocketDimensionHolderComponent comp, MeleeHitEvent args)
    {
        if (comp.pocketDimensionGrid == null)
            return;

        foreach (var entity in args.HitEntities)
        {
            if (HasComp<PocketDimensionInhabitantComponent>(entity))
                return;

            if (!HasComp<HumanoidAppearanceComponent>(entity)) //Better check for a player if needed
                return;

            if (HasComp<PocketDimensionHolderComponent>(entity))
                return;

            if (!HasComp<TransformComponent>(entity))
                return;

            var dweller = AddComp<PocketDimensionInhabitantComponent>(entity);
            dweller.dimensionOwner = owner;

            if (!_spawn.TryGetRandomPair(comp, out var puddleSpawn, out var playerSpawn))
                return;

            _puddle.CreatePuddle(entity, comp, puddleSpawn);
            _transform.SetCoordinates(entity, new EntityCoordinates(comp.pocketDimensionGrid.Value, playerSpawn));

            EntityManager.EventBus.RaiseComponentEvent(entity,dweller,new PocketDimensionCaptureEvent());
        }
    }

    private void OnDie(EntityUid inhabitant, PocketDimensionInhabitantComponent comp, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Dead || args.NewMobState == MobState.Critical)
        {
            _puddle.DeletePuddle(inhabitant);
            EntityManager.EventBus.RaiseComponentEvent(inhabitant,comp,new PocketDimensionPerishEvent());
        }
    }

    private void OnExit(EntityUid uid, CorrosivePuddleHolderComponent comp, PocketDimensionEscapeDoAfterEvent args)
    {
        var puddle = Comp<TransformComponent>(_puddle.GetLinkedPuddle(args.Target.GetValueOrDefault()));

        _transform.SetCoordinates(uid,puddle.Coordinates);

        _puddle.DeletePuddle(uid);
        _alerts.ClearAlert(uid, comp.PocketDimensionAlert);

        RemComp<CorrosivePuddleHolderComponent>(uid);
        RemComp<PocketDimensionInhabitantComponent>(uid);
    }
}
