using Game.Gameplay;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    private BattleController _battleController;
    private AlienModel _model;
    public int InstanceId => _model.InstanceId;
    public bool IsAlive => _model.IsAlive;

    public void AssignControllers(BattleController battleController, AlienModel model)
    {
        _battleController = battleController;
        _model = model;
    }

    private void Update()
    {
        if (!_model.IsGameStart)
            return;
        float dt = Time.deltaTime;
        float speedBoost = _battleController != null ? _battleController.AlienSpeedBooster : 1;
        if (_model.CurMoveState == AlienModel.MoveState.Down)
        {
            _model.MoveDownDuration -= dt;
            transform.position += _model.MoveDir* _model.Def.MoveSpd * speedBoost * dt;
            if (_model.MoveDownDuration <= 0f)
                _model.ChangeState();
            _model.CheckIfAlienReachBottomEdge();
        }
        else
        {
            Vector3 movement = _model.GetValidMovement(transform.position, _model.MoveDir * _model.Def.MoveSpd * speedBoost * dt);
            if (movement.magnitude == 0f)
            {
                _model.ChangeState();
            }
            else
                transform.position += movement;
        }
    }
}
