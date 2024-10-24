using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._SCP.Scps.Oldman.Components;

[RegisterComponent]
public sealed partial class TraversePocketDimensionActionComponent : Component
{
    [DataField]
    public TimeSpan cooldownEnter = TimeSpan.FromSeconds(10f);

    [DataField]
    public TimeSpan cooldownExit = TimeSpan.FromMinutes(1f);
}

[RegisterComponent]
public sealed partial class CreateTeleportNodeComponent : Component
{
    [DataField]
    public TimeSpan cooldown = TimeSpan.FromSeconds(10f);

    [DataField]
    public TimeSpan destroyCooldown = TimeSpan.FromSeconds(5f);
}

[RegisterComponent]
public sealed partial class DestroyTeleportNodeComponent : Component { }

[RegisterComponent]
public sealed partial class TraverseTeleportNodeComponent : Component
{
    [DataField]
    public TimeSpan teleportCooldown = TimeSpan.FromSeconds(30f);
}
