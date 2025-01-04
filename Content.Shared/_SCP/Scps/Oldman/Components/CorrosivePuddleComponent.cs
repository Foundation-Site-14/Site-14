using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._SCP.Scps.Oldman.Components;

[RegisterComponent]
public sealed partial class CorrosivePuddleComponent : Component
{
    public bool isParent = false; //If this puddle has an owner (AKA if its in the pocket dimension)
    public EntityUid linkedPuddle; //The puddle thats in the pocket dimension
    public EntityUid puddleOwner; //The entity that owns this puddle
}
