using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Shared._SCP.Scps.Oldman.Components;
using Content.Shared.Weapons.Melee;

public sealed partial class TogglePocketDimension : InstantActionEvent { }


[NetSerializable,Serializable]
public sealed partial class TogglePocketDimensionDoAfter : SimpleDoAfterEvent { }

public sealed partial class OldManSpawn : EventArgs { }

[NetSerializable, Serializable]
public sealed class EnterPocketDimensionEvent : EventArgs  {}
[NetSerializable, Serializable]
public sealed class DieInPocketDimensionEvent : EventArgs  {}
[NetSerializable, Serializable]
public sealed partial class EscapePocketDimensionDoAfterEvent : SimpleDoAfterEvent
{}
