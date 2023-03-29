using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.AI;
using System;
using GameInit.Pool;
using GameInit.RandomWalk;
using GameInit.Optimization;
using GameInit.GameCyrcleModule;
using GameInit.Building;

namespace GameInit.Connector
{
    public class AIConnector : IUpdate, IDayChange
    {
        public List<IWork> StrayList { get; set; }
        public List<IWork> CitizenList { get; set; }
        public List<IWork> BuilderList { get; set; }
        public List<IWork> ArcherList { get; set; }
        public List<IWork> FarmerList { get; set; }
        public List<IWork> SwordManList { get; set; }

        public List<List<IWork>> ListOfLists { get; set; }

        private Pools _pool;
        private HeroComponent _heroComponent;
        private List<Action> lateMove;
        private GameCyrcle _gameCyrcle;
        private List<IWork> stillInMove;

        private const int _minDistance = 5;
        private const float _minimalDistanceToHero = 1f;

        public AIConnector(Pools pool, GameCyrcle cyrcle)
        {
            stillInMove = new List<IWork>();
            ListOfLists = new List<List<IWork>>();
            lateMove = new List<Action>();

            ListOfLists.Add(StrayList = new List<IWork>());
            ListOfLists.Add(CitizenList = new List<IWork>());
            ListOfLists.Add(BuilderList = new List<IWork>());
            ListOfLists.Add(ArcherList = new List<IWork>());
            ListOfLists.Add(FarmerList = new List<IWork>());
            ListOfLists.Add(SwordManList = new List<IWork>());

            _gameCyrcle = cyrcle;
            _pool = pool;
        }
        
        public void GetHeroComponent(HeroComponent heroComponent)
        {
            _heroComponent = heroComponent;
        }
        public void MoveToClosestAIStray(Vector3 targetPosition, Action callback)
        {
            float minDistance = Mathf.Infinity;
            Vector3 closestPosition = Vector3.zero;
            IWork _stray = null;

            foreach (var stray in StrayList)
            {
                float distance = Distance.Manhattan(stray.getTransform().position, targetPosition);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPosition = stray.getTransform().position;
                    _stray = stray;
                }
            }

            _stray.Move(targetPosition, callback);
        }

        public void MoveToClosest()
        {
            for (int i = 0; i < lateMove.Count; i++)
            {
                lateMove[i].Invoke();
                lateMove.Remove(lateMove[i]);
            }
        }
        public void MoveToClosestAICitizen(Vector3 targetPosition, Action callback, ItemsType type)
        {
            float minDistance = Mathf.Infinity;
            Vector3 closestPosition = Vector3.zero;
            IWork _stray = null;

            foreach (var stray in CitizenList)
            {
                if (!stray.InMove)
                {
                    float distance = Distance.Manhattan(stray.getTransform().position, targetPosition);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPosition = stray.getTransform().position;
                        _stray = stray;
                    }
                }
            }

            if(_stray != null)
            {
                _stray.Move(targetPosition, callback, type);
            }
            else
            {
                lateMove.Add(() =>
                {
                    MoveToClosestAICitizen(targetPosition, callback, type);
                });
            }
        }
        public IWork MoveToClosestAIBuilder(Vector3 targetPosition, Action callback, IBuilding building)
        {
            float minDistance = Mathf.Infinity;
            Vector3 closestPosition = Vector3.zero;
            IWork _stray = null;

            foreach (var builder in BuilderList)
            {
                if (builder.InWork != true)
                {
                    float distance = Distance.Manhattan(builder.getTransform().position, targetPosition);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPosition = builder.getTransform().position;
                        _stray = builder;
                    }
                }
            }

            if (_stray != null && _gameCyrcle.ChekIfDay())
            {
                _stray.Move(targetPosition, callback);
            }
            else
            {
                lateMove.Add(() =>
                {
                    building.SetBuilder(_stray);
                    MoveToClosestAIBuilder(targetPosition, callback, building);
                });
            }
            return _stray;
        }
       
        public void CheckIfPlayerWaitForCoinsMain()
        {
            if (_heroComponent == null) return;

            foreach (var listOfWorks in ListOfLists)
            {
                foreach (var stray in listOfWorks)
                {
                    var Ai = stray.GetAiComponent();
                    if (Distance.Manhattan(_heroComponent.Transform.position, Ai.GetTransform().position) < _minimalDistanceToHero)
                    {
                        stray.CheckIfPlayerWaitForCoins();
                    }
                }
            }
        }
        public void CheckAndGoToCoin()
        {
            float minDistance = Mathf.Infinity;
            Vector3 closestPosition = Vector3.zero;
            IWork _stray = null;
            Coin _coin = null;
            
            foreach (var listOfWorks in ListOfLists)
            {
                foreach (var stray in listOfWorks)
                {
                    if (!stray.InMove || stray.GoingForCoin)
                    {

                        Coin coin = _pool.GetClosestEngagedElementsSecondTouch(stray.getTransform().position);

                        if (coin == null)
                        {
                            break;
                        }

                        float distance = Distance.Manhattan(stray.getTransform().position, coin.GetTransform().position);

                        if (distance < minDistance)
                        {
                            _coin = coin;
                            minDistance = distance;
                            closestPosition = stray.getTransform().position;
                            _stray = stray;
                        }
                    }
                }
            }
                if (_stray != null && Distance.Manhattan(_stray.getTransform().position, _coin.GetTransform().position) < _minDistance)
                {
                    _stray.Move(_coin.transform.position, () => { _coin.Hide(); CheckAndGoToCoin(); });
                    _stray = null;
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

        private void NightReturnAll()
        {
            List<IWork> stillInMove = new List<IWork>();


            if (!_gameCyrcle.ChekIfDay())
            {
                foreach (var listOfWorks in ListOfLists)
                {
                    foreach (var stray in listOfWorks)
                    {
                        stray.InWork = _gameCyrcle.ChekIfDay();
                        if (!stray.InMove)
                        {
                            stray.GetRandomWalker().Move();
                        }
                        else
                        {
                            stillInMove.Add(stray);
                        }
                    }
                }
            }

            _heroComponent.GetMono().StartCoroutine(WaitWhyleAllFinishMove());
        }

        public void OnUpdate()
        {
            CheckIfPlayerWaitForCoinsMain();
        }

        public void OnDayChange()
        {
            NightReturnAll();
        }

        private IEnumerator WaitWhyleAllFinishMove()
        {
            bool waitMove = true;
            do
            {
                waitMove = false;
                yield return new WaitForEndOfFrame();
                for (int i = 0; i < stillInMove.Count; i++)
                {
                    if(stillInMove[i].InMove == false)
                    {
                        stillInMove[i].GetRandomWalker().Move();
                    }
                    else
                    {
                        waitMove = true;
                    }
                }

            } while (waitMove);
        }
    }
}
