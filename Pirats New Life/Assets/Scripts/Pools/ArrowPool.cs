using System.Collections.Generic;
using UnityEngine;
using System;
using GameInit.Optimization;
using GameInit.Enemy;
using GameInit.AI;
using GameInit.Projectiles;

public class ArrowPool
{
    private ArrowComponent _prefab;

    private Transform _container;
    private int _minCapacity = 30;
    private int _maxCapacity = Int32.MaxValue;
    private bool _isExpand;

    public List<Arrow> _pool { get; private set; }
    private void OnExpand()
    {
        if (_isExpand)
        {
            _maxCapacity = Int32.MaxValue;
        }
    }

    public ArrowPool(ArrowComponent prefab)
    {
        _prefab = prefab;
        _pool = new List<Arrow>();

        _container = new GameObject("ArrowContainer").transform;

        for (int i = 0; i < _minCapacity; i++)
        {
            CreateObj();
        }
    }
    private Arrow CreateObj(bool isActibebydefault = false)
    {
        var createdObj = MonoBehaviour.Instantiate(_prefab, _container);

        Arrow arrow = new Arrow(createdObj);

        arrow.GetGm().GetGameObj().SetActive(isActibebydefault);

        createdObj.transform.SetParent(_container);

        _pool.Add(arrow);

        return arrow;
    }
    public bool TryGetElement(out Arrow gameobj)
    {
        foreach (var item in _pool)
        {
            if (!item.GetGm().GetGameObj().activeInHierarchy)
            {
                gameobj = item;
                item.GetGm().GetGameObj().SetActive(true);
                var _item = item;
                return true;
            }
        }
        gameobj = null;
        return false;
    }
    public bool TryGetEngagedElement(out List<Arrow> _list)
    {
        _list = new List<Arrow>();
        bool isNotEmpty = false;
        foreach (var item in _pool)
        {
            if (item.GetGm().GetGameObj().activeInHierarchy)
            {
                _list.Add(item);
                isNotEmpty = true;
            }
        }

        return isNotEmpty;
    }
    public bool TryGetEngagedElementSecondTouch(out List<Arrow> _list)
    {
        _list = new List<Arrow>();
        bool isNotEmpty = false;
        foreach (var item in _pool)
        {
            if (item.GetGm().GetGameObj().activeSelf)
            {
                _list.Add(item);
                isNotEmpty = true;
            }
        }

        return isNotEmpty;
    }
    public Arrow GetFreeElements(Vector3 pos, Quaternion rotation)
    {
        var obj = GetFreeElements(pos);
        obj.GetGm().GetTransform().rotation = rotation;
        obj.GetGm().GetGameObj().SetActive(true);
        return obj;
    }
    public Arrow GetFreeElements(Vector3 pos)
    {
        var obj = GetFreeElements();
        obj.GetGm().GetTransform().position = pos;
        obj.GetGm().GetGameObj().SetActive(true);
        return obj;
    }
    public Arrow GetClosestEngagedElements(Vector3 pos)
    {
        var obj = GetEngagedElements();
        Arrow closestObj = null;
        for (int i = 0; i < obj.Count - 1; i++)
        {
            if (Distance.Manhattan(pos, obj[i].GetGm().GetTransform().position) < Distance.Manhattan(pos, obj[i + 1].GetGm().GetTransform().position))
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
    public Arrow GetClosestEngagedElementsSecondTouch(Vector3 pos)
    {
        var obj = GetEngagedElementsSecondTouch();
        Arrow closestObj = null;
        if (obj == null) return null;
        for (int i = 0; i < obj.Count - 1; i++)
        {
            if (Distance.Manhattan(pos, obj[i].GetGm().GetTransform().position) < Distance.Manhattan(pos, obj[i + 1].GetGm().GetTransform().position))
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
    public Arrow GetFreeElements()
    {
        if (TryGetElement(out var gameObj))
        {
            gameObj.Clear();
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
    public List<Arrow> GetEngagedElements()
    {
        if (TryGetEngagedElement(out var gameObj))
        {
            return gameObj;
        }
        return null;
    }
    public List<Arrow> GetEngagedElementsSecondTouch()
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
            if (item.GetGm().GetGameObj().activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}
