using Robust.Shared.Prototypes;
using Content.Shared._SCP.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;

// ReSharper disable CheckNamespace
namespace Content.Shared.Roles;
// ReSharper restore CheckNamespace


public sealed partial class JobPrototype : IInheritingPrototype, ISCPSpecific
{
    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<JobPrototype>))]
    public string[]? Parents { get; }

    [NeverPushInheritance]
    [AbstractDataField]
    public bool Abstract { get; }

    [DataField]
    public bool IsSCP { get; }

    [DataField]
    public readonly bool Hidden;

}
