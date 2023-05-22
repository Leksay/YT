using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class EventReceiver : MonoBehaviour, IEventReceiver<RedEvent>, IEventReceiver<GreenEvent>, IEventReceiver<BlueEvent>
{
    #region fields

    [SerializeField] private EventBusHolder _eventBusHolder;
    private MeshRenderer _meshRenderer;

    #endregion

    #region engine methods

    private void OnEnable()
    {
        _eventBusHolder.EventBus.Register(this as IEventReceiver<RedEvent>);
        _eventBusHolder.EventBus.Register(this as IEventReceiver<GreenEvent>);
        _eventBusHolder.EventBus.Register(this as IEventReceiver<BlueEvent>);

        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnDestroy() => _meshRenderer.sharedMaterial.color = Color.white;

    private void OnDisable()
    {
        _eventBusHolder.EventBus.Unregister(this as IEventReceiver<RedEvent>);
        _eventBusHolder.EventBus.Unregister(this as IEventReceiver<GreenEvent>);
        _eventBusHolder.EventBus.Unregister(this as IEventReceiver<BlueEvent>);
    }

    #endregion

    #region IEventReceiver

    public void OnEvent(RedEvent @event)
    {
        transform.position += @event.MoveDelta;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void OnEvent(GreenEvent @event) => transform.localScale += @event.Scale;

    public void OnEvent(BlueEvent @event) => _meshRenderer.sharedMaterial.color = @event.Color;

    #endregion
}