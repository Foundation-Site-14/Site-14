using System.Linq;
using Content.Server.Station.Systems;
using Content.Server._SCP.Agenda.Components;
using Content.Shared._SCP.Agenda;
using Content.Shared._SCP.Prototypes;
using Content.Shared.Mobs;
using Robust.Shared.Prototypes;

namespace Content.Server._SCP.Agenda;

/// <summary>
/// This handles the serverside code for the agenda system.
/// </summary>
public sealed class AgendaSystem : EntitySystem

{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override void Initialize()
    {
        Logger.Debug("Agenda system initializing");
        SubscribeLocalEvent<StationInitializedEvent>(OnStationInitialized);
        SubscribeLocalEvent<AgendaTrackerComponent,MobStateChangedEvent>(OnDead);
    }

    private AgendaObjectivePrototype? GetProto(ProtoId<AgendaObjectivePrototype> protoId)
    {
        _prototypeManager.TryIndex(protoId, out var prototype);
        return prototype;
    }

    private void CompleteObjective(ProtoId<AgendaObjectivePrototype> protoId)
    {
        var prototype = GetProto(protoId);
        if (prototype == null)
            return;

        prototype.Completed = true;
    }

    private ObjectiveType GetGoal(ProtoId<AgendaObjectivePrototype> protoId)
    {
        var prototype = GetProto(protoId);

        if (prototype == null)
            return ObjectiveType.None;

        return prototype.ObjectiveGoal;

    }

    private void OnDead(EntityUid inhabitant, AgendaTrackerComponent comp, MobStateChangedEvent args)
    {

        if (GetGoal(comp.PrototypeId) != ObjectiveType.Terminate)
            return;

        if (args.NewMobState is not (MobState.Dead or MobState.Critical))
            return;

        Logger.Debug("SCP terminated");
        CompleteObjective(comp.PrototypeId);
    }

    private void OnStationInitialized(StationInitializedEvent msg)
    {
        if (!TryComp<AgendaComponent>(msg.Station, out var agenda))
            return;

        var objectives = agenda.AvailableObjectives;

        foreach (var agendaObjectivePrototype in objectives)
        {
            Logger.Debug(agendaObjectivePrototype.Id);
        }
    }
}
