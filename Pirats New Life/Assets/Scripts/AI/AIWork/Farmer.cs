using GameInit.AI;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.RandomWalk;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : IWork, IKDTree
{
    private AIComponent _AIComponent;
    private ItemsType _type = ItemsType.Hoe;
    private int _id;
    private bool _hasCoin = false;
    private Pools _pool;
    private int _coinsCount;
    private CoinDropAnimation _coinDropAnimation;
    private HeroComponent _heroComponent;
    private bool _waitCoins = false;
    private RandomWalker _RandomWalker;
    private bool _inWork = false;
    private const bool _isEnemy = false;

    private const float _coefDistance = 0.5f;
    private const bool canPickUp = true;
    private const int numberOfBuilder = 3;
    private const float _minimalDistanceToHero = 1f;
    private const int radiusRandomWalk = 5;
    private const bool _canDamage = false;
    public bool InMove { get; set; } = false;
    public bool InWork { get; set; } = false;
    public bool GoingForCoin { get; set; } = false;
    public int HP { get; set; } = 1;

    public Farmer(AIComponent component, int id, Pools pool, CoinDropAnimation coinDropAnimation, HeroComponent heroComponent, RandomWalker randomWalker, Vector3 mainPosition)
    {
        _heroComponent = heroComponent;
        _coinDropAnimation = coinDropAnimation;
        _pool = pool;
        _AIComponent = component;
        _id = id;
        _RandomWalker = randomWalker;
        _RandomWalker.Init(_AIComponent.GeNavMeshAgent(), mainPosition, this, radiusRandomWalk);
        CollectGold();
        SetStrayModel();
    }

    public void SetStrayModel()
    {
        var modelList = _AIComponent.GetModels();
        for (int i = 0; i < modelList.Count; i++)
        {
            if (i == numberOfBuilder)
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
    public void Move(Vector3 position, Action action, ItemsType type)
    {
        return; //Will not move to position never;
    }
    public void Move(Vector3 position, Action action)
    {
        if (InMove == false)
        {
            GoingForCoin = true;
            InMove = true;
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

        while (agent.velocity != Vector3.zero && _AIComponent.GeNavMeshAgent().remainingDistance > _AIComponent.GeNavMeshAgent().stoppingDistance)
        {
            yield return null;
        }

        action?.Invoke();
        
        GoingForCoin = false;
        InMove = false;
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