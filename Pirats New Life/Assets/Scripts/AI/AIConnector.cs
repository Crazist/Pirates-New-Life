using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.AI;
using System;
using GameInit.Pool;

namespace GameInit.Connector
{
    public class AIConnector : IUpdate
    {
        public List<IWork> StrayList { get; set; }
        public List<IWork> BuilderList { get; set; }
        public List<IWork> ArcherList { get; set; }
        public List<IWork> FarmerList { get; set; }
        public List<IWork> SwordManList { get; set; }

        private Pools _pool;

        private const int _minDistance = 5;
        public AIConnector(Pools pool)
        {
            StrayList = new List<IWork>();
            BuilderList = new List<IWork>();
            ArcherList = new List<IWork>();
            FarmerList = new List<IWork>();
            SwordManList = new List<IWork>();

            _pool = pool;
        }

        public void MoveToClosestAI(Vector3 targetPosition, Action callback, ItemsType type)
        {
            float minDistance = Mathf.Infinity;
            Vector3 closestPosition = Vector3.zero;
            IWork _stray = null;

            foreach (var stray in StrayList)
            {
                float distance = Vector3.Distance(stray.getTransform().position, targetPosition);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPosition = stray.getTransform().position;
                    _stray = stray;
                }
            }

            _stray.Move(targetPosition, callback, type);
        }
        public void CheckAndGoToCoin()
        {
            float minDistance = Mathf.Infinity;
            Vector3 closestPosition = Vector3.zero;
            IWork _stray = null;
            Coin _coin =  null;

            if (_pool.CheckForActiveItems())
            {
                foreach (var stray in StrayList)
                {
                    Coin coin = _pool.GetClosestEngagedElementsSecondTouch(stray.getTransform().position);
                    
                    if(coin == null)
                    {
                        return;
                    }
                    float distance = Vector3.Distance(stray.getTransform().position, coin.GetTransform().position);

                    if (distance < minDistance)
                    {
                        _coin = coin;
                        minDistance = distance;
                        closestPosition = stray.getTransform().position;
                        _stray = stray;
                    }
                }
                if (Vector3.Distance(_stray.getTransform().position, _pool.GetClosestEngagedElements(_stray.getTransform().position).transform.position) < _minDistance)
                {
                    _stray.Move(_coin.transform.position, () => { _coin.Hide(); });
                }
            }
        }
        public int GenerateId()
        {
            bool sameId = false;
            int id = 0;

            do
            {
                sameId = false;

                id = 0;

                id = UnityEngine.Random.Range(0, 1000);

                List<List<IWork>> allList = new List<List<IWork>>() { StrayList, BuilderList, ArcherList, FarmerList, SwordManList };

                sameId = CycleList(allList, id);
            } 
            while (sameId);
          
            return id;
        }

        private bool CycleList(List<List<IWork>> allList, int id)
        {
            foreach (var list in allList)
            {
                foreach (var item in list)
                {
                    if (item.GetId() == id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void OnUpdate()
        {
            CheckAndGoToCoin();
        }
    }
}

