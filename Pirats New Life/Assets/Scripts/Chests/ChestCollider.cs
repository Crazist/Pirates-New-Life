using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameInit.Builders;
using GameInit.Pool;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace GameInit.Chest
{
    public class ChestCollider : IUpdate
    {
        private ChestComponent _chestComponent;
        private HeroComponent _heroComponent;
        private ChestBuilder _chestBuilder;
        private ResourceManager _resourceManager;
        private Pools _pool;
        private List<GameObject> coins;
        private List<Vector3> positions;
        private float speedSplash = 5f;
        
        public ChestCollider(ChestComponent chestComponent, ChestBuilder chestBuilder, ResourceManager resourceManager, Pools pool)
        {
            _chestComponent = chestComponent;
            _chestBuilder = chestBuilder;
            _resourceManager = resourceManager;
            _pool = pool;
            coins = new List<GameObject>();
            positions = new List<Vector3>();
        }

        public void OnUpdate()
        {
            if (_chestComponent.IsCollided)
            {
                for (int i = 0; i < _chestComponent.GoldAmount; i++)
                {
                    var x = Random.Range(-2f, 2f);
                    var z = Random.Range(-2f, 2f);
                    positions.Add(new Vector3(x, _chestComponent.transform.position.y, z));
                    coins.Add(_pool.GetFreeElements(_chestComponent.transform.position));
                }
                _chestComponent.GetMono().StartCoroutine(MoveCoins());
                _chestBuilder.RemoveChestCollider(this);
            }
        }
        
        private IEnumerator MoveCoins()
        {
            float distanceThreshold = 0.3f; // adjust this as needed
            while (true)
            {
                int allCoinsOnPlace = 0;
                for (int i = 0; i < coins.Count; i++)
                {
                    if (Vector3.Distance(coins[i].transform.position, positions[i]) < distanceThreshold)
                    {
                        allCoinsOnPlace++;
                    }
                    else
                    {
                        coins[i].transform.position = Vector3.MoveTowards(coins[i].transform.position, positions[i], Time.deltaTime * speedSplash); // adjust speed as needed
                    }
                }

                if(allCoinsOnPlace == coins.Count)
                {
                    break;
                }

                yield return new WaitForFixedUpdate();
            }
            _chestComponent.enabled = false;
            _chestComponent.GetMono().StopCoroutine(MoveCoins());
        }
    }
}