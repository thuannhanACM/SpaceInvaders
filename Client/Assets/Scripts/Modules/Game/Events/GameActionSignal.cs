
namespace Core.Framework
{
    public class GameActionSignal
    {
        public GameAction Action { get; private set; }

        public GameActionSignal(GameAction action)
        {
            Action = action;
        }
    }

    public class GameActionSignal<TModel>
    {
        public TModel NewModel { get; private set; }
        public GameAction Action { get; private set; }

        public GameActionSignal(GameAction action, TModel newModel)
        {
            if (newModel == null)
                throw new GameActionModelIsNull();

            Action = action;
            NewModel = newModel;
        }

        public class GameActionModelIsNull : System.Exception { }
    }

    public enum GameAction
    {
        ModuleCreate,
        ModuleRemove,
        ModuleHide,
        ModuleShow,
        ScreenChange,
        ScreenReEnter,
        ScreenForceChange
    }
}
