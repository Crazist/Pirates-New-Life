using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GameInit.Pool;
using GameInit.Animation;
using System.Linq;

namespace GameInit.AI
{
    public class Stray : IWork, IUpdate
    {
        private AIComponent _AIComponent;
        private ItemsType _type = ItemsType.None;
        private int _id;
        private bool _hasCoin = false;
        private Pools _pool;
        private int _coinsCount;
        private CoinDropAnimation _coinDropAnimation;
        private HeroComponent _heroComponent;
        private bool _waitToDropCoins = false;
        private List<Vector3> _nextMove;
        private List<Action> _nextAction;
        private bool _inMove = false;

        private const float _minimalDistanceToHero = 1f;
        private const float _coefDistance = 0.5f;
        private const bool canPickUp = true;

        public Stray(AIComponent component, int id, Pools pool, CoinDropAnimation coinDropAnimation, HeroComponent heroComponent)
        {
            _heroComponent = heroComponent;
            _coinDropAnimation = coinDropAnimation;
            _pool = pool;
            _AIComponent = component;
            _id = id;

            _nextMove = new List<Vector3>();
            _nextAction = new List<Action>();
        }

        public AIComponent GetComponent()
        {
            return _AIComponent;
        }

        public int GetId()
        {
            return _id;
        }

        public ItemsType GetItemType()
        {
            return _type;
        }

        public Transform getTransform()
        {
            return _AIComponent.GetTransform();
        }

        public bool HasCoin()
        {
            return _hasCoin;
        }

        private void CollectGold()
        {
            _hasCoin = true;
            _coinsCount++;
        }
        private void DropGold()
        {
            if(_coinsCount > 1)
            {
                _coinDropAnimation.RandomCoinJump(_AIComponent.GetTransform().localPosition, _coinsCount - 1, _AIComponent.GetTransform().position, _pool, canPickUp);
                _coinsCount = 1;
            }
        }
        public void Move(Vector3 position, Action action, ItemsType type)
        {
            if (_inMove == false)
            {
                _AIComponent.GeNavMeshAgent().SetDestination(position);
                _AIComponent.GetMonoBehaviour().StartCoroutine(Waiter(action, type));
            }
        }
        public void Move(Vector3 position, Action action)
        {
            if(_inMove == false)
            {
                _inMove = true;
                _AIComponent.GeNavMeshAgent().destination = position;
                _AIComponent.GetMonoBehaviour().StartCoroutine(Waiter(action));
            }
        }
        private IEnumerator Waiter(Action action)
        {
            var agent = _AIComponent.GeNavMeshAgent();

            while (agent.velocity == Vector3.zero)
            {
                yield return null;
            }

            while (_AIComponent.GeNavMeshAgent().remainingDistance > _AIComponent.GeNavMeshAgent().stoppingDistance)
            {
                yield return null;
            }

            action.Invoke();
            CollectGold();
            yield return new WaitForSeconds(0.5f);
            _inMove = false;
        }
        private IEnumerator Waiter(Action action, ItemsType type)
        {
            var agent = _AIComponent.GeNavMeshAgent();

            while (agent.velocity == Vector3.zero)
            {
                yield return null;
            }
            while (_AIComponent.GeNavMeshAgent().remainingDistance > _AIComponent.GeNavMeshAgent().stoppingDistance)
            {
                yield return null;
            }

            action.Invoke();
            _type = type;
        }
        private IEnumerator Waiter()
        {
            _waitToDropCoins = true; 
            yield return new WaitForSecondsRealtime(3);
            var i = Vector3.Distance(_heroComponent.Transform.position, _AIComponent.GetTransform().position);
            if (Vector3.Distance(_heroComponent.Transform.position, _AIComponent.GetTransform().position) < _minimalDistanceToHero && _AIComponent.GeNavMeshAgent().remainingDistance < _coefDistance)
            {
                DropGold();
            }
            _waitToDropCoins = false;
        }
        public void RemoveAllEveants()
        {
            return;
        }

        public void OnUpdate()
        {
            if (!_waitToDropCoins && Vector3.Distance(_heroComponent.Transform.position, _AIComponent.GetTransform().position) < _minimalDistanceToHero)
            {
                _AIComponent.StartCoroutine(Waiter());
            }
        }
    }
}

