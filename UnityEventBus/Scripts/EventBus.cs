using System;
using System.Collections.Generic;
using System.Linq;

public class EventBus
{
    #region fields

    private readonly Dictionary<Type, List<WeakReference<IBaseEventReceiver>>> _receivers;
    private readonly Dictionary<string, WeakReference<IBaseEventReceiver>> _receiverHashToReference;

    #endregion

    #region constructors

    public EventBus()
    {
        _receivers = new Dictionary<Type, List<WeakReference<IBaseEventReceiver>>>();
        _receiverHashToReference = new Dictionary<string, WeakReference<IBaseEventReceiver>>();
    }

    #endregion

    #region public methods

    public void Register<T>(IEventReceiver<T> receiver) where T : struct, IEvent
    {
        Type eventType = typeof(T);
        if (!_receivers.ContainsKey(eventType))
            _receivers[eventType] = new List<WeakReference<IBaseEventReceiver>>();

        if (!_receiverHashToReference.TryGetValue(receiver.Id, out WeakReference<IBaseEventReceiver> reference))
        {
            reference = new WeakReference<IBaseEventReceiver>(receiver);
            _receiverHashToReference[receiver.Id] = reference;
        }

        _receivers[eventType].Add(reference);
    }

    public void Unregister<T>(IEventReceiver<T> receiver) where T : struct, IEvent
    {
        Type eventType = typeof(T);
        if (!_receivers.ContainsKey(eventType) || !_receiverHashToReference.ContainsKey(receiver.Id))
            return;

        WeakReference<IBaseEventReceiver> reference = _receiverHashToReference[receiver.Id];

        _receivers[eventType].Remove(reference);

        int weakRefCount = _receivers.SelectMany(x => x.Value).Count(x => x == reference);
        if (weakRefCount == 0)
            _receiverHashToReference.Remove(receiver.Id);
    }

    public void Raise<T>(T @event) where T : struct, IEvent
    {
        Type eventType = typeof(T);
        if (!_receivers.ContainsKey(eventType))
            return;

        List<WeakReference<IBaseEventReceiver>> references = _receivers[eventType];
        for (int i = references.Count - 1; i >= 0; i--)
        {
            if (references[i].TryGetTarget(out IBaseEventReceiver receiver))
                ((IEventReceiver<T>)receiver).OnEvent(@event);
        }
    }

    #endregion
}