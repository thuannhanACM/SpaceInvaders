using Game.Gameplay;
using UnityEngine;

public class PlayerShipIputHandler : MonoBehaviour
{
    private BattleController _battleController;

    public void RegisterBattleController(BattleController battle)
    {
        _battleController = battle;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);

        transform.position += move;
    }
}
