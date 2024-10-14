using Content.Shared._SCP.Scps.Oldman.Components;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.Mind;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System.Numerics;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;

namespace Content.Shared._SCP.Scps.Oldman;
public sealed class SharedPocketDimensionSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly MapLoaderSystem _map = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly SharedColorFlashEffectSystem _color = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;

    public const string pocketDimensionMapPath = "/Maps/_SCP/testpocket.yml";
    public override void Initialize()
    {
        // SubscribeLocalEvent<PocketDimensionHolderComponent,ComponentStartup>(OnStartup);
        //
        // SubscribeLocalEvent<PocketDimensionHolderComponent, MeleeHitEvent>(OnSend);
        //
        // SubscribeLocalEvent<PocketDimensionInhabitantComponent, MobStateChangedEvent>(OnStateChange);
    }

    public override void Update(float frameTime)
    {
        var people = EntityQueryEnumerator<PocketDimensionInhabitantComponent>();

        while (people.MoveNext(out var uid, out var person))
        {
            if (person.lastDamaged + person.damageInterval < _timing.CurTime)
            {
                person.lastDamaged = _timing.CurTime;
                TickPocketDimension(uid,person);
            }
        }
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
    }

    //Enter Pocket Dimension

    //Get Taken to Pocket Dimension
    private void OnSend(EntityUid owner, PocketDimensionHolderComponent comp, MeleeHitEvent args)
    {
        if (comp.pocketDimensionGrid == null)
            return;

        foreach (var entity in args.HitEntities)
        {
            if (HasComp<PocketDimensionInhabitantComponent>(entity))
                return;

            if (!HasComp<HumanoidAppearanceComponent>(entity)) //Better check for a player
                return;

            if (HasComp<PocketDimensionHolderComponent>(entity))
                return;

            if (!TryComp<TransformComponent>(entity, out var transform))
                return;

            var dweller = AddComp<PocketDimensionInhabitantComponent>(entity);
            EnterPocketDimension(owner,dweller,owner,comp,comp.pocketDimensionGrid.Value);
        }
    }

    private void EnterPocketDimension(
        EntityUid inhabitantUid, PocketDimensionInhabitantComponent inhabitant,
        EntityUid dimensionHolderUid, PocketDimensionHolderComponent dimensionHolder,
        EntityUid gridId)
    {

        inhabitant.dimensionOwner = dimensionHolderUid;
        var spawnVectors = Vector2.Zero; //Change once corrosive puddles work

        _transform.SetCoordinates(inhabitantUid, new EntityCoordinates(gridId, spawnVectors));

        if (!_mind.TryGetMind(inhabitantUid, out var _, out var mind))
            return;
        if(mind.Session==null)
            return;

        _alerts.ShowAlert(inhabitantUid, AlertType.PocketDimension);
        _audio.PlayGlobal(dimensionHolder.laughSound, mind.Session);
    }

    //Take Damage in Pocket Dimension
    private void TickPocketDimension(EntityUid uid, PocketDimensionInhabitantComponent person)
    {
        if (!_prototypeManager.TryIndex(person.damageProto, out var damageType))
            return;

        var damageSpecifier = new DamageSpecifier(damageType,person.damageOverTime);
        _damage.TryChangeDamage(uid, damageSpecifier);

        _color.RaiseEffect(Color.Red, new List<EntityUid>() { uid }, Filter.Pvs(uid, entityManager: EntityManager));

        _audio.PlayPvs(person.HitNoise, uid);
    }

    //Die in pocket dimension
    private void OnStateChange(EntityUid uid, PocketDimensionInhabitantComponent comp, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Critical || args.NewMobState == MobState.Dead)
        {
            if (!TryComp<PocketDimensionHolderComponent>(comp.dimensionOwner, out var dimensionOwner))
                return;
            _audio.PlayPvs(dimensionOwner.puddleSound, uid);
            QueueDel(uid);
        }
    }
}
