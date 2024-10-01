using Content.Shared._SCP.Scps.Oldman.Components;
using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.Mind;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Player;

namespace Content.Shared._SCP.Scps.Oldman;
public sealed class SharedOldManSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly SharedColorFlashEffectSystem _color = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly MobStateSystem _state = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<PocketDimensionHolderComponent, ComponentStartup>(OnStartComponent);

        SubscribeLocalEvent<PocketDimensionHolderComponent, TogglePocketDimension>(OnTogglePocket);
        SubscribeLocalEvent<PocketDimensionHolderComponent, CreateTeleportNodeEvent>(OnCreateNode);
        SubscribeLocalEvent<PocketDimensionHolderComponent, TraverseTeleportNodeEvent>(OnTraverseNode);

        SubscribeLocalEvent<PocketDimensionInhabitantComponent, EnterPocketDimension>(OnEnter);
    }

    #region startup
    public void OnEnter(EntityUid owner, PocketDimensionInhabitantComponent comp, EnterPocketDimension args)
    {
        if (!TryComp<PocketDimensionHolderComponent>(comp.dimensionOwner, out var pocket))
            return;
        if (!_mind.TryGetMind(owner, out var _, out var mind))
            return;
        if (mind.Session == null)
            return;
        _alerts.ShowAlert(owner, AlertType.PocketDimension);
        _audio.PlayGlobal(pocket.laughSound, mind.Session);
    }

    public void OnStartComponent(EntityUid owner, PocketDimensionHolderComponent comp, ComponentStartup args)
    {
        _actions.AddAction(owner, comp.traversePocketAction);
        _actions.AddAction(owner, comp.createNodeAction);
        EntityManager.EventBus.RaiseComponentEvent<OldManSpawn>(comp, new OldManSpawn());
    }

    #endregion

    #region actions
    public void OnTogglePocket(EntityUid owner, PocketDimensionHolderComponent pocket, TogglePocketDimension args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        var ev = new TogglePocketDimensionDoAfter();
        var popup = "scp-oldman-traversepocket";

        var doAfterArgs = new DoAfterArgs(EntityManager, args.Performer, TimeSpan.FromSeconds(3f), ev, args.Performer)
        {
            BreakOnUserMove = true,
        };
        if (_doAfter.TryStartDoAfter(doAfterArgs))
            _popup.PopupPredicted(Loc.GetString(popup), args.Performer, args.Performer, PopupType.LargeCaution);
    }

    public void OnCreateNode(EntityUid owner, PocketDimensionHolderComponent pocket, CreateTeleportNodeEvent args)
    {
        if (args.Handled)
            return;
        args.Handled = true;

        if (pocket.inPocketDimension)
        {
            _popup.PopupClient(Loc.GetString("scp-oldman-node-inpocket"), args.Performer, args.Performer, PopupType.MediumCaution);
            return;
        }

        var ev = new CreateTeleportNodeDoAfterEvent();
        var popup = "scp-oldman-createnode";

        var doAfterArgs = new DoAfterArgs(EntityManager, args.Performer, TimeSpan.FromSeconds(3f), ev, args.Performer)
        {
            BreakOnUserMove = true,
        };
        if (_doAfter.TryStartDoAfter(doAfterArgs))
            _popup.PopupClient(Loc.GetString(popup), args.Performer, args.Performer, PopupType.Medium);
    }

    public void OnTraverseNode(EntityUid owner, PocketDimensionHolderComponent pocket, TraverseTeleportNodeEvent args)
    {
        if (args.Handled)
            return;
        args.Handled = true;

        if (pocket.inPocketDimension)
        {
            _popup.PopupClient(Loc.GetString("scp-oldman-node-inpocket"), args.Performer, args.Performer, PopupType.MediumCaution);
            return;
        }

        var ev = new TraverseTeleportNodeDoAfterEvent();
        var popup = "scp-oldman-traversepocket";

        var doAfterArgs = new DoAfterArgs(EntityManager, args.Performer, TimeSpan.FromSeconds(3f), ev, args.Performer)
        {
            BreakOnUserMove = true,
        };
        if (_doAfter.TryStartDoAfter(doAfterArgs))
            _popup.PopupPredicted(Loc.GetString(popup), args.Performer, args.Performer, PopupType.LargeCaution);
    }
    #endregion

    public bool GetAction<T>(EntityUid uid, [NotNullWhen(true)] out T? comp, out EntityUid id) where T : IComponent
    {
        foreach (var item in _actions.GetActions(uid))
        {
            if (TryComp<T>(item.Id, out var traverse))
            {
                id = item.Id;
                comp = traverse;
                return true;
            }
        }
        comp = default;
        id = uid;
        return false;
    }

    public override void Update(float frameTime)
    {
        var puddles = EntityQueryEnumerator<CorrosivePuddleComponent>();

        while (puddles.MoveNext(out var uid, out var puddle))
        {
            if (puddle.shouldDecay && !puddle.isDecaying)
            {
                puddle.decayStart = _timing.CurTime;
                puddle.isDecaying = true;
            }
            else if (puddle.isDecaying)
            {
                if (puddle.decayStart + puddle.decayTimer < _timing.CurTime)
                    QueueDel(uid);
            }
        }

        var people = EntityQueryEnumerator<PocketDimensionInhabitantComponent>();

        while (people.MoveNext(out var uid, out var person))
        {
            if (person.lastDamaged + person.damageInterval < _timing.CurTime)
            {
                if (!TryComp<PocketDimensionHolderComponent>(person.dimensionOwner, out var _))
                    continue;
                if (!_prototypeManager.TryIndex(person.damageProto, out var damageType))
                    continue;

                var downer = Comp<MeleeWeaponComponent>(person.dimensionOwner);

                DamageSpecifier damages = new DamageSpecifier(damageType, person.damageOverTime);
                _damage.TryChangeDamage(uid, downer.Damage);

                _color.RaiseEffect(Color.Red, new List<EntityUid>() { uid }, Filter.Pvs(uid, entityManager: EntityManager));

                person.lastDamaged = _timing.CurTime;

                if (!_mind.TryGetMind(uid, out var _, out var mind))
                    continue;
                if (mind.Session == null)
                    continue;
                _audio.PlayGlobal(person.HitNoise, mind.Session);
            }
        }
    }
}
