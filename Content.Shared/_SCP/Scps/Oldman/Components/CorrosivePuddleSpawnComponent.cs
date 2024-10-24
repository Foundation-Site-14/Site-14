namespace Content.Shared._SCP.Scps.Oldman.Components;

[RegisterComponent]
public sealed partial class CorrosivePuddleSpawnComponent : Component
{
    [DataField,ViewVariables(VVAccess.ReadWrite)]
    public bool shouldUnclaim; //If the spawn should be claimed after the timeToUnclaim has elapsed

    [DataField,ViewVariables(VVAccess.ReadWrite)]
    public bool claimed; //If the spawn is claimed

    [DataField,ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan timeClaimed = TimeSpan.Zero; //The time that this puddle was claimed

    [DataField,ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan timeToUnclaim = TimeSpan.FromSeconds(30f); //The time for a puddle to unclaim automatically
}
