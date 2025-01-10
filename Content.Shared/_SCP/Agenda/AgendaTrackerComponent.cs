using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._SCP.Agenda;

/// <summary>
/// Stores information about a sites' current agenda .
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState(true)]
public sealed partial class AgendaTrackerComponent : Component
{
    /// <summary>
    /// Current total objectives.
    /// </summary>
    [DataField("prototypeId")]
    [AutoNetworkedField]
    public ProtoId<AgendaObjectivePrototype> PrototypeId;


}
