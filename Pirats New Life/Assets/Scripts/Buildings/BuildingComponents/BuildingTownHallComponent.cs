using Cysharp.Threading.Tasks;
using GameInit.Component;
using GameInit.Connector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Building
{
    public class BuildingTownHallComponent : MonoBehaviour
    {
        [SerializeField] private BuildNeedGoldShow _BuildNeedGoldShow;
        [SerializeField] private Transform _position;
        [SerializeField] private List<GameObject> _forms;
        [SerializeField] private int countOfGold = 2;
        [SerializeField] private float timeToBuild = 2;
        [SerializeField] private Animator _chestAnim;
        [SerializeField] private int _countOfMoneyForDrop = 1;
        [SerializeField] private Transform _chestPos;
        [SerializeField] private GameObject _particlePrefab;
        [SerializeField] private string _name = "TownHall";


        private AIWarConnector _AIWarConnector;
        private int curGold = 0;
        private bool canProduce = true;
        private Action _action;
        private Action _playerInCollider;
        private bool _checkOneTime = true;

        public GameObject GetParticlePrefab()
        {
            return _particlePrefab;
        }
        public int GetCountMoneyPerDay()
        {
            return _countOfMoneyForDrop;
        }
        public Vector3 GetChestPos()
        {
            return _chestPos.position;
        }
        public Animator GetChestAnim()
        {
            return _chestAnim;
        }
       
        public List<GameObject> GetFormList()
        {
            return _forms;
        }
        public float GetTimeForBuild()
        {
            return timeToBuild;
        }
        public Vector3 GetBuildingsPosition()
        {
            return _position.position;
        }
        public int GetCountOfGold()
        {
            return countOfGold;
        }
        public void SetNewCountOfgold(int _gold)
        {
            countOfGold = _gold;
        }
        public void SetCurGold(int gold)
        {
            curGold = gold;
            _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
        }
        public void SetAction(Action action, Action playerInCollider)
        {
            _action = action;
            _playerInCollider = playerInCollider;
        }
        public void SetCanProduce(bool canProd)
        {
            if (!canProd && _BuildNeedGoldShow != null)
            {
                _BuildNeedGoldShow.DeactiveBuildingNeeds();
            }
            canProduce = canProd;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out HeroComponent hero))
            {
                _playerInCollider.Invoke();
                if (canProduce)
                {
                    _BuildNeedGoldShow.ActiveBuildingNeeds();
                    _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
                    _BuildNeedGoldShow.SetNameText(_name);
                }
            }

            if (!canProduce)
            {
                return;
            }

            var _coin = other.gameObject.GetComponent<Coin>();
            if (_coin && !_coin.SecondTouch)
            {
                _coin.Hide();
                _action.Invoke();
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
            _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
            _BuildNeedGoldShow.SetNameText(_name);
        }
        private void OnTriggerExit(Collider other)
        {
            if (!canProduce)
            {
                return;
            }

            if (other.TryGetComponent(out HeroComponent hero))
            {
                _BuildNeedGoldShow.DeactiveBuildingNeeds();
            }
        }
        public void GetWar(AIWarConnector AIWarConnector)
        {
            _AIWarConnector = AIWarConnector;
        }
        public Transform GetTransform()
        {
            return transform;
        }
        private void OnDrawGizmos()
        {
            if (_AIWarConnector != null)
            {
                _AIWarConnector.DrawGiz();
            }
        }
    }
}


