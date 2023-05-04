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
        private AIWarConnector _AIWarConnector;
        private CoinDropAnimation _coinDropAnimation;
        private Pools _pool;
        private HeroComponent _heroComponent;
        private TownHallComponent _townHallComponent;
        private AIConnector _AIConnector;

        
        public WorkChecker(CoinDropAnimation coinDropAnimation,  Pools pool, HeroBuilder heroBuilder, BuilderConnectors _builderConnectors)
        {
            _AIConnector = _builderConnectors.GetAiConnector();
            _AIWarConnector = _builderConnectors.GetAIWarConnector();
            _heroComponent = heroBuilder.GetHeroComponent();
            _pool = pool;
            _coinDropAnimation = coinDropAnimation;

            _townHallComponent = Object.FindObjectOfType<TownHallComponent>();
            _townHallComponent.GetWar(_AIWarConnector);

        }

        private IWork CreateStray(IWork work)
        {
            work.RemoveAllEveants();
            var stray = new Stray(work.GetAiComponent(), work.GetId(), _pool,  _coinDropAnimation, _heroComponent, work.GetRandomWalker());
            _AIConnector.StrayList.Add(stray);

            switch (work.GetItemType())
            {
                case ItemsType.None:
                    _AIConnector.CitizenList.Remove(work);
                    break;
                case ItemsType.Hammer:
                    _AIConnector.BuilderList.Remove(work);
                    break;
                case ItemsType.Hoe:
                    _AIConnector.FarmerList.Remove(work);
                    break;
                case ItemsType.Bowl:
                    _AIConnector.ArcherList.Remove(work);
                    _AIWarConnector.ArcherList.Remove(work);
                    break;
                case ItemsType.Sword:
                    _AIConnector.SwordManList.Remove(work);
                    _AIWarConnector.SwordManList.Remove(work);
                    break;
            }

            _AIWarConnector.UpdateTree();
            return stray;
        }

        private IWork CreateCitizen(IWork work)
        {
            var citizen = new Citizen(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position, _AIConnector);
            _AIWarConnector.PointsInWorld.Add(citizen);
            _AIWarConnector.UpdateTree();
            _AIConnector.CitizenList.Add(citizen);
            _AIConnector.StrayList.Remove(work);
            _AIConnector.CheckAndGoToCoin();
            _AIConnector.MoveToClosest();
            return citizen;
        }
        private IWork CreateBuilder(IWork work)
        {
            var builder = new Builder(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position, _AIConnector);
            _AIConnector.BuilderList.Add(builder);
            _AIConnector.CitizenList.Remove(work);
            _AIWarConnector.PointsInWorld.Add(builder);
            _AIConnector.MoveToClosest();
            _AIWarConnector.UpdateTree();
            return builder;
        }
        private IWork CreateFarmer(IWork work)
        {
            var farmer = new Farmer(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _AIConnector.FarmerList.Add(farmer);
            _AIConnector.CitizenList.Remove(work);
            _AIWarConnector.PointsInWorld.Add(farmer);
            _AIConnector.MoveToClosest();
            _AIWarConnector.UpdateTree();
            return farmer;
        }
        private void CreateSwordMan(IWork work)
        {
            var swordMan = new SwordMan(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _AIWarConnector.PointsInWorld.Add(swordMan);
            _AIWarConnector.SwordManList.Add(swordMan);
            _AIConnector.SwordManList.Add(swordMan);
            _AIConnector.CitizenList.Remove(work);
            _AIConnector.MoveToClosest();
            _AIWarConnector.UpdateTree();
            _AIWarConnector.SetSwordManToNewPosition();
        }
        private void CreateArcher(IWork work)
        {
            var archer = new Archer(work.GetAiComponent(), work.GetId(), _pool, _coinDropAnimation, _heroComponent, work.GetRandomWalker(), _townHallComponent.GetTransform().position);
            _AIWarConnector.ArcherList.Add(archer);
            _AIWarConnector.PointsInWorld.Add(archer);
            _AIConnector.ArcherList.Add(archer);
            _AIConnector.CitizenList.Remove(work);
            _AIConnector.MoveToClosest();
            _AIWarConnector.UpdateTree();
        }
      
        private void SwapWorkForCitizen()
        {
            foreach (var stray in _AIConnector.CitizenList)
            {
                if (stray.HasCoin())
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
                if (!stray.HasCoin() && stray.GetItemType() == ItemsType.None)
                {
                    stray.RemoveAllEveants();
                    CreateStray(stray);
                    return;
                }
            }
        }
        private void LoseWork()
        {
            foreach (var stray in _AIConnector.BuilderList)
            {
                if (!stray.HasCoin() && stray.GetItemType() != ItemsType.None)
                {
                    stray.RemoveAllEveants();
                    CreateStray(stray);
                    return;
                }
            }
            foreach (var stray in _AIConnector.FarmerList)
            {
                if (!stray.HasCoin() && stray.GetItemType() != ItemsType.None)
                {
                    stray.RemoveAllEveants();
                    CreateStray(stray);
                    return;
                }
            }
            foreach (var stray in _AIConnector.ArcherList)
            {
                if (!stray.HasCoin() && stray.GetItemType() != ItemsType.None)
                {
                    stray.RemoveAllEveants();
                    CreateStray(stray);
                    return;
                }
            }
            foreach (var stray in _AIConnector.SwordManList)
            {
                if (!stray.HasCoin() && stray.GetItemType() != ItemsType.None)
                {
                    stray.RemoveAllEveants();
                    CreateStray(stray);
                    return;
                }
            }
        }
        public void OnUpdate()
        {
            foreach (var stray in _AIConnector.StrayList)
            {
               if(stray.GetItemType() == ItemsType.None && stray.HasCoin())
                    {
                        stray.RemoveAllEveants();
                        CreateCitizen(stray);
                        return;
                    }
            }
            SwapWorkForCitizen();
            LoseWork();
        }
    }
}

