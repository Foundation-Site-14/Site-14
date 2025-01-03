using Content.Shared.Alert;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._SCP.Scps.Oldman.Components;

[RegisterComponent,NetworkedComponent]
public sealed partial class PocketDimensionHolderComponent : Component
{
    public EntityUid? pocketDimensionGrid;
    public EntityUid? pocketDimensionMap;

    [DataField]
    public EntProtoId enterPocketAction = "ActionEnterPocketDimension";

    [DataField]
    public EntProtoId PocketPuddle = "OldManPuddle";

    [DataField]
    public SoundSpecifier puddleSound = new SoundPathSpecifier("/Audio/_SCP/Effects/106puddle.ogg");
    [DataField]
    public SoundSpecifier laughSound = new SoundPathSpecifier("/Audio/_SCP/Effects/106laugh.ogg");
}
