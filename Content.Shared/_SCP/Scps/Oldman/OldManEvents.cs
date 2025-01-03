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

[NetSerializable,Serializable] public sealed class OldManSpawnEvent : EventArgs {}

public sealed partial class PocketDimensionEnterEvent : InstantActionEvent { }
[NetSerializable,Serializable] public sealed partial class PocketDimensionEnterDoAfterEvent : SimpleDoAfterEvent { }

[NetSerializable, Serializable] public sealed class PocketDimensionCaptureEvent : EventArgs  {}

[NetSerializable, Serializable] public sealed class PocketDimensionPerishEvent : EventArgs  {}

[NetSerializable, Serializable] public sealed partial class PocketDimensionEscapeDoAfterEvent : SimpleDoAfterEvent {}
