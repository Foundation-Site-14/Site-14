using Content.Shared._SCP.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

// ReSharper disable CheckNamespace
namespace Content.Shared.Roles;
// ReSharper restore CheckNamespace

public sealed partial class DepartmentPrototype : IInheritingPrototype, ISCPSpecific
{
    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<DepartmentPrototype>))]
    public string[]? Parents { get; }

    [NeverPushInheritance]
    [AbstractDataField]
    public bool Abstract { get; }

    [DataField]
    public bool IsSCP { get; }

    [DataField]
    public string? CustomName;

    [DataField]
    public bool Hidden;
}
