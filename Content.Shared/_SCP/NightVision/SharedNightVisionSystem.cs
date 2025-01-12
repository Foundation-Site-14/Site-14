using System.Diagnostics;
using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.Inventory.Events;
using Content.Shared.Rounding;
using Content.Shared.Toggleable;
using Robust.Shared.Timing;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;


namespace Content.Shared._SCP.NightVision;

public abstract class SharedNightVisionSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ClothingSystem _clothingSystem = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ScpNightVisionComponent, ComponentStartup>(OnNightVisionStartup);
        SubscribeLocalEvent<ScpNightVisionComponent, MapInitEvent>(OnNightVisionMapInit);
        SubscribeLocalEvent<ScpNightVisionComponent, AfterAutoHandleStateEvent>(OnNightVisionAfterHandle);
        SubscribeLocalEvent<ScpNightVisionComponent, ComponentRemove>(OnNightVisionRemove);
        SubscribeLocalEvent<ScpNightVisionItemComponent, GetItemActionsEvent>(OnNightVisionItemGetActions);
        SubscribeLocalEvent<ScpNightVisionItemComponent, ToggleActionEvent>(OnNightVisionItemToggle);
        SubscribeLocalEvent<ScpNightVisionItemComponent, GotEquippedEvent>(OnNightVisionItemGotEquipped);
        SubscribeLocalEvent<ScpNightVisionItemComponent, GotUnequippedEvent>(OnNightVisionItemGotUnequipped);
        SubscribeLocalEvent<ScpNightVisionItemComponent, ActionRemovedEvent>(OnNightVisionItemActionRemoved);
        SubscribeLocalEvent<ScpNightVisionItemComponent, ComponentRemove>(OnNightVisionItemRemove);
        SubscribeLocalEvent<ScpNightVisionItemComponent, EntityTerminatingEvent>(OnNightVisionItemTerminating);
    }

    private void OnNightVisionStartup(Entity<ScpNightVisionComponent> ent, ref ComponentStartup args)
    {
        NightVisionChanged(ent);
    }

    private void OnNightVisionAfterHandle(Entity<ScpNightVisionComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        NightVisionChanged(ent);
    }

    private void OnNightVisionMapInit(Entity<ScpNightVisionComponent> ent, ref MapInitEvent args)
    {
        UpdateAlert(ent);
    }

    private void OnNightVisionRemove(Entity<ScpNightVisionComponent> ent, ref ComponentRemove args)
    {
        if (ent.Comp.Alert is { } alert)
            _alerts.ClearAlert(ent, ent.Comp.Alert);

        NightVisionRemoved(ent);
    }

    private void OnNightVisionItemGetActions(Entity<ScpNightVisionItemComponent> ent, ref GetItemActionsEvent args)
    {
        if (args.InHands || !ent.Comp.Toggleable)
            return;

        args.AddAction(ref ent.Comp.Action, ent.Comp.ActionId);
    }

    private void OnNightVisionItemToggle(Entity<ScpNightVisionItemComponent> ent, ref ToggleActionEvent args)
    {

        args.Handled = true;
        ToggleNightVisionItem(ent, args.Performer);
    }

    private void OnNightVisionItemGotEquipped(Entity<ScpNightVisionItemComponent> ent, ref GotEquippedEvent args)
    {
        ToggleNightVisionItem(ent, args.Equipee);
    }

    private void OnNightVisionItemGotUnequipped(Entity<ScpNightVisionItemComponent> ent, ref GotUnequippedEvent args)
    {
        DisableNightVisionItem(ent, args.Equipee);
    }

    private void OnNightVisionItemActionRemoved(Entity<ScpNightVisionItemComponent> ent, ref ActionRemovedEvent args)
    {
        DisableNightVisionItem(ent, ent.Comp.User);
    }

    private void OnNightVisionItemRemove(Entity<ScpNightVisionItemComponent> ent, ref ComponentRemove args)
    {
        DisableNightVisionItem(ent, ent.Comp.User);
    }

    private void OnNightVisionItemTerminating(Entity<ScpNightVisionItemComponent> ent, ref EntityTerminatingEvent args)
    {
        DisableNightVisionItem(ent, ent.Comp.User);
    }

    public void Toggle(Entity<ScpNightVisionComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        ent.Comp.State = ent.Comp.State switch
        {
            NightVisionState.Off => NightVisionState.Half,
            NightVisionState.Half => NightVisionState.Full,
            NightVisionState.Full => NightVisionState.Off,
            _ => throw new ArgumentOutOfRangeException(),
        };

        Dirty(ent);
        UpdateAlert((ent, ent.Comp));
    }

    private void UpdateAlert(Entity<ScpNightVisionComponent> ent)
    {
        if (ent.Comp.Alert is { } alert)
        {
            var level = MathF.Max((int) NightVisionState.Off, (int) ent.Comp.State);
            var max = _alerts.GetMaxSeverity(ent.Comp.Alert);
            var severity = max - ContentHelpers.RoundToLevels(level, (int) NightVisionState.Full, max + 1);
            _alerts.ShowAlert(ent, ent.Comp.Alert, (short) severity);
        }

        NightVisionChanged(ent);
    }

    private void ToggleNightVisionItem(Entity<ScpNightVisionItemComponent> item, EntityUid user)
    {

        if (item.Comp.User == user && item.Comp.Toggleable)
        {
            DisableNightVisionItem(item, item.Comp.User);
            return;
        }

        EnableNightVisionItem(item, user);
    }

    private void EnableNightVisionItem(Entity<ScpNightVisionItemComponent> item, EntityUid user)
    {
        DisableNightVisionItem(item, item.Comp.User);
        var entity = item.Owner;
        var clothingComponent = _entManager.GetComponent<ClothingComponent>(entity);
        _clothingSystem.SetEquippedPrefix(entity, null, clothingComponent);

        item.Comp.User = user;
        Dirty(item);

        _appearance.SetData(item, NightVisionItemVisuals.Active, true);
        _appearance.SetData(item, NightVisionItemVisuals.Inactive, false);


        if (!_timing.ApplyingState)
        {
            var nightVision = EnsureComp<ScpNightVisionComponent>(user);
            var greenVision = EnsureComp<GreenVisionComponent>(user);
            nightVision.State = NightVisionState.Full;
            Dirty(user, nightVision);
            Dirty(user, greenVision);
        }

        _actions.SetToggled(item.Comp.Action, true);
    }

    protected virtual void NightVisionChanged(Entity<ScpNightVisionComponent> ent)
    {
    }

    protected virtual void NightVisionRemoved(Entity<ScpNightVisionComponent> ent)
    {
    }

    protected void DisableNightVisionItem(Entity<ScpNightVisionItemComponent> item, EntityUid? user)
    {
        _actions.SetToggled(item.Comp.Action, false);
        var entity = item.Owner;
        var clothingComponent = _entManager.GetComponent<ClothingComponent>(entity);
        Logger.Debug("test");
        _clothingSystem.SetEquippedPrefix(item, "up", clothingComponent); // pretend like i didnt do this for now someone can make it a component variable
        item.Comp.User = null;
        Dirty(item);

        _appearance.SetData(item, NightVisionItemVisuals.Active, false);
        _appearance.SetData(item, NightVisionItemVisuals.Inactive, true);

        if (TryComp(user, out ScpNightVisionComponent? nightVision) &&
            !nightVision.Innate)
        {
            RemCompDeferred<ScpNightVisionComponent>(user.Value);
            RemCompDeferred<GreenVisionComponent>(user.Value);
        }
    }

    public void SetSeeThroughContainers(Entity<ScpNightVisionComponent?> ent, bool see)
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return;

        ent.Comp.SeeThroughContainers = see;
        Dirty(ent);
    }
}
