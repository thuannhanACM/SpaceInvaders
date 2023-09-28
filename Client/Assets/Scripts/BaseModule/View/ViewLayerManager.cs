using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ViewLayerManager : MonoBehaviour
{
    public enum ViewLayer
    {
        Back,
        Main,
        Top,
        None
    }

    [SerializeField]
    private List<Transform> _layers = new List<Transform>();

    public Transform GetLayerRoot(ViewLayer layer)
    {
        if (layer == ViewLayer.None)
            return null;
        return _layers[(int)layer];
    }
}
