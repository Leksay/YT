using System;
using System.Collections.Generic;
using System.Linq;

public class EventBus
{
    #region fields

    private Dictionary<Type, List<WeakReference<IBaseEventReceiver>>> _receivers;
    private Dictionary<int, WeakReference<IBaseEventReceiver>> _receiverHashToReference;

    #endregion

    #region constructors

    public EventBus()
    {
        _receivers = new Dictionary<Type, List<WeakReference<IBaseEventReceiver>>>();
        _receiverHashToReference = new Dictionary<int, WeakReference<IBaseEventReceiver>>();
    }

    #endregion

    #region public methods

    public void Register<T>(IEventReceiver<T> receiver) where T : struct, IEvent
    {
        Type eventType = typeof(T);
        if (!_receivers.ContainsKey(eventType))
            _receivers[eventType] = new List<WeakReference<IBaseEventReceiver>>();

        int receiverHash = receiver.GetHashCode();
        if (!_receiverHashToReference.TryGetValue(receiverHash, out WeakReference<IBaseEventReceiver> reference))
        {
            reference = new WeakReference<IBaseEventReceiver>(receiver);
            _receiverHashToReference[receiverHash] = reference ;
        }

        _receivers[eventType].Add(reference);
    }

    public void Unregister<T>(IEventReceiver<T> receiver) where T: struct, IEvent
     {
        Type eventType = typeof(T);
        int receiverHash = receiver.GetHashCode();
        if(!_receivers.ContainsKey(eventType) || !_receiverHashToReference.ContainsKey(receiverHash))
            return;

        WeakReference<IBaseEventReceiver> reference = _receiverHashToReference[receiverHash];

        _receivers[eventType].Remove(reference);

        int weakRefCount = _receivers.SelectMany(x => x.Value).Count(x => x == reference);
        if (weakRefCount == 0)
            _receiverHashToReference.Remove(receiverHash);
     }

    public void Raise<T>(T @event) where T : struct, IEvent
    {
        Type eventType = typeof(T);
        if(!_receivers.ContainsKey(eventType))
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