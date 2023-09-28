using Core.Framework;
using Core.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Game.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        #region Injection
        private ILogger _logger;
        [Inject(Id = PoolName.Object)]
        protected readonly IPoolManager _poolManager;
        #endregion

        #region serializeFields
        [SerializeField] private string _battleControllerPath;
        #endregion

        #region private 
        private BattleController _battleController;
        #endregion

        [Inject]
        public void Construct(ILogger logger)
        {
            _logger = logger;
        }

        public async UniTask LoadBattle(string sceneId = "")
        {
            _battleController = await _poolManager.GetObject<BattleController>(_battleControllerPath);
            await _battleController.Init();
        }
    }
}