using Content.Shared._SCP.Agenda;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Server._SCP.Agenda.Components;


/// <summary>
/// Stores information about a sites' current agenda .
/// </summary>
[RegisterComponent, Access(typeof(AgendaSystem)), PublicAPI]
public sealed partial class AgendaComponent : Component
{
    /// <summary>
    /// Current total objectives.
    /// </summary>
    [DataField("totalObjectives")] public int TotalObjectives;

    /// <summary>
    /// If objectives are randomly picked or not.
    /// </summary>
    [DataField("setObjectives")] public bool SetObjectives = false;

    /// <summary>
    /// A list of all available objectives that can be randomly picked from.
    /// </summary>
    [DataField("availableObjectives", required: true)]
    public HashSet<ProtoId<AgendaObjectivePrototype>> AvailableObjectives = new();

    [DataField("objectiveList")]
    public HashSet<ProtoId<AgendaObjectivePrototype>> ObjectiveList = new();

}
