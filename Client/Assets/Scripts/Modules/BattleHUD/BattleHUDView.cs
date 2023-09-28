using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Core.Framework
{

    public class BattleHUDView : View<BattleHUD>
    {
        #region Injection
        private ILogger _logger;
        private SignalBus _signalBus;
        #endregion

        #region SerializeField
        [SerializeField] private GameObject _startGamePanel;
        [SerializeField] private Button _startBtn;
        [SerializeField] private GameObject _gameoverPanel; 
        [SerializeField] private Button _restartBtn;
        [SerializeField] private TMP_Text[] _scoreTxts;
        #endregion

        private bool _gameStart = false;

        [Inject]
        public void Construct(
            ILogger logger,
            SignalBus signal)
        {
            _logger = logger;
            _signalBus = signal;
        }
        public override void OnReady()
        {
            _signalBus.Subscribe<BattleSignal>(OnBattleAction);
            _startBtn.onClick.AddListener(StartBattle);
            _restartBtn.onClick.AddListener(RestartBattle);
        }

        private void StartBattle()
        {
            _startGamePanel.SetActive(false);
            _signalBus.Fire<BattleSignal>(new BattleSignal(BattleAction.StartBattle));
        }
        
        private void RestartBattle()
        {
            _startGamePanel.SetActive(true);
            _gameoverPanel.SetActive(false);
            _signalBus.Fire<BattleSignal>(new BattleSignal(BattleAction.Restart));
        }

        private void Update()
        {
            if (_gameStart)
            {
                foreach (var txt in _scoreTxts)
                    txt.text = $"SCORE: {Module.Model.Score}";
            }
        }

        private void OnBattleAction(BattleSignal data)
        {
            switch (data.Action)
            {
                case BattleAction.StartBattle:
                    _gameStart = true;
                    break;
                case BattleAction.GameOver: 
                    _gameStart = false;
                    _gameoverPanel.SetActive(true);
                    foreach (var txt in _scoreTxts)
                        txt.text = $"SCORE: {Module.Model.Score}";
                    break;
            }
        }
    }
     
}
