namespace Content.Shared._SCP.Scps.PlaugeDoctor.Components;


[RegisterComponent]
public sealed partial class CoversSkinComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("full")]
    public bool IsFullCover;
}
