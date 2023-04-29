using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameInit.Connector;
using GameInit.Animation;
using GameInit.Pool;

namespace GameInit.Building
{
    public class Produce
    {
        private Action _action;
        private ProduceComponent _produceComponent;
        private int curCount = 0;
        private List<GameObject> items;
        private bool canProduce = false;
        private Action<int> DropBeforePickUp;
        private AIConnector _AIConnector;
        private CoinDropAnimation _coinDropAnimation;
        private Pools _coinPool;
        private BuildingComponent _buildingComponent;

        private const bool canPickUp = false;
        public Produce(ProduceComponent produceComponent, AIConnector AIConnector, Pools coinPool, CoinDropAnimation coinDropAnimation, BuildingComponent buildingComponent)
        {
            _buildingComponent = buildingComponent;
            _coinDropAnimation = coinDropAnimation;
            _coinPool = coinPool;
            _AIConnector = AIConnector;
            _produceComponent = produceComponent;
            items = new List<GameObject>();
            _produceComponent.SetCanProduce(canProduce);
            DropBeforePickUp += DropBeforePickUpCoin;
            _action += BuidItem;
            produceComponent.SetAction(_action, DropBeforePickUp);
        }
        private void DropBeforePickUpCoin(int count)
        {
            _coinDropAnimation.RandomCoinJump(_buildingComponent.GetBuildPositions()[0].position, count, _buildingComponent.GetBuildPositions()[0].position, _coinPool, canPickUp);
        }

        private void BuidItem()
        {
            _produceComponent.SetCanProduce(CheckCountOfItems());
            SpawnItem(_produceComponent.GetItemPrefab(), _produceComponent.GetPositionForSpawn().position);
        }

        private void SpawnItem(GameObject prefab, Vector3 position)
        {
            for (int i = 0; i < items.Count; i++)
            {
                position = new Vector3(position.x, position.y, position.z + 1);
            }
            GameObject instance = GameObject.Instantiate(prefab, position, Quaternion.identity);
            items.Add(instance);
            _AIConnector.MoveToClosestAICitizen(position, () =>
            {
                items.Remove(instance);
                GameObject.Destroy(instance);
                _produceComponent.SetCanProduce(CheckCountOfItems());
                // Add any additional code you want to execute here
            }, _produceComponent.GetItemType());
        }

        public void CanProduce()
        {
            canProduce = true;
            _produceComponent.SetCanProduce(canProduce);
        }

        private bool CheckCountOfItems()
        {
            bool empty = false;
            foreach (var item in items)
            {
                if(item == null)
                {
                    items.Remove(item);
                    empty = true;
                }
            }
            if(items.Count == 0)
            {
                empty = true;
            }
            return empty;
        }
    }
}

