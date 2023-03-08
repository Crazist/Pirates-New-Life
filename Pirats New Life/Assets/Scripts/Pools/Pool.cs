using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameInit.Pool
{
    public class Pools
    {
        [SerializeField] private GameObject _prefab;

        private Transform _container;
        private int _minCapacity = 10;
        private int _maxCapacity = Int32.MaxValue;
        private bool _isExpand;

        public List<GameObject> _pool { get; private set; }
        private void OnExpand()
        {
            if (_isExpand)
            {
                _maxCapacity = Int32.MaxValue;
            }
        }

        public Pools(GameObject prefab)
        {
            _prefab = prefab;
            _pool = new List<GameObject>();

            for (int i = 0; i < _minCapacity; i++)
            {
                CreateObj();
            }
        }
        private GameObject CreateObj(bool isActibebydefault = false)
        {
            var createdObj = MonoBehaviour.Instantiate(_prefab, _container);
            createdObj.gameObject.SetActive(isActibebydefault);

            _pool.Add(createdObj);

            return createdObj;
        }
        public bool TryGetElement(out GameObject gameobj)
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
        public bool TryGetEngagedElement(out List<GameObject> _list)
        {
            _list = new List<GameObject>();
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
        public GameObject GetFreeElements(Vector3 pos, Quaternion rotation)
        {
            var obj = GetFreeElements(pos);
            obj.transform.rotation = rotation;
            return obj;
        }
        public GameObject GetFreeElements(Vector3 pos)
        {
            var obj = GetFreeElements();
            obj.transform.position = pos;
            return obj;
        }
        public GameObject GetClosestEngagedElements(Vector3 pos)
        {
            var obj = GetEngagedElements();
            GameObject closestObj = null;
            for (int i = 0; i < obj.Count - 1; i++)
            {
                if (Vector3.Distance(pos, obj[i].transform.position) < Vector3.Distance(pos, obj[i + 1].transform.position))
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
        public GameObject GetFreeElements()
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
        public List<GameObject> GetEngagedElements()
        {
            if (TryGetEngagedElement(out var gameObj))
            {
                return gameObj;
            }
            return null;
        }
    }
}
