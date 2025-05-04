using Content.Server.Players;
using Content.Server.Popups;
using Content.Shared._SCP.Scps.PlagueDoctor.Components;
using Content.Shared._SCP.Scps.PlaugeDoctor.Components;
using Content.Shared._SCP.SkinContact;
using Content.Shared.Damage;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Timing;
using ZombieRevivableComponent = Content.Shared._SCP.Scps.PlagueDoctor.Components.ZombieRevivableComponent;


namespace Content.Server._SCP.Scps.PlagueDoctor;


public sealed class PlagueDoctorSystem : EntitySystem
{

    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<MobStateComponent,ActivateInWorldEvent>(OnTouch);
        SubscribeLocalEvent<CoversSkinComponent, InventoryRelayedEvent<SkinContactEvent>>(OnTouchCheck);
        SubscribeLocalEvent<ZombieRevivableComponent,MobStateChangedEvent>(TryRevive);
    }

    private void TryRevive(EntityUid uid, ZombieRevivableComponent component, MobStateChangedEvent args)
    {
        if(args.NewMobState==MobState.Dead) return;
        RemComp(uid, component);
    }

    private void OnTouch(EntityUid uid, MobStateComponent comp, ActivateInWorldEvent args)
    {
        if(comp.CurrentState==MobState.Dead) return;

        var user = args.User;
        if(!TryComp<PlagueDoctorComponent>(user, out var contactComponent)) return;

        var ev = new SkinContactEvent();
        RaiseLocalEvent(uid, ref ev);

        if (ev.IsFullyCovered || ev.CoverAmount == 5)
        {
            _popup.PopupEntity(Loc.GetString(contactComponent.FailureMessage),uid, user, PopupType.Medium);
            return;
        }

        _damage.TryChangeDamage(uid, contactComponent.Damage,origin:user);

        var zombieComp = new ZombieRevivableComponent();
        zombieComp.DeathTime = _timing.CurTime;
        AddComp(uid,zombieComp);
    }

    private void OnTouchCheck(Entity<CoversSkinComponent> ent, ref InventoryRelayedEvent<SkinContactEvent> args)
    {
        args.Args.CoverAmount++;
        if(ent.Comp.IsFullCover)
            args.Args.IsFullyCovered = true;
    }
}
