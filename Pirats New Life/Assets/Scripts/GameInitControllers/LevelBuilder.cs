using UnityEngine;
using System.Collections.Generic;
using System;
using GameInit.GameCyrcleModule;
using System.Resources;
using GameInit.Pool;
using GameInit.PoolPrefabs;

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