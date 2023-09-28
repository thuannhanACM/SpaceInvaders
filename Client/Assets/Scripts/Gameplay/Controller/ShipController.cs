using Core.Business;
using Core.Framework;
using Cysharp.Threading.Tasks;
using Game.Gameplay;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

public class ShipController : BasePoolObj
{
    #region injection
    [Inject]
    protected readonly ILogger _logger;
    [Inject]
    protected readonly SignalBus _signalBus;
    #endregion
    
    private BattleController _battleController;
    private ShipDefinition _shipDef;
    private BulletDefinition[] _bulletDefs;

    private bool _gameStart = false;
    private Quaternion _idleRotation;
    private Quaternion _strafeLeftRotation;
    private Quaternion _strafeRightRotation;


    public void Init(ShipDefinition shipDef, 
                    BulletDefinition[] bulletDefinitions,
                    BattleController battleController,
                    Vector3 initPos)
    {
        _signalBus.Subscribe<BattleSignal>(OnBattleAction);
        _shipDef = shipDef;
        _bulletDefs = bulletDefinitions;
        _battleController = battleController;

        ModelObj.transform.position = initPos;

        _strafeLeftRotation = Quaternion.Euler(0, 45, 0);
        _strafeRightRotation = Quaternion.Euler(0, -45, 0);
        _idleRotation = Quaternion.Euler(0, 0, 0);
    }

    public override UniTask Reinitialize()
    {
        return UniTask.CompletedTask;
    }

    public void InternalUpdate(float dt)
    {
        if (!_gameStart)
            return;

        float horizontalAxis = Input.GetAxis("Horizontal");
        Vector3 moveDir = new Vector3(horizontalAxis, 0, 0);

        UpdateRotation(horizontalAxis);

        transform.position += _battleController.GetValidMovement(transform.position, moveDir * _shipDef.MoveSpeed * dt, _shipDef.Radius);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireRandomBullet();
        }
    }

    private void UpdateRotation(float horizontalAxis)
    {
        Quaternion targetRotation = _idleRotation;
        if (horizontalAxis < 0)
            targetRotation = _strafeLeftRotation;
        else if (horizontalAxis > 0)
            targetRotation = _strafeRightRotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180 * Time.deltaTime);
    }

    private async void FireRandomBullet()
    {
        int r = Random.Range(0, _bulletDefs.Length);
        BulletDefinition def = _bulletDefs[r];

        BulletModel bulletController = await _poolManager.GetObject<BulletModel>(def.SkinPath);
        bulletController.Init(def, _battleController, transform.position);
    }

    private void OnBattleAction(BattleSignal signal)
    {
        OnBattleAction(signal.Action, signal.Data);
    }

    private void OnBattleAction(BattleAction action, string data)
    {
        switch (action)
        {
            case BattleAction.StartBattle:
                _gameStart = true;
                break;
            case BattleAction.GameOver:
                _gameStart = false;
                break;
        }
    }

    public override void SelfDespawn()
    {
        base.SelfDespawn();
        _signalBus.Unsubscribe<BattleSignal>(OnBattleAction);
    }
}
