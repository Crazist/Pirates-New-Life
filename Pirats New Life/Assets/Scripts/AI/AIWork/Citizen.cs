using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GameInit.Pool;
using GameInit.Animation;
using System.Linq;
using GameInit.RandomWalk;

namespace GameInit.AI
{
    public class Citizen : IWork
    {

        private AIComponent _AIComponent;
        private ItemsType _type = ItemsType.None;
        private int _id;
        private bool _hasCoin = false;
        private Pools _pool;
        private int _coinsCount;
        private CoinDropAnimation _coinDropAnimation;
        private HeroComponent _heroComponent;
        private bool _waitCoins = false;
        private List<Vector3> _nextMove;
        private List<Action> _nextAction;
        private RandomWalker _RandomWalker;
        
        private const float _minimalDistanceToHero = 1f;
        private const float _coefDistance = 0.5f;
        private const bool canPickUp = true;
        private const int numberOfCitizenModel = 1;
        private const int radiusRandomWalk = 10;
        public bool InMove { get; set; } = false;

        public Citizen(AIComponent component, int id, Pools pool, CoinDropAnimation coinDropAnimation, HeroComponent heroComponent, RandomWalker randomWalker)
        {
            _heroComponent = heroComponent;
            _coinDropAnimation = coinDropAnimation;
            _pool = pool;
            _AIComponent = component;
            _id = id;

            _nextMove = new List<Vector3>();
            _nextAction = new List<Action>();

            _RandomWalker = randomWalker;
            _RandomWalker.Init(_AIComponent.GeNavMeshAgent(), _AIComponent.GetTransform().position, this, radiusRandomWalk); 
            SetStrayModel();
        }

        public void SetStrayModel()
        {
            var modelList = _AIComponent.GetModels();
            for (int i = 0; i < modelList.Count; i++)
            {
                if (i == numberOfCitizenModel)
                {
                    modelList[i].SetActive(true);
                }
                else
                {
                    modelList[i].SetActive(false);
                }
            }
        }

        public AIComponent GetAiComponent()
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
            if (_coinsCount > 1)
            {
                _coinDropAnimation.RandomCoinJump(_AIComponent.GetTransform().localPosition, _coinsCount - 1, _AIComponent.GetTransform().position, _pool, canPickUp);
                _coinsCount = 1;
            }
        }
        public void Move(Vector3 position, Action action, ItemsType type)
        {
            if (InMove == false)
            {
                _AIComponent.GeNavMeshAgent().SetDestination(position);
                _AIComponent.GetMonoBehaviour().StartCoroutine(Waiter(action, type));
            }
        }
        public void Move(Vector3 position, Action action)
        {
            if (InMove == false)
            {
                InMove = true;
                _AIComponent.GetMonoBehaviour().StartCoroutine(Waiter(action));
                _AIComponent.GeNavMeshAgent().destination = position;
                _AIComponent.GetMonoBehaviour().StartCoroutine(Waiter(action));
            }
        }
        private IEnumerator Waiter(Action action)
        {
            var agent = _AIComponent.GeNavMeshAgent();

            while (agent.velocity == Vector3.zero)
            {
                yield return new WaitForEndOfFrame();
            }

            while (_AIComponent.GeNavMeshAgent().remainingDistance > _AIComponent.GeNavMeshAgent().stoppingDistance)
            {
                yield return null;
            }

            action.Invoke();
            CollectGold();
            InMove = false;
            _AIComponent.GetMonoBehaviour().StartCoroutine(StartRandomWallk());
        }
        private IEnumerator Waiter(Action action, ItemsType type)
        {
            InMove = true;
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
            InMove = false;
            _AIComponent.GetMonoBehaviour().StartCoroutine(StartRandomWallk());
        }
        private IEnumerator Waiter()
        {
            yield return new WaitForSecondsRealtime(3);
            var i = Vector3.Distance(_heroComponent.Transform.position, _AIComponent.GetTransform().position);
            if (Vector3.Distance(_heroComponent.Transform.position, _AIComponent.GetTransform().position) < _minimalDistanceToHero && _AIComponent.GeNavMeshAgent().remainingDistance < _coefDistance)
            {
                DropGold();
            }
            _waitCoins = false;
        }
        public void RemoveAllEveants()
        {
            _AIComponent.GetMonoBehaviour().StopAllCoroutines();
        }

        private IEnumerator StartRandomWallk()
        {
            yield return new WaitForSecondsRealtime(5);
            if (!InMove)
            {
                _RandomWalker.Move();
            }
        }

        public void CheckIfPlayerWaitForCoins()
        {
            if (_waitCoins == false)
            {
                _waitCoins = true;
                _AIComponent.StartCoroutine(Waiter());
            }
        }
        public RandomWalker GetRandomWalker()
        {
            return _RandomWalker;
        }
    }
}

