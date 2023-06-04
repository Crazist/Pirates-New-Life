using UnityEngine;
using System.Collections.Generic;
using System;
using GameInit.GameCyrcleModule;
using GameInit.Pool;
using GameInit.PoolPrefabs;
using GameInit.AI;
using GameInit.Animation;
using GameInit.TraderLogic;
using GameInit.Hacks;

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
            ArrowPool _arrowRedPool = new ArrowPool(prefabHolder.GetArrowRedPrefab());

            ResourceManager _resourceManager = new ResourceManager();

            UIBuilder _uiBuilder = new UIBuilder(_resourceManager);

            CoinDropAnimation _coinDropAnimation = new CoinDropAnimation();

            BuilderConnectors _builderConnectors = new BuilderConnectors(_coinPool, gameCyrcle, _resourceManager, _arrowPool, _arrowRedPool);
            
            ChestBuilder _chestBuilder = new ChestBuilder(gameCyrcle, _resourceManager, _coinPool, _coinDropAnimation);
            
            HeroBuilder _heroBuilder = new HeroBuilder(gameCyrcle, _coinPool, _resourceManager, _builderConnectors, _builderConnectors, _uiBuilder, _coinDropAnimation, _resourceManager);
            _builderConnectors.GetHeroComponent(_heroBuilder.GetHeroComponent(), _heroBuilder.GetHeroMove());

            BuildingsBuilder _buildingsBuilder = new BuildingsBuilder(gameCyrcle, _resourceManager, _builderConnectors, _heroBuilder.GetHeroComponent(), _coinPool, _coinDropAnimation);

            WorkChecker _workChecker = new WorkChecker(_coinDropAnimation, _coinPool, _heroBuilder, _builderConnectors);

            AIBuilder _aiBuilder = new AIBuilder(_builderConnectors, _coinPool, _coinDropAnimation, _heroBuilder, gameCyrcle, _enemyPool, _workChecker);

            HacksBuilder _hacksBuilder = new HacksBuilder(gameCyrcle, _resourceManager, _aiBuilder, _heroBuilder, _builderConnectors);

            gameCyrcle.Add(_workChecker);

            gameCyrcle.AddDayChange(new Trader(gameCyrcle, _coinDropAnimation, _coinPool));
            
            Hacks(_resourceManager);
        }

        private void Hacks(ResourceManager _resourceManager)
        {
            _resourceManager.SetResource(ResourceType.Gold, 300);
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