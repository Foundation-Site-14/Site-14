namespace Content.Shared._SCP.Scps.PlagueDoctor.Components;

[AutoGenerateComponentState]
[RegisterComponent]
public sealed partial class ZombieRevivableComponent : Component
{
    [AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan DeathTime;
}
