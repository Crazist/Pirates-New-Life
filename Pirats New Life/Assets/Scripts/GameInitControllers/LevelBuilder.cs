using UnityEngine;
using System.Collections.Generic;
using System;
using GameInit.GameCyrcleModule;
using System.Resources;
using GameInit.Pool;
using GameInit.PoolPrefabs;
using GameInit.Builders;

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

            ChestBuilder _chestBuilder = new ChestBuilder(gameCyrcle, _resourceManager, _coinPool);
            
            HeroBuilder _heroBuilder = new HeroBuilder(gameCyrcle, _coinPool, _resourceManager);

            UIBuilder _uiBuilder = new UIBuilder(_resourceManager);

            BuildingsBuilder _buildingsBuilder = new BuildingsBuilder(gameCyrcle, _resourceManager);

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