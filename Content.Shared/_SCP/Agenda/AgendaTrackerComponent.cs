using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._SCP.Agenda;

/// <summary>
/// Stores information about a sites' current agenda .
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class AgendaTrackerComponent : Component
{
    /// <summary>
    /// Current total objectives.
    /// </summary>
    [DataField("prototypeId", customTypeSerializer: typeof(PrototypeIdSerializer<AgendaObjectivePrototype>))]
    [AutoNetworkedField]
    public string? PrototypeId;

}
