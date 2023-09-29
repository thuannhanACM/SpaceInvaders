using Core.Framework;
using Core.Infrastructure;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Game.Gameplay
{
    public class LocalizationManager : MonoBehaviour
    {
        #region Injection
        private ILogger _logger;
        [Inject(Id = PoolName.Object)]
        protected readonly IPoolManager _poolManager;
        #endregion

        [Inject]
        public void Construct(ILogger logger)
        {
            _logger = logger;
        }

        public string GetLocalText(LOCALIZE_KEY enumKey)
        {
            return GetLocalText(enumKey.ToString());
        }

        public string GetLocalText(string key)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedStringAsync(key).Result;
        }
    }

    public enum LOCALIZE_KEY
    {
        score,
        start,
        touch_to_replay,
        game_over,
        you_win,
        you_lose,
    }
}