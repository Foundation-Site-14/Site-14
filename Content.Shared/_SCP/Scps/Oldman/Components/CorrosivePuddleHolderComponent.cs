using Content.Shared.Alert;
using Robust.Shared.Prototypes;


namespace Content.Shared._SCP.Scps.Oldman.Components;

[RegisterComponent]
public sealed partial class CorrosivePuddleHolderComponent : Component
{
    public EntityUid puddle;

    public ProtoId<AlertPrototype> PocketDimensionAlert = "PocketDimension";

}
