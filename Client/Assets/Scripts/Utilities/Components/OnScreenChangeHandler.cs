using Core.Framework;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class OnScreenChangeHandler : MonoBehaviour
{
    [SerializeField]
    private ScreenName _toScreen;
    [SerializeField]
    private UnityEvent _action;

    private SignalBus _signalBus;

    private bool _isUnsubscribed;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<OnScreenChangeSignal>(OnScreenChange);
    }

    private void UnsubscribeSignal()
    {
        if (_isUnsubscribed)
            return;
        _isUnsubscribed = true;
        _signalBus.Unsubscribe<OnScreenChangeSignal>(OnScreenChange);
    }

    private void OnScreenChange(OnScreenChangeSignal e)
    {
        if (e.Current == _toScreen && gameObject != null
            && gameObject.activeInHierarchy)
        {
            _action.Invoke();
        }
    }

    private void OnDisable()
    {
        UnsubscribeSignal();
    }

    private void OnDestroy()
    {
        UnsubscribeSignal();
    }
}
