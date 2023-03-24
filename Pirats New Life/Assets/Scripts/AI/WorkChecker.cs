using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Connector;
using GameInit.Pool;
using GameInit.Animation;
using GameInit.GameCyrcleModule;
using GameInit.Builders;
using GameInit.MainPositions;

namespace GameInit.AI
{
    public class WorkChecker : IUpdate
    {
        private AIConnector _connector;
        private CoinDropAnimation _coinDropAnimation;
        private Pools _pool;
        private HeroComponent _heroComponent;
        private TownHallComponent _townHallComponent;

        private Dictionary<int, ItemsType> _strayList;
        private Dictionary<int, ItemsType> _citizenList;
        private Dictionary<int, ItemsType> _builderList;
        private Dictionary<int, ItemsType> _archerList;
        private Dictionary<int, ItemsType> _farmerList;
        private Dictionary<int, ItemsType> _swordManList;
        public WorkChecker(AIConnector connector, CoinDropAnimation coinDropAnimation,  Pools pool, HeroBuilder heroBuilder)
        {
            _heroComponent = heroBuilder.GetHeroComponent();
            _connector = connector;
            _pool = pool;
            _coinDropAnimation = coinDropAnimation;

            _townHallComponent = Object.FindObjectOfType<TownHallComponent>();

            Init();
        }

        public void Init()
        {
            _strayList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_strayList, _connector.StrayList);

            _citizenList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_citizenList, _connector.StrayList);

            _builderList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_builderList, _connector.StrayList);

            _archerList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_archerList, _connector.StrayList);

            _farmerList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_farmerList, _connector.StrayList);

            _swordManList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_swordManList, _connector.StrayList);
        }

        private Dictionary<int, ItemsType> CopyToDictinary(Dictionary<int, ItemsType> dictinary, List<IWork> list)
        {
            foreach (var item in list)
            {
                dictinary.Add(item.GetId(), item.GetItemType());
            }

            return dictinary;
        }

        
        private IWork CreateStray(IWork work)
        {
            var stray = new Stray(work.GetAiComponent(), work.GetId(), _pool,  _coinDropAnimation, _heroComponent, work.GetRandomWalker());
            _connector.StrayList.Add(stray);
            _connector.StrayList.Remove(work);
            return stray;
        }

        private IWork CreateCitizen(IWork work)
        {
            var citizen = new Citizen(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _connector.CitizenList.Add(citizen);
            _connector.StrayList.Remove(work);
            _connector.MoveToClosestAICitizen();
            return citizen;
        }

        public void OnUpdate()
        {
            foreach (var stray in _connector.StrayList)
            {
                foreach (var _stray in _strayList)
                {
                    if(stray.GetId() == _stray.Key && stray.GetItemType() == ItemsType.None && stray.HasCoin())
                    {
                        stray.RemoveAllEveants();
                        CreateCitizen(stray);
                        return;
                    }
                    if (stray.GetId() == _stray.Key && !stray.HasCoin() && stray.GetItemType() != _stray.Value)
                    {
                        stray.RemoveAllEveants();
                        CreateStray(stray);
                        return;
                    }
                }

            }
        }
    }
}

