using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Robust.Shared.Prototypes;

namespace Content.Shared._SCP.Prototypes;

public static class SCPPrototypeExtensions
{
    public static bool FilterSCP = true;

    public static IEnumerable<T> EnumerateSCP<T>(this IPrototypeManager prototypes) where T : class, IPrototype, ISCPSpecific
    {
        var protos = prototypes.EnumeratePrototypes<T>();
        if (FilterSCP)
            protos = protos.Where(p => p.IsSCP);

        return protos;
    }

    public static bool TrySCP<T>(this IPrototypeManager prototypes, string id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype, ISCPSpecific
    {
        prototype = default;

        if (!prototypes.TryIndex(id, out T? proto))
            return false;

        if (FilterSCP && !proto.IsSCP)
            return false;

        prototype = proto;
        return true;
    }
}
