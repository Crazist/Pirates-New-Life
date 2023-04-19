using UnityEngine;
using System.Collections.Generic;
using System;
using GameInit.GameCyrcleModule;
using GameInit.Pool;
using GameInit.PoolPrefabs;
using GameInit.AI;
using GameInit.Animation;
using GameInit.TraderLogic;

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
            EnemyPool _enemyPool = new EnemyPool(prefabHolder.GetEnemyPrefab());
            ArrowPool _arrowPool = new ArrowPool(prefabHolder.GetArrowPrefab());

            ResourceManager _resourceManager = new ResourceManager();

            UIBuilder _uiBuilder = new UIBuilder(_resourceManager);

            CoinDropAnimation _coinDropAnimation = new CoinDropAnimation();

            BuilderConnectors _builderConnectors = new BuilderConnectors(_coinPool, gameCyrcle, _resourceManager, _arrowPool);

            ChestBuilder _chestBuilder = new ChestBuilder(gameCyrcle, _resourceManager, _coinPool, _coinDropAnimation);
            
            HeroBuilder _heroBuilder = new HeroBuilder(gameCyrcle, _coinPool, _resourceManager, _builderConnectors, _builderConnectors, _uiBuilder);
            _builderConnectors.GetAiConnector().GetHeroComponent(_heroBuilder.GetHeroComponent());

            BuildingsBuilder _buildingsBuilder = new BuildingsBuilder(gameCyrcle, _resourceManager, _builderConnectors, _heroBuilder.GetHeroComponent(), _coinPool, _coinDropAnimation);

            AIBuilder _aiBuilder = new AIBuilder(_builderConnectors, _coinPool, _coinDropAnimation, _heroBuilder, gameCyrcle, _enemyPool);

            gameCyrcle.Add(new WorkChecker( _coinDropAnimation,  _coinPool, _heroBuilder, _builderConnectors));
            
            gameCyrcle.AddDayChange(new Trader(gameCyrcle, _coinDropAnimation, _coinPool));
            
            Hacks(_resourceManager);
        }

        private void Hacks(ResourceManager _resourceManager)
        {
            _resourceManager.SetResource(ResourceType.Gold, 40);
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