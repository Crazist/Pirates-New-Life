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
        private AIWorkConnector _AIWorkConnector;
        private CoinDropAnimation _coinDropAnimation;
        private Pools _pool;
        private HeroComponent _heroComponent;
        private TownHallComponent _townHallComponent;
        private AIConnector _AIConnector;

        private Dictionary<int, ItemsType> _copyList;
        
        public WorkChecker(BuilderConnectors builderConnectors, CoinDropAnimation coinDropAnimation,  Pools pool, HeroBuilder heroBuilder, BuilderConnectors _builderConnectors)
        {
            _AIConnector = _builderConnectors.GetAiConnector();
            _AIWorkConnector = _builderConnectors.GetAIWorkConnector();
            _heroComponent = heroBuilder.GetHeroComponent();
            _pool = pool;
            _coinDropAnimation = coinDropAnimation;

            _townHallComponent = Object.FindObjectOfType<TownHallComponent>();

            Init();
        }

        public void Init()
        {
            _copyList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_copyList, _AIConnector.StrayList);
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
            _AIConnector.StrayList.Add(stray);
            _AIConnector.StrayList.Remove(work);
            foreach (var list in _AIConnector.ListOfLists)
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
            _AIConnector.CitizenList.Add(citizen);
            _AIConnector.StrayList.Remove(work);
            _AIConnector.MoveToClosest();
            ModifCollection(citizen);
            _AIConnector.CheckAndGoToCoin();
            return citizen;
        }
        private IWork CreateBuilder(IWork work)
        {
            var builder = new Builder(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _AIConnector.BuilderList.Add(builder);
            _AIConnector.CitizenList.Remove(work);
            _AIConnector.MoveToClosest();
            ModifCollection(builder);
            _AIConnector.CheckAndGoToCoin();
            return builder;
        }
        private IWork CreateFarmer(IWork work)
        {
            var farmer = new Farmer(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _AIConnector.FarmerList.Add(farmer);
            _AIConnector.CitizenList.Remove(work);
            _AIConnector.MoveToClosest();
            ModifCollection(farmer);
            _AIConnector.CheckAndGoToCoin();
            return farmer;
        }
        private void CreateSwordMan(IWork work)
        {
            var swordMan = new SwordMan(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _AIWorkConnector.SwordManList.Add(swordMan);
            _AIConnector.CitizenList.Remove(work);
            _AIConnector.MoveToClosest();
            ModifCollection(work);
            _AIConnector.CheckAndGoToCoin();
        }
        private void CreateArcher(IWork work)
        {
            var archer = new Archer(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _AIWorkConnector.ArcherList.Add(archer);
            _AIConnector.CitizenList.Remove(work);
            _AIConnector.MoveToClosest();
            ModifCollection(work);
            _AIConnector.CheckAndGoToCoin();
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
        private void SwapWorkForCitizen()
        {
            foreach (var stray in _AIConnector.CitizenList)
            {
                foreach (var _stray in _copyList)
                {
                    if (stray.GetId() == _stray.Key && stray.HasCoin() && stray.GetItemType() != _stray.Value)
                    {
                        switch (stray.GetItemType())
                        {
                            case ItemsType.Hammer:
                                stray.RemoveAllEveants();
                                CreateBuilder(stray);
                                return;
                            case ItemsType.Hoe:
                                stray.RemoveAllEveants();
                                CreateFarmer(stray);
                                return;
                            case ItemsType.Bowl:
                                stray.RemoveAllEveants();
                                CreateArcher(stray);
                                return;
                            case ItemsType.Sword:
                                stray.RemoveAllEveants();
                                CreateSwordMan(stray);
                                return;
                            default:
                                break;
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
        public void OnUpdate()
        {
            foreach (var stray in _AIConnector.StrayList)
            {
                foreach (var _stray in _copyList)
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
            SwapWorkForCitizen();
        }
    }
}

