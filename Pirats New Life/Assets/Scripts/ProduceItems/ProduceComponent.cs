using System.Collections;
using System.Collections.Generic;
using GameInit.Building;
using UnityEngine;
using System;
using GameInit.DropAndCollectGold;
using Cysharp.Threading.Tasks;

namespace GameInit.Building
{
    public class ProduceComponent : MonoBehaviour
    {
        [SerializeField] private Transform positionForSpawn;
        [SerializeField] private int cost = 4;
        [SerializeField] private int maxCount = 2;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] ItemsType type;
        [SerializeField] private BuildNeedGoldShow _BuildNeedGoldShow;

        private int curGold = 0;
        private bool canProduce = false;
        private Action _action;
        private bool inBuild = false;
        private Action<int> _dropBeforePickUp;
        private bool _checkOneTime = true;

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

        public void SetAction(Action action, Action<int> DropBeforePickUp)
        {
            _action = action;
            _dropBeforePickUp = DropBeforePickUp;
        }

        public Transform GetPositionForSpawn()
        {
            return positionForSpawn;
        }

        public void SetCanProduce(bool prod)
        {
            if (!prod)
            {
                _BuildNeedGoldShow.DeactiveBuildingNeeds();
            }
            canProduce = prod;
        }

        public ItemsType GetItemType()
        {
            return type;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!canProduce || curGold == cost) return;

            if (other.TryGetComponent(out HeroComponent hero))
            {
                _BuildNeedGoldShow.ActiveBuildingNeeds();
                _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + cost.ToString());
                _BuildNeedGoldShow.SetNameText(type.ToString());
            }

            var _coin = other.gameObject.GetComponent<Coin>();
            if (_coin && !_coin.SecondTouch)
            {
                _coin.Hide();
                GoldCollects();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (!canProduce || curGold == cost) return;

            if (other.TryGetComponent(out HeroComponent hero))
            {
                _BuildNeedGoldShow.DeactiveBuildingNeeds();
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (_checkOneTime && canProduce)
            {
                if (other.TryGetComponent(out HeroComponent hero))
                {
                    WaitBeforeShow();
                    _checkOneTime = false;
                }
            }
        }

        private async void WaitBeforeShow()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            _BuildNeedGoldShow.ActiveBuildingNeeds();
            _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + cost.ToString());
            _BuildNeedGoldShow.SetNameText(type.ToString());
        }
        private void GoldCollects()
        {
            curGold++;
            _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + cost.ToString());
            if (!inBuild)
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
                _dropBeforePickUp.Invoke(curGold);
            }
            curGold = 0;
            checkForGold = false;
            inBuild = false;
            _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + cost.ToString());
        }
    }
}

