using Core.Business;
using Core.Framework;
using Core.Framework.Utilities;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Game.Gameplay
{
    public struct SpaceBoundary
    {
        public float Up;
        public float Down;
        public float Left;
        public float Right;
    }

    public class BattleController : BasePoolObj, ITickable
    {
        #region injection
        [Inject]
        protected readonly ILogger _logger;
        [Inject]
        protected IConfigTableManager _configTable;
        [Inject]
        protected readonly GameManager _gameManager;
        [Inject]
        protected readonly SignalBus _signalBus;
        [Inject]
        protected readonly GameStore _gameStore;
        #endregion

        #region members
        private BattleGroundSetup _battleGroundSetup;
        private SpaceBoundary _boundaries;
        private ShipDefinition[] _shipDefinitions;
        private BulletDefinition[] _bulletDefinitions;
        private AlienDefinition[] _alienDefinitions;
        private IModuleContextModel _battleHudModel;
        private ShipController _shipController;
        private List<AlienModel> _alienControllers = new List<AlienModel>();
        private List<int> _despawningInstanceIds = new List<int>();

        private bool _gameStart = false;
        #endregion

        #region Properties
        public List<AlienModel> AlienControllers => _alienControllers;
        #endregion

        private float _lastTickTime;

        public async UniTask Init()
        {
            _lastTickTime = Time.time;
            _signalBus.Subscribe<BattleSignal>(OnBattleAction);

            CalculateGameBoundaries();
            await LoadAllDefinitions();

            await PrepareBattle();

            _gameStore.GState.TryGetModel(ModuleName.BattleHUD, out _battleHudModel);
        }

        private async UniTask PrepareBattle()
        {
            var spawnPlayerShip = LoadPLayerShip();
            var spawnAliens = SpawnAliens();
            await UniTask.WhenAll(spawnPlayerShip, spawnAliens);
        }

        private async UniTask LoadAllDefinitions()
        {
            _shipDefinitions = await _configTable.GetDefinitionAll<ShipDefinition>();
            _bulletDefinitions = await _configTable.GetDefinitionAll<BulletDefinition>();
            _alienDefinitions = await _configTable.GetDefinitionAll<AlienDefinition>();
        }

        private void CalculateGameBoundaries()
        {
            _battleGroundSetup = ModelObj.GetComponent<BattleGroundSetup>();
            float targetCamDepth = 20f;
            Vector3 bottomLeftCorner = _battleGroundSetup.Camera.ScreenToWorldPoint(new Vector3(0, 0, targetCamDepth));
            Vector3 topRightCorner = _battleGroundSetup.Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, targetCamDepth));

            _boundaries = new SpaceBoundary()
            {
                Up = topRightCorner.y,
                Down = bottomLeftCorner.y,
                Left = bottomLeftCorner.x,
                Right = topRightCorner.x,
            };
        }

        private async UniTask LoadPLayerShip()
        {
            int randomIdx = Random.Range(0, _shipDefinitions.Length);
            var shipDef = _shipDefinitions[randomIdx];
            _shipController = await _poolManager.GetObject<ShipController>(shipDef.SkinPath);
            _shipController.Init(shipDef, _bulletDefinitions, this, _battleGroundSetup.PlayerStartTF.position);
        }

        private async UniTask SpawnAliens()
        {
            _alienControllers = new List<AlienModel>();
            int numAlienPerLine = Random.Range((int)_battleGroundSetup.NumAlientPerLine.x, (int)_battleGroundSetup.NumAlientPerLine.y + 1);
            int numAlienLine = Random.Range((int)_battleGroundSetup.NumAlientLine.x, (int)_battleGroundSetup.NumAlientLine.y + 1);

            float alientHeight = 2f;
            for (int i = 0; i < numAlienLine; i++)
            {
                await SpawnAlienLine(alientHeight * i, numAlienPerLine);
            }
        }

        private async UniTask SpawnAlienLine(float linePosY, int numAlien)
        {
            float alienWidth = 2f;
            float totalAlienSpaces = alienWidth * numAlien;
            float startPosX = -totalAlienSpaces / 2;
            for (int i = 0; i < numAlien; i++)
            {
                int randomIdx = Random.Range(0, _alienDefinitions.Length);
                var alienDef = _alienDefinitions[randomIdx];
                AlienModel alienController = await _poolManager.GetObject<AlienModel>(alienDef.SkinPath);
                Vector3 pos = new Vector3(startPosX + i * alienWidth, linePosY, 0);
                alienController.Init(_alienControllers.Count, alienDef, this, pos);
                _alienControllers.Add(alienController);
            }
        }

        public override UniTask Reinitialize()
        {
            return UniTask.CompletedTask;
        }

        private void OnBattleAction(BattleSignal signal)
        {
            OnBattleAction(signal.Action, signal.Data);
        }

        private void OnBattleAction(BattleAction action, string data)
        {
            switch (action)
            {
                case BattleAction.StartBattle:
                    HandleStartBattle();
                    break;
                case BattleAction.Restart:
                    HandleRestartBattle();
                    break;
                case BattleAction.GameOver:
                    HandleGameOver();
                    break;
                case BattleAction.EnemyHit:
                    _despawningInstanceIds.Add(int.Parse(data));
                    break;
            }
        }

        private void HandleStartBattle()
        {
            _gameStart = true;
        }

        private async void HandleRestartBattle()
        {
            _shipController.SelfDespawn();
            foreach (var alien in _alienControllers) {
                alien.SelfDespawn();
            }
            (_battleHudModel as BattleHUDModel).Score = 0;
            await PrepareBattle();
        }
        
        private void HandleGameOver()
        {
            _gameStart = false;
        }

        public override void SelfDespawn()
        {
            _signalBus.Unsubscribe<BattleSignal>(OnBattleAction);
            base.SelfDespawn();
        }

        public Vector3 GetValidMovement(Vector3 curPos, Vector3 moveStep, float shipRadius)
        {
            Vector3 newPos = curPos + moveStep;
            if (newPos.x - shipRadius < _boundaries.Left)
                return Vector3.zero;

            if (newPos.x + shipRadius > _boundaries.Right)
                return Vector3.zero;
            return moveStep;
        }

        public bool IsOutOfTopBoundary(Vector3 pos, float radius)
        {
            float bulletBottomBoundary = pos.y - radius;
            return (bulletBottomBoundary > _boundaries.Up);
        }

        public bool IsReachingBottomBoundary(Vector3 pos, float radius)
        {
            float bulletBottomBoundary = pos.y - radius;
            return (bulletBottomBoundary < _boundaries.Down);
        }

        public void Tick()
        {
            float dt = Time.time - _lastTickTime;
            _lastTickTime = Time.time;
            
            if (!_gameStart)
                return;

            _shipController.InternalUpdate(dt);
            UpdateAliens(dt);
        }

        private void UpdateAliens(float dt)
        {
            foreach (var instanceId in _despawningInstanceIds)
            {
                var controller = _alienControllers.Find(_ => _.InstanceId == instanceId);
                _alienControllers.Remove(controller);
                if (_battleHudModel != null)
                    (_battleHudModel as BattleHUDModel).Score += controller.KillPoint;
                controller.SelfDespawn();
            }
            _despawningInstanceIds.Clear();

            if (_alienControllers.Count <= 0)
                _signalBus.Fire<BattleSignal>(new BattleSignal(BattleAction.GameOver));
        }
    }
};