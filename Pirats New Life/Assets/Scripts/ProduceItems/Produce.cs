using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameInit.Building
{
    public class Produce
    {
        private Action _action;
        private ProduceComponent _produceComponent;
        private int curCount = 0;
        private List<GameObject> items;
        private bool canProduce = false;

        public Produce(ProduceComponent produceComponent)
        {
            _produceComponent = produceComponent;
            _produceComponent.SetCanProduce(canProduce);
            _action += BuidItem;
            produceComponent.SetAction(_action);
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
            
            return empty;
        }
    }
}
