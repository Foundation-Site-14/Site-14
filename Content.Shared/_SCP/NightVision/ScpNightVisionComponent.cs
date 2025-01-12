using Content.Shared.Alert;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._SCP.NightVision;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(SharedNightVisionSystem))]
public sealed partial class ScpNightVisionComponent : Component
{
    [DataField]
    public ProtoId<AlertPrototype> Alert = "NightVision";

    [DataField, AutoNetworkedField]
    public NightVisionState State = NightVisionState.Full;

    [DataField, AutoNetworkedField]
    public bool Overlay;

    [DataField, AutoNetworkedField]
    public bool Innate;

    [DataField, AutoNetworkedField]
    public bool SeeThroughContainers;

}

[Serializable, NetSerializable]
public enum NightVisionState
{
    Off,
    Half,
    Full
}
