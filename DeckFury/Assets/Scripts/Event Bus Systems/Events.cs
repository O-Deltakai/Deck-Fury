public interface IEvent { }

public struct ItemEvent : IEvent
{
    public ItemBase itemBase;
    public ItemSO itemSO;
}