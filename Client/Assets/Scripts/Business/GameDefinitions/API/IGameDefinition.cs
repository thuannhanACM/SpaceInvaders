
using System;

namespace Core.Business
{
    public interface IGameDefinition
    {
        string Id { get; set; }
    }

    [Serializable]
    public abstract class BaseGameDefinition: IGameDefinition
    {
        public string Id { get; set; }
    }
}
