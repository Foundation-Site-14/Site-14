using Content.Shared.Damage;


namespace Content.Shared._SCP.Scps.PlagueDoctor.Components;


[AutoGenerateComponentState]
[RegisterComponent]
public sealed partial class PlagueDoctorComponent : Component
{
    [AutoNetworkedField]
    [DataField("fail")]
    public string FailureMessage; //The message shown when the attack fails

    [AutoNetworkedField]
    [DataField("damage")]
    public DamageSpecifier Damage; //The damage to do on success

    [AutoNetworkedField]
    [DataField("reviveTime")]
    public TimeSpan ReviveTime;
}
