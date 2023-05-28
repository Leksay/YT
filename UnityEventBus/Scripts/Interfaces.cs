// Пустой интерфейс для маркировки событий
public interface IEvent{}

// Базовый интерфейс для слушателей событий
public interface IBaseEventReceiver
{
    public UniqueId Id { get; }
}

// Интерфейс для параметризированных слушателей событий
public interface IEventReceiver<T> : IBaseEventReceiver where T : struct, IEvent
{
    void OnEvent(T @event);
}