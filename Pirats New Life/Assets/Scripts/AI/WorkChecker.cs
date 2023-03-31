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
        private AIConnector _AIConnector;

        private Dictionary<int, ItemsType> _copyList;
        
        public WorkChecker(BuilderConnectors builderConnectors, CoinDropAnimation coinDropAnimation,  Pools pool, HeroBuilder heroBuilder, BuilderConnectors _builderConnectors)
        {
            _AIConnector = _builderConnectors.GetAiConnector();
            _heroComponent = heroBuilder.GetHeroComponent();
            _connector = builderConnectors.GetAiConnector();
            _pool = pool;
            _coinDropAnimation = coinDropAnimation;

            _townHallComponent = Object.FindObjectOfType<TownHallComponent>();

            Init();
        }

        public void Init()
        {
            _copyList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_copyList, _connector.StrayList);
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
            foreach (var list in _connector.ListOfLists)
            {
                foreach (var item in list)
                {
                    if(item.GetId() == stray.GetId())
                    {
                        list.Remove(item);
                        break;
                    }
                }
            }
            ModifCollection(stray);
            return stray;
        }

        private IWork CreateCitizen(IWork work)
        {
            var citizen = new Citizen(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _connector.CitizenList.Add(citizen);
            _connector.StrayList.Remove(work);
            _connector.MoveToClosest();
            ModifCollection(citizen);
            _AIConnector.CheckAndGoToCoin();
            return citizen;
        }
        private IWork CreateBuilder(IWork work)
        {
            var builder = new Builder(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _connector.BuilderList.Add(builder);
            _connector.CitizenList.Remove(work);
            _connector.MoveToClosest();
            ModifCollection(builder);
            _AIConnector.CheckAndGoToCoin();
            return builder;
        }
        private IWork CreateFarmer(IWork work)
        {
            var farmer = new Farmer(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _connector.FarmerList.Add(farmer);
            _connector.CitizenList.Remove(work);
            _connector.MoveToClosest();
            ModifCollection(farmer);
            _AIConnector.CheckAndGoToCoin();
            return farmer;
        }
        private void ModifCollection(IWork citizen)
        {
            foreach (var copy in _copyList)
            {
                if (copy.Key == citizen.GetId())
                {
                    _copyList.Remove(copy.Key);
                    break;
                }
            }
            _copyList.Add(citizen.GetId(), citizen.GetItemType());
        }
        public void OnUpdate()
        {
            foreach (var stray in _connector.StrayList)
            {
                foreach (var _stray in _copyList)
                {
                    if(stray.GetId() == _stray.Key && stray.GetItemType() == ItemsType.None && stray.HasCoin())
                    {
                        stray.RemoveAllEveants();
                        CreateCitizen(stray);
                        return;
                    }
                    if (stray.GetId() == _stray.Key && stray.HasCoin() && stray.GetItemType() != _stray.Value)
                    {
                        stray.RemoveAllEveants();
                        CreateStray(stray);
                        return;
                    }
                }
            }
            foreach (var stray in _connector.CitizenList)
            {
                foreach (var _stray in _copyList)
                {
                    if (stray.GetId() == _stray.Key && stray.HasCoin() && stray.GetItemType() != _stray.Value)
                    {
                        if (stray.GetItemType() == ItemsType.Hammer)
                        {
                            stray.RemoveAllEveants();
                            CreateBuilder(stray);
                            return;
                        }
                        if (stray.GetItemType() == ItemsType.Hoe)
                        {
                            stray.RemoveAllEveants();
                            CreateFarmer(stray);
                            return;
                        }
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

