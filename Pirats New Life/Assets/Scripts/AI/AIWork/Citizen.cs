using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GameInit.Pool;
using GameInit.Animation;
using System.Linq;
using GameInit.RandomWalk;
using GameInit.Connector;

namespace GameInit.AI
{
    public class Citizen : IWork, IKDTree
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
        private RandomWalker _RandomWalker;
        private Func<bool> _interruptedAction;
        private ItemsType _item;
        private AIConnector _aIConnector;
        private Vector3 _lastPosition = Vector3.zero;

        private const float _minimalDistanceToHero = 1f;
        private const float _coefDistance = 0.5f;
        private const bool canPickUp = true;
        private const int numberOfCitizenModel = 1;
        private const int radiusRandomWalk = 10;
        private const bool _isEnemy = false;
        private const bool _canDamage = false;
        public bool InMove { get; set; } = false;
        public bool InWork { get; set; } = false;
        public bool GoingForCoin { get; set; } = false;
        public EntityType Type { get; } = EntityType.Ally;
        public int HP { get; set; } = 1;
        public Citizen(AIComponent component, int id, Pools pool, CoinDropAnimation coinDropAnimation, HeroComponent heroComponent, RandomWalker randomWalker, Vector3 mainPosition, AIConnector _connector)
        {
            _aIConnector = _connector;
            _heroComponent = heroComponent;
            _coinDropAnimation = coinDropAnimation;
            _pool = pool;
            _AIComponent = component;
            _id = id;

            _RandomWalker = randomWalker;
            _RandomWalker.Init(_AIComponent.GeNavMeshAgent(), mainPosition, this, radiusRandomWalk);
            _RandomWalker.Move();
            CollectGold();
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

        public void CollectGold()
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
        public bool Move(Vector3 position, Func<bool> action, ItemsType type)
        {
            if (InMove == false)
            {
                _interruptedAction = action;
                _item = type;
                _lastPosition = position;
                InMove = true;
                _AIComponent.GetMonoBehaviour().StartCoroutine(Waiter(action, type));
                _AIComponent.GeNavMeshAgent().destination = position;
                return true;
            }
            return false;
        }
        public bool Move(Vector3 position, Action action)
        {
            if (!InMove)
            {
                InMove = true;
                _AIComponent.GetMonoBehaviour().StartCoroutine(Waiter(action));
                _AIComponent.GeNavMeshAgent().destination = position;
                return true;
            }
            return false;
        }
        private IEnumerator Waiter(Action action)
        {
            GoingForCoin = true;
            yield return new WaitForEndOfFrame();

            var agent = _AIComponent.GeNavMeshAgent();

            while (agent.remainingDistance == 0 || !agent.hasPath)
            {
                yield return null;
            }

            while (agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            InMove = false;
            action?.Invoke();

            GoingForCoin = false;
        }
        private IEnumerator Waiter(Func<bool> action, ItemsType type)
        {
            GoingForCoin = true;

            yield return new WaitForEndOfFrame();

            var agent = _AIComponent.GeNavMeshAgent();

            while (agent.remainingDistance == 0 || !agent.hasPath)
            {
                yield return null;
            }

            while (agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            
            InMove = false;

            bool result = (bool)action?.Invoke();

            GoingForCoin = false;

            if (result) // если result не null, то используем его, иначе значение по умолчанию - false
                CollectGold();

            if (type != ItemsType.None)
            {
                _type = type;
                _RandomWalker.Move();
            }
            _item = ItemsType.None;
            _interruptedAction = null;
            _lastPosition = Vector3.zero;
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
            if (_item != ItemsType.None && _interruptedAction != null && _lastPosition != Vector3.zero)
            {
                _aIConnector.MoveToClosestAICitizen(_lastPosition, _interruptedAction, _item);
                _item = ItemsType.None;
                _interruptedAction = null;
                _lastPosition = Vector3.zero;
            }
            _AIComponent.GetMonoBehaviour().StopAllCoroutines();
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

        public Vector2 GetPositionVector2()
        {
            Vector2 _positionOnVector2;
            _positionOnVector2.x = _AIComponent.GetTransform().position.x;
            _positionOnVector2.y = _AIComponent.GetTransform().position.z;

            return _positionOnVector2;
        }

        public void GetDamage(int damage)
        {
            if (HP - damage <= 0)
            {
                Die();
                HP = 0;
                _coinsCount = 0;
                _hasCoin = false;
            }
            else
            {
                HP = HP - damage;
            }
        }

        private void Die()
        {
           // _AIComponent.GetGm().SetActive(false);
        }
        public bool CheckIfEnemy()
        {
            return _isEnemy;
        }

        public bool CheckIfCanDamage()
        {
            return _canDamage;
        }

        public int CountOFDamage()
        {
            return 0; // can not damage
        }

        public void Attack()
        {
            //can not damage
        }
    }
}

