using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventSender : MonoBehaviour
{
    public enum TypeOfEvent
    {
        Red,
        Green,
        Blue
    }

    #region fields

    [SerializeField] private TypeOfEvent _eventType;
    [SerializeField] private EventBusHolder _busHolder;

    #endregion

    #region engine methods

    private void OnMouseDown()
    {
        switch (_eventType)
        {
            case TypeOfEvent.Red:
                _busHolder.EventBus.Raise(new RedEvent(Vector3.one * 0.15f));
                break;

            case TypeOfEvent.Green:
                _busHolder.EventBus.Raise(new GreenEvent(Vector3.one * 0.25f));
                break;

            case TypeOfEvent.Blue:
                _busHolder.EventBus.Raise(new BlueEvent(Color.cyan * Random.Range(0f, 1f)));
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}