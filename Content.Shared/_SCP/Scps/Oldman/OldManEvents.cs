using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed partial class TogglePocketDimension : InstantActionEvent { }


[NetSerializable,Serializable]
public sealed partial class TogglePocketDimensionDoAfter : SimpleDoAfterEvent { }

public sealed partial class OldManSpawn : EventArgs { }

[NetSerializable,Serializable]
public sealed partial class EnterPocketDimension : EventArgs { }

public sealed partial class CreateTeleportNodeEvent : InstantActionEvent { }
[NetSerializable,Serializable]
public sealed partial class CreateTeleportNodeDoAfterEvent : SimpleDoAfterEvent { }
public sealed partial class DestroyTeleportNodeEvent : InstantActionEvent { }
public sealed partial class TraverseTeleportNodeEvent : InstantActionEvent { }
[NetSerializable,Serializable]
public sealed partial class TraverseTeleportNodeDoAfterEvent : SimpleDoAfterEvent { }
