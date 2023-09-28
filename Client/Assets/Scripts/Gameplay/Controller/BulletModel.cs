using Core.Business;
using Core.Framework;
using Cysharp.Threading.Tasks;
using Game.Gameplay;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

public class BulletModel : BasePoolObj
{
    #region injection
    [Inject]
    protected readonly ILogger _logger;
    [Inject]
    protected readonly SignalBus _signalBus;
    #endregion

    #region members
    private BattleController _battleController;
    private BulletDefinition _bulletDef;
    public bool IsAlive = true;
    #endregion

    #region Properties
    public BattleController BattleController => _battleController;
    public float MoveSpd => _bulletDef.MoveSpd;
    public float Radius => _bulletDef.Radius;
    #endregion

    public void Init(BulletDefinition bulletDefinition,
                    BattleController battleController,
                    Vector3 initPos)
    {
        _bulletDef = bulletDefinition;
        _battleController = battleController;

        transform.position = initPos;
        ModelObj.GetComponent<BulletController>().Init(this);
    }

    public override UniTask Reinitialize()
    {
        IsAlive = true;
        return UniTask.CompletedTask;
    }

    public async void OutOScreen()
    {
        await UniTask.Delay(1000);
        SelfDespawn();
    }

    public bool IsOutOfScreen()
    {
        return _battleController.IsOutOfTopBoundary(transform.position, Radius);
    }

    public void HitEnemy(int instanceId)
    {
        _signalBus.Fire<BattleSignal>(new BattleSignal(BattleAction.EnemyHit, instanceId.ToString()));
        SelfDespawn();
    }
}
