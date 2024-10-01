using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._SCP.Scps.Oldman.Components;

[RegisterComponent]
public sealed partial class CorrosivePuddleComponent : Component
{
    [DataField]
    public TimeSpan decayTimer = TimeSpan.FromSeconds(10f);

    public TimeSpan decayStart;

    public bool shouldDecay = false;
    public bool isDecaying = false;
}
