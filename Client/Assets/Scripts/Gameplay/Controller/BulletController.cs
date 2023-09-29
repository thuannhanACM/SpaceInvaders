using Core.Business;
using Core.Framework;
using Game.Gameplay;
using UnityEngine;
using Zenject;

public class BulletController : MonoBehaviour
{
    private BulletModel _model;
    private static int _alienLayerMask = 0;

    public void Init(BulletModel controller)
    {
        if (_alienLayerMask == 0)
        {
            int layerIndex = LayerMask.NameToLayer("Alien");
            _alienLayerMask = 1 << layerIndex;
        }

        _model = controller;
        _model.IsAlive = true;
    }

    private void Update()
    {
        transform.position += Vector3.up * _model.MoveSpd * Time.deltaTime;
        if (!_model.IsAlive)
            return;
        
        if (_model.IsOutOfScreen())
        {
            _model.IsAlive = false;
            _model.OutOScreen();
            return;
        }

        var colliders = Physics.OverlapSphere(transform.position, _model.Radius, _alienLayerMask);
        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                AlienController controller = collider.GetComponent<AlienController>();
                if (controller.IsAlive)
                { 
                    _model.IsAlive = false;
                    _model.HitEnemy(controller.InstanceId);
                    break;
                }
            }
        }
    }
}
