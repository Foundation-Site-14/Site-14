using Robust.Shared.Prototypes;

namespace Content.Shared._SCP.Agenda;

public enum ObjectiveType : byte
{
    Terminate = 0,
    Transport = 1,
    Tech = 2,
    Tests = 3,
    None = 4
}

/// <summary>
/// This is a prototype for agenda objectives
/// </summary>
[Prototype("agendaObjective")]
public sealed class AgendaObjectivePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;
    public string Text => Loc.GetString($"agenda-objective-{ID.ToLower()}");

    public bool Completed = false;

    [DataField("objectiveGoal")]
    public ObjectiveType ObjectiveGoal;

}
