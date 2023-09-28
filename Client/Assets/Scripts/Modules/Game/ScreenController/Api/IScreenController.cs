
namespace Core.Framework
{
    public interface IScreenController
    {
        ScreenName Name { get; }
        bool IsAllowChangeScreen(ScreenName newScreen);
        void Enter();

        void Out();
    }

    public enum ScreenName
    {
        SessionStart = 0,
        GamePlay,
        Restart
    }

    public enum BattleAction
    {
        StartBattle,
        GameOver,
        Restart,
        EnemyHit,
        InvaderHitEdge
    }
}
