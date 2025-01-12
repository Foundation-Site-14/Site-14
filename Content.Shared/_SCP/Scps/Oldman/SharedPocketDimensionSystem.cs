using Content.Shared._SCP.Scps.Oldman.Components;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.Mind;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System.Numerics;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Shared._SCP.Scps.Oldman;
public sealed class SharedPocketDimensionSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly SharedColorFlashEffectSystem _color = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<PocketDimensionHolderComponent,OldManSpawnEvent>(OnSpawn);
        SubscribeLocalEvent<PocketDimensionHolderComponent, PocketDimensionEnterEvent>(OnTryEnter);

        SubscribeLocalEvent<CorrosivePuddleComponent,ActivateInWorldEvent>(OnTryExit);

        SubscribeLocalEvent<PocketDimensionInhabitantComponent, PocketDimensionCaptureEvent>(OnSend);
        SubscribeLocalEvent<PocketDimensionInhabitantComponent, PocketDimensionPerishEvent>(OnStateChange);
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
    //Spawn
    private void OnSpawn(EntityUid holder, PocketDimensionHolderComponent comp, OldManSpawnEvent e)
    {
        _actions.AddAction(holder, comp.enterPocketAction);
    }

    //Enter Pocket Dimension TODO
    private void OnTryEnter(EntityUid uid, PocketDimensionHolderComponent comp, PocketDimensionEnterEvent args)
    {
        var doAfterArgs = new DoAfterArgs(EntityManager, args.Performer, TimeSpan.FromSeconds(3f), new PocketDimensionEnterDoAfterEvent(),args.Performer)
        {
            BreakOnMove = true,
            BreakOnDamage = false
        };

        if(_doAfter.TryStartDoAfter(doAfterArgs))
            _popup.PopupPredicted(Loc.GetString("scp-oldman-enterpocket"),uid,uid);
    }

    //Get Taken to Pocket Dimension
    private void OnSend(EntityUid inhabitant, PocketDimensionInhabitantComponent component, PocketDimensionCaptureEvent e)
    {

        if (!_mind.TryGetMind(inhabitant, out var _, out var mind))
            return;
        if(mind.Session==null)
            return;
        if(!TryComp<PocketDimensionHolderComponent>(component.dimensionOwner, out var holder))
            return;

        _alerts.ShowAlert(inhabitant, component.PocketDimensionAlert);
        _audio.PlayGlobal(holder.laughSound, mind.Session);
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
    private void OnStateChange(EntityUid uid, PocketDimensionInhabitantComponent comp, PocketDimensionPerishEvent args)
    {
        if (!TryComp<PocketDimensionHolderComponent>(comp.dimensionOwner, out var dimensionOwner))
            return;
        _audio.PlayPvs(dimensionOwner.puddleSound, uid);
        QueueDel(uid);
    }

    private void OnTryExit(EntityUid uid, CorrosivePuddleComponent comp, ActivateInWorldEvent args)
    {
        if(!TryComp<CorrosivePuddleComponent>(uid, out var puddle))
            return;

        if(!puddle.isParent)
            return;

        var doAfterArgs = new DoAfterArgs(EntityManager, args.User, TimeSpan.FromSeconds(3f), new PocketDimensionEscapeDoAfterEvent(),args.User,args.Target)
        {
            BreakOnMove = true,
            BreakOnDamage = false
        };

        if(_doAfter.TryStartDoAfter(doAfterArgs))
            _popup.PopupPredicted(Loc.GetString("scp-oldman-escapepocket"),uid,uid);
    }
}
