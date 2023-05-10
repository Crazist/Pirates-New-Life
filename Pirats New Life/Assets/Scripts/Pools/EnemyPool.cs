using System.Collections.Generic;
using UnityEngine;
using System;
using GameInit.Optimization;
using GameInit.Enemy;
using GameInit.AI;

public class EnemyPool
{
    private AIComponent _prefab;

    private Transform _container;
    private int _minCapacity = 10;
    private int _maxCapacity = Int32.MaxValue;
    private bool _isExpand;

    public List<IEnemy> _pool { get; private set; }
    private void OnExpand()
    {
        if (_isExpand)
        {
            _maxCapacity = Int32.MaxValue;
        }
    }

    public EnemyPool(AIComponent prefab)
    {
        _prefab = prefab;
        _pool = new List<IEnemy>();

        for (int i = 0; i < _minCapacity; i++)
        {
            CreateObj();
        }
    }
    private IEnemy CreateObj(bool isActibebydefault = false)
    {
        var createdObj = MonoBehaviour.Instantiate(_prefab, _container);
       
        DefaultEnemy _enemy = new DefaultEnemy(createdObj);
        _enemy.GetAiComponent().GetGm().SetActive(isActibebydefault);

        _pool.Add(_enemy);

        return _enemy;
    }
    public bool TryGetElement(out IEnemy gameobj)
    {
        foreach (var item in _pool)
        {
            if (!item.GetAiComponent().GetGm().activeInHierarchy)
            {
                gameobj = item;
                //item.GetAiComponent().GetGm().SetActive(true);
                var _item = (IKDTree)item;
                _item.HP = 1;
                return true;
            }
        }
        gameobj = null;
        return false;
    }
    public bool TryGetEngagedElement(out List<IEnemy> _list)
    {
        _list = new List<IEnemy>();
        bool isNotEmpty = false;
        foreach (var item in _pool)
        {
            if (item.GetAiComponent().GetGm().activeInHierarchy)
            {
                _list.Add(item);
                isNotEmpty = true;
            }
        }

        return isNotEmpty;
    }
    public bool TryGetEngagedElementSecondTouch(out List<IEnemy> _list)
    {
        _list = new List<IEnemy>();
        bool isNotEmpty = false;
        foreach (var item in _pool)
        {
            if (item.GetAiComponent().GetGm().activeSelf)
            {
                _list.Add(item);
                isNotEmpty = true;
            }
        }

        return isNotEmpty;
    }
    public IEnemy GetFreeElements(Vector3 pos, Quaternion rotation)
    {
        var obj = GetFreeElements(pos);
        obj.GetAiComponent().GetTransform().position = pos;
        obj.GetAiComponent().GetTransform().rotation = rotation;
        obj.GetAiComponent().GetGm().SetActive(true);
        return obj;
    }
    public IEnemy GetFreeElements(Vector3 pos)
    {
        var obj = GetFreeElements();
        obj.GetAiComponent().GetTransform().position = pos;
        obj.GetAiComponent().GetGm().SetActive(true);
        return obj;
    }
    public IEnemy GetClosestEngagedElements(Vector3 pos)
    {
        var obj = GetEngagedElements();
        IEnemy closestObj = null;
        for (int i = 0; i < obj.Count - 1; i++)
        {
            if (Distance.Manhattan(pos, obj[i].GetAiComponent().GetTransform().position) < Distance.Manhattan(pos, obj[i + 1].GetAiComponent().GetTransform().position))
            {
                closestObj = obj[i];
            }
            else
            {
                closestObj = obj[i + 1];
            }
        }
        if (obj.Count - 1 == 0 && obj.Count != 0) return obj[0];
        return closestObj;
    }
    public IEnemy GetClosestEngagedElementsSecondTouch(Vector3 pos)
    {
        var obj = GetEngagedElementsSecondTouch();
        IEnemy closestObj = null;
        if (obj == null) return null;
        for (int i = 0; i < obj.Count - 1; i++)
        {
            if (Distance.Manhattan(pos, obj[i].GetAiComponent().GetTransform().position) < Distance.Manhattan(pos, obj[i + 1].GetAiComponent().GetTransform().position))
            {
                closestObj = obj[i];
            }
            else
            {
                closestObj = obj[i + 1];
            }
        }
        if (obj.Count - 1 == 0 && obj.Count != 0) return obj[0];
        return closestObj;
    }
    public IEnemy GetFreeElements()
    {
        if (TryGetElement(out var gameObj))
        {
            return gameObj;
        }
        if (_isExpand)
        {
            return CreateObj();
        }

        if (_pool.Count < _maxCapacity)
        {
            return CreateObj();
        }
        throw new Exception("Pool is over!");
    }
    public List<IEnemy> GetEngagedElements()
    {
        if (TryGetEngagedElement(out var gameObj))
        {
            return gameObj;
        }
        return null;
    }
    public List<IEnemy> GetEngagedElementsSecondTouch()
    {
        if (TryGetEngagedElementSecondTouch(out var gameObj))
        {
            return gameObj;
        }
        return null;
    }
    public bool CheckForActiveItems()
    {
        foreach (var item in _pool)
        {
            if (item.GetAiComponent().GetGm().activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}
