using Content.Shared.Alert;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._SCP.Scps.Oldman.Components;

[RegisterComponent,NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class PocketDimensionInhabitantComponent : Component
{
    public EntityUid ownedPuddle;

    [AutoNetworkedField]
    public EntityUid dimensionOwner;

    [DataField]
    public ProtoId<AlertPrototype> PocketDimensionAlert = "PocketDimension";

    public TimeSpan lastDamaged;

    [DataField]
    public TimeSpan damageInterval = TimeSpan.FromSeconds(5f);

    [DataField]
    public SoundSpecifier HitNoise = new SoundPathSpecifier("/Audio/_SCP/Effects/106noise.ogg");

    [DataField]
    public int damageOverTime = 1;

    [DataField]
    public ProtoId<DamageTypePrototype> damageProto = "Heat";

}
