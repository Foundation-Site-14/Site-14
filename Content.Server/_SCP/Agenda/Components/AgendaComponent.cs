using Content.Shared.Roles;
using JetBrains.Annotations;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

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
    [DataField("availableObjectives", required: true, customTypeSerializer: typeof(PrototypeIdDictionarySerializer<uint?, AgendaObjectivePrototype>))]
    public Dictionary<string, uint?> AvailableObjectives = new();

    [DataField("objectiveList", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<uint?, AgendaObjectivePrototype>))]
    public Dictionary<string, uint?> ObjectiveList = new();

}
