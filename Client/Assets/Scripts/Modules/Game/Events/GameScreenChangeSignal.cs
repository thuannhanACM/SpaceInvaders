namespace Core.Framework
{
    public class OnScreenChangeSignal
    {
        public ScreenName Current { get; private set; }
        public ScreenName Previous { get; private set; }

        public OnScreenChangeSignal(ScreenName screenName, ScreenName previousScreenName)
        {
            Current = screenName;
            Previous = previousScreenName;
        }
    }

    public class GameScreenForceChangeSignal
    {
        public ScreenName Current { get; private set; }
        public ScreenName Previous { get; private set; }

        public GameScreenForceChangeSignal(ScreenName screenName, ScreenName previousScreenName)
        {
            Current = screenName;
            Previous = previousScreenName;
        }
    }

    public class BattleSignal
    {
        public BattleAction Action { get; private set; }
        public string Data { get; private set; }

        public BattleSignal(BattleAction battleAction)
        {
            Action = battleAction;
        }

        public BattleSignal(BattleAction battleAction, string data)
        {
            Action = battleAction;
            Data = data;
        }
    }
}