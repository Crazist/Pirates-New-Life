using UnityEngine.AI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using GameInit.Optimization;

namespace GameInit.TraderLogic
{
    public class TraderComponent : MonoBehaviour
    {
        [SerializeField] private int _goldCount;
        [SerializeField] private int _dayToWait;
        [SerializeField] private float _modificator = 3;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _minDistanceToStartCollect = 2;

        public float Modificator { get { return _modificator; } }
        public int GoldCount { get { return _goldCount; } }
        public Transform StartPoint { get { return _startPoint; } }
        public Transform EndPoint { get { return _endPoint; } }
        public NavMeshAgent Agent { get { return _agent; } }
        public int DayToWait { get { return _dayToWait; } }

        private Action _action;
        private Action _drop;
        private Action<int> _dropBeforePickUp;
        private bool _checkForGold = false;
        private int _curGold;
        private List<Coin> _curCoinsList;
        private bool _canPick = true;
        public void SetAction(Action _collectGold, Action drop, Action<int> DropBeforePickUp)
        {
            _curCoinsList = new List<Coin>();
            _dropBeforePickUp = DropBeforePickUp;
            _drop = drop;
            _action = _collectGold;
        }
        public void CanPick(bool canPick)
        {
            _canPick = canPick;
        }
        private void OnTriggerEnter(Collider other)
        {
            var _coin = other.gameObject.GetComponent<Coin>();
            if (_action != null && _canPick && _curGold < _goldCount && Distance.Manhattan(transform.position, EndPoint.position) < _minDistanceToStartCollect && _coin && !_coin.SecondTouch)
            {
                _curCoinsList.Add(_coin);
                _coin.Hide();
                GoldCollects();
            }
            else if (other.gameObject.GetComponent<HeroComponent>() && Distance.Manhattan(transform.position, EndPoint.position) < _minDistanceToStartCollect)
             {
                _drop.Invoke();
            }
        }

        private void GoldCollects()
        {
            _curGold++;
            if (!_checkForGold)
                StartCoroutine(GoldCollectorWaiter());
        }
        private IEnumerator GoldCollectorWaiter()
         {
            _checkForGold = true;
            int gold = 0;
            do
            {
                if (gold < _curGold)
                {
                    gold++;
                    yield return new WaitForSecondsRealtime(1);
                }
                else
                {
                    _checkForGold = false;
                }
            }
            while (_checkForGold);

            if (_curGold >= _goldCount)
            {
                CanPick(false);
                _action.Invoke();
            }
            else
            {
                _dropBeforePickUp.Invoke(_curGold);
            }
            _curGold = 0;
        }
    }
}
