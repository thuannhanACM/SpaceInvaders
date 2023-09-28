using Core.Business;
using Core.Framework;
using Cysharp.Threading.Tasks;
using Game.Gameplay;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

public class AlienModel : BasePoolObj
{
    public struct AlienGroupData
    {
        public Vector3 MoveDir;
    }

    public enum MoveState
    {
        Right,
        Down,
        Left
    }
    #region injection
    [Inject]
    protected readonly ILogger _logger;
    [Inject]
    protected readonly SignalBus _signalBus;
    #endregion

    #region members
    private BattleController _battleController;
    private AlienDefinition _alienDef;
    private int _instanceId;

    private MoveState _moveState = MoveState.Right;
    private MoveState _lastHorizontalMove = MoveState.Right;

    
    public float MoveDownDuration = 2f;
    public bool IsGameStart = false;
    public bool IsAlive = true;
    public Vector3 MoveDir = Vector3.right;
    #endregion

    #region Properties
    public AlienDefinition Def => _alienDef;
    public int InstanceId => _instanceId;
    public int KillPoint => _alienDef.Point;
    public MoveState CurMoveState => _moveState;
    #endregion

    public void Init(int instanceId,
                    AlienDefinition alienDef, 
                    BattleController battleController,
                    Vector3 initPos)
    {
        _instanceId = instanceId;
        _signalBus.Subscribe<BattleSignal>(OnBattleAction);
        _alienDef = alienDef;
        
        _battleController = battleController;

        ModelObj.transform.position = initPos;
        ModelObj.GetComponent<AlienController>().Model = this;
    }

    public override UniTask Reinitialize()
    {
        IsAlive = true;
        return UniTask.CompletedTask;
    }

    private void UpdateMoveState()
    {
        if (_moveState == MoveState.Right || _moveState == MoveState.Left)
        {
            _lastHorizontalMove = _moveState;
            _moveState = MoveState.Down;
            MoveDownDuration = 0.5f;
        }
        else
        {
            if (_lastHorizontalMove == MoveState.Left)
            {
                _moveState = MoveState.Right;
            }
            else if (_lastHorizontalMove == MoveState.Right)
            {
                _moveState = MoveState.Left;
            }
        }

        switch (_moveState)
        {
            case MoveState.Left:
                MoveDir = Vector3.left;
                break;
            case MoveState.Right:
                MoveDir = Vector3.right;
                break;
            case MoveState.Down:
                MoveDir = Vector3.down;
                break;
        }
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
                IsGameStart = true;
                break;
            case BattleAction.GameOver:
                IsGameStart = false;
                break;
            case BattleAction.EnemyHit:
                if (data == InstanceId.ToString())
                {
                    IsAlive = false;
                }
                break;
            case BattleAction.InvaderHitEdge:
                UpdateMoveState();
                break;
        }
    }

    public override void SelfDespawn()
    {
        base.SelfDespawn();
        _signalBus.Unsubscribe<BattleSignal>(OnBattleAction);
    }

    public Vector3 GetValidMovement(Vector3 pos, Vector3 moveStep)
    {
        return _battleController.GetValidMovement(pos, moveStep, _alienDef.Radius);
    }

    public void ChangeState()
    {
        _signalBus.Fire<BattleSignal>(new BattleSignal(BattleAction.InvaderHitEdge));
    }

    public void CheckIfAlienReachBottomEdge()
    {
        if (_battleController.IsReachingBottomBoundary(transform.position, _alienDef.Radius))
            _signalBus.Fire<BattleSignal>(new BattleSignal(BattleAction.GameOver));
    }
}
