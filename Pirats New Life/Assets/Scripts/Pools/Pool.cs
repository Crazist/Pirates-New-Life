using System.Collections.Generic;
using UnityEngine;
using System;
using GameInit.Optimization;

namespace GameInit.Pool
{
    public class Pools
    {
        [SerializeField] private Coin _prefab;

        private Transform _container;
        private int _minCapacity = 30;
        private int _maxCapacity = Int32.MaxValue;
        private bool _isExpand;

        public List<Coin> _pool { get; private set; }
        private void OnExpand()
        {
            if (_isExpand)
            {
                _maxCapacity = Int32.MaxValue;
            }
        }

        public Pools(Coin prefab)
        {
            _prefab = prefab;
            _pool = new List<Coin>();

            _container = new GameObject("GoldContainer").transform;

            for (int i = 0; i < _minCapacity; i++)
            {
                CreateObj();
            }
        }
        private Coin CreateObj(bool isActibebydefault = false)
        {
            var createdObj = MonoBehaviour.Instantiate(_prefab, _container);
            createdObj.gameObject.SetActive(isActibebydefault);

            _pool.Add(createdObj);

            createdObj.transform.SetParent(_container);

            return createdObj;
        }
        public bool TryGetElement(out Coin gameobj)
        {
            foreach (var item in _pool)
            {
                if (!item.gameObject.activeInHierarchy)
                {
                    gameobj = item;
                    item.gameObject.SetActive(true);
                    return true;
                }
            }
            gameobj = null;
            return false;
        }
        public bool TryGetEngagedElement(out List<Coin> _list)
        {
            _list = new List<Coin>();
            bool isNotEmpty = false;
            foreach (var item in _pool)
            {
                if (item.gameObject.activeInHierarchy)
                {
                    _list.Add(item);
                    isNotEmpty = true;
                }
            }

            return isNotEmpty;
        }
        public bool TryGetEngagedElementSecondTouch(out List<Coin> _list)
        {
            _list = new List<Coin>();
            bool isNotEmpty = false;
            foreach (var item in _pool)
            {
                if (item.gameObject.activeSelf && !item.SecondTouch)
                {
                        _list.Add(item);
                        isNotEmpty = true;
                }
            }

            return isNotEmpty;
        }
        public Coin GetFreeElements(Vector3 pos, Quaternion rotation)
        {
            var obj = GetFreeElements(pos);
            obj.transform.rotation = rotation;
            return obj;
        }
        public Coin GetFreeElements(Vector3 pos)
        {
            var obj = GetFreeElements();
            obj.transform.position = pos;
            return obj;
        }
        public Coin GetClosestEngagedElements(Vector3 pos)
        {
            var obj = GetEngagedElements();
            Coin closestObj = null;
            for (int i = 0; i < obj.Count - 1; i++)
            {
                if (Distance.Manhattan(pos, obj[i].transform.position) < Distance.Manhattan(pos, obj[i + 1].transform.position))
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
        public Coin GetClosestEngagedElementsSecondTouch(Vector3 pos)
        {
            var obj = GetEngagedElementsSecondTouch();
            Coin closestObj = null;
            if (obj == null) return null;
            for (int i = 0; i < obj.Count - 1; i++)
            { 
                if (Distance.Manhattan(pos, obj[i].GetTransform().position) < Distance.Manhattan(pos, obj[i + 1].GetTransform().position))
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
        public Coin GetFreeElements()
        {
            if (TryGetElement(out var gameObj))
            {
                return gameObj;
            }
            if (_isExpand)
            {
                return CreateObj(true);
            }

            if (_pool.Count < _maxCapacity)
            {
                return CreateObj(true);
            }
            throw new Exception("Pool is over!");
        }
        public List<Coin> GetEngagedElements()
        {
            if (TryGetEngagedElement(out var gameObj))
            {
                return gameObj;
            }
            return null;
        }
        public List<Coin> GetEngagedElementsSecondTouch()
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
                if (item.gameObject.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
