using Content.Shared.Inventory;


namespace Content.Shared._SCP.SkinContact;


[ByRefEvent]
public sealed class SkinContactEvent : IInventoryRelayEvent
{
    public SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;
    public int CoverAmount;
    public bool IsFullyCovered;
}
