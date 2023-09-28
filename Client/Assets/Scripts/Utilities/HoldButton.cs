using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldButton : MonoBehaviour
{
    private Action<int, RectTransform> _onHoldAction;
    private Action<int, RectTransform> _onReleaseAction;

    private int _actionIntParam;
    // Start is called before the first frame update

    public void RegisterIntActions(int value, Action<int, RectTransform> onHoldAction, Action<int, RectTransform> onReleaseAction)
    {
        _actionIntParam = value;
        _onHoldAction = onHoldAction;
        _onReleaseAction = onReleaseAction;
    }

    public void OnHold()
    {
        _onHoldAction?.Invoke(_actionIntParam, GetComponent<RectTransform>());
    }

    public void OnReleases()
    {
        _onReleaseAction?.Invoke(_actionIntParam, GetComponent<RectTransform>());
    }
}
