using UnityEngine;

public class AlienController : MonoBehaviour
{
    private AlienModel _model;
    public AlienModel Model 
    {
        get {
            return _model;
        }
        set {
            _model = value;
        }
    }
    public int InstanceId => _model.InstanceId;
    public bool IsAlive => _model.IsAlive;

    private void Update()
    {
        if (!_model.IsGameStart)
            return;
        float dt = Time.deltaTime;
        if (_model.CurMoveState == AlienModel.MoveState.Down)
        {
            _model.MoveDownDuration -= dt;
            transform.position += _model.MoveDir* _model.Def.MoveSpd * dt;
            if (_model.MoveDownDuration <= 0f)
                _model.ChangeState();
            _model.CheckIfAlienReachBottomEdge();
        }
        else
        {
            Vector3 movement = _model.GetValidMovement(transform.position, _model.MoveDir * _model.Def.MoveSpd * dt);
            if (movement.magnitude == 0f)
            {
                _model.ChangeState();
            }
            else
                transform.position += movement;
        }
    }
}
