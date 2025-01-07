using System.Linq;
using Content.Server.Station.Systems;
using Content.Server._SCP.Agenda.Components;
using Content.Shared._SCP.Agenda;
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
        SubscribeLocalEvent<StationInitializedEvent>(OnStationInitialized);

    }

    private HashSet<T> ExtractPrototypes<T>(HashSet<string> prototypeIds) where T : class, IPrototype
    {
        var prototypes = new HashSet<T>();
        foreach (var prototypeId in prototypeIds)
        {
            if (!_prototypeManager.TryIndex(prototypeId, out T? prototype))
                continue;
            prototypes.Add(prototype);
        }
        return prototypes;
    }

    /// <summary>
    /// Returns all available objectives for the site.
    /// </summary>
    public IReadOnlySet<string> GetAvailableObjectives(EntityUid station, AgendaComponent? agenda = null)
    {
        if (!Resolve(station, ref agenda))
            throw new ArgumentException("Tried to use a non-station entity as a station!", nameof(station));

        return agenda.AvailableObjectives.ToHashSet();
    }

    private void OnStationInitialized(StationInitializedEvent msg)
    {
        if (!TryComp<AgendaComponent>(msg.Station, out var agenda))
            return;

        var objectives = agenda.AvailableObjectives;
        var objectivePrototypes = ExtractPrototypes<AgendaObjectivePrototype>(objectives);

        foreach (var agendaObjectivePrototype in objectivePrototypes)
        {
            Logger.Debug(agendaObjectivePrototype.ID);
        }
    }
}

