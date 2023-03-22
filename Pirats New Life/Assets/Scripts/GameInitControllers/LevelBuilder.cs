using UnityEngine;
using System.Collections.Generic;
using System;
using GameInit.GameCyrcleModule;
using System.Resources;
using GameInit.Pool;
using GameInit.PoolPrefabs;
using GameInit.Builders;
using GameInit.AI;
using GameInit.Connector;
using GameInit.Animation;

namespace GameInit.Builders
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameCyrcle))]

    public class LevelBuilder : MonoBehaviour
    {
        private readonly List<IDisposable> _dispose = new List<IDisposable>();

        private void Awake()
        {
            var GameCyrcle = GetComponent<GameCyrcle>();
            var prefabHolder = FindObjectOfType<PrefabPoolHolderComponent>();

            Builders(GameCyrcle, prefabHolder);
        }

        private void Builders(GameCyrcle gameCyrcle, PrefabPoolHolderComponent prefabHolder)
        {
            Pools _coinPool = new Pools(prefabHolder.GetCoinPrefab());

            ResourceManager _resourceManager = new ResourceManager();

            CoinDropAnimation _coinDropAnimation = new CoinDropAnimation();

            AIConnector _AIConnector = new AIConnector(_coinPool);
            gameCyrcle.Add(_AIConnector);

            ChestBuilder _chestBuilder = new ChestBuilder(gameCyrcle, _resourceManager, _coinPool, _coinDropAnimation);
            
            HeroBuilder _heroBuilder = new HeroBuilder(gameCyrcle, _coinPool, _resourceManager, _AIConnector);

            UIBuilder _uiBuilder = new UIBuilder(_resourceManager);

            BuildingsBuilder _buildingsBuilder = new BuildingsBuilder(gameCyrcle, _resourceManager, _AIConnector);

            AIBuilder _aiBuilder = new AIBuilder(_AIConnector, _coinPool,_coinDropAnimation, _heroBuilder);

            WorkChecker _workChecker = new WorkChecker(_AIConnector, _coinDropAnimation,  _coinPool, gameCyrcle, _heroBuilder);
            gameCyrcle.Add(_workChecker);

            Hacks(_resourceManager);
        }

        private void Hacks(ResourceManager _resourceManager)
        {
            _resourceManager.SetResource(ResourceType.Gold, 11);
        }
        private void OnDestroy()
        {
            foreach (var item in _dispose)
            {
                item.Dispose();
            }
        }
    }
}