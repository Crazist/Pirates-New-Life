using System.Collections;
using System.Collections.Generic;
using GameInit.Building;
using UnityEngine;
using System;
using GameInit.DropAndCollectGold;

namespace GameInit.Building
{
    public class ProduceComponent : MonoBehaviour
    {
        [SerializeField] private Transform positionForSpawn;
        [SerializeField] private int cost = 4;
        [SerializeField] private int maxCount = 2;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] ItemsType type;

        private int curGold = 0;
        private bool canProduce = false;
        private List<Coin> curCoinsList;
        private Action _action;
        private bool inBuild = false;

        public int GetMaxCount()
        {
            return maxCount;
        }

        public MonoBehaviour GetMonoBehavior()
        {
            return this;
        }
        public GameObject GetItemPrefab()
        {
            return itemPrefab;
        }

        private void Start()
        {
            curCoinsList = new List<Coin>();
        }

        public void SetAction(Action action)
        {
            _action = action;
        }

        public Transform GetPositionForSpawn()
        {
            return positionForSpawn;
        }

        public void SetCanProduce(bool prod)
        {
            canProduce = prod;
        }

        public ItemsType GetItemType()
        {
            return type;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!canProduce || curGold == cost) return;

            var _coin = other.gameObject.GetComponent<Coin>();
            if (_coin && !_coin.SecondTouch)
            {
                curCoinsList.Add(_coin);
                _coin.Hide();
                GoldCollects();
            }
        }

        private void GoldCollects()
        {
            curGold++;
            if(!inBuild)
            StartCoroutine(Waiter());
        }

        private IEnumerator Waiter()
        {
            bool checkForGold = true;
            inBuild = true;
            int gold = 0;
            do
            {
                if (gold < curGold)
                {
                    gold++;
                    yield return new WaitForSecondsRealtime(1);
                }
                else
                {
                    checkForGold = false;
                }
            }
            while (checkForGold);
            if (curGold == cost)
            {
                _action.Invoke();
            }
            else
            {
                foreach (var coin in curCoinsList)
                {
                    coin.Active();
                    coin.SecondTouch = true;
                }
            }
            curCoinsList.Clear();
            curGold = 0;
            checkForGold = false;
            inBuild = false;
            StopCoroutine(Waiter());
        }
    }
}

