using Content.Server.Station.Systems;
using Content.Server._SCP.Agenda.Components;

namespace Content.Server._SCP.Agenda;

/// <summary>
/// This handles...
/// </summary>
public sealed class AgendaSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<StationInitializedEvent>(OnStationInitialized);

    }

    private void OnStationInitialized(StationInitializedEvent msg)
    {
        // Check if the comp exists if not return
        if (!TryComp<AgendaComponent>(msg.Station, out var agenda))
            return;

        Logger.Debug("Found the site agenda component");


    }
}
