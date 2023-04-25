using System.Collections.Generic;
using System;
using GameInit.Connector;
using GameInit.AI;
using GameInit.Pool;
using GameInit.Animation;

namespace GameInit.Building
{
    public class ProductionBuilding : IBuilding
    {
        private bool isBuilded = false;
        
        private Action _startBuilding;
        private BuildingComponent _workShopComponent;
        private ResourceManager _res;
        private Produce producer;
        private Action<int> DropBeforePickUp;
        private CoinDropAnimation _coinDropAnimation;
        private Pools _coinPool;

        private const bool canPickUp = false;
        public ProductionBuilding(BuildingComponent workShopComponent, ResourceManager res, AIConnector _AIConnector, Pools coinPool, CoinDropAnimation coinDropAnimation)
        {
           _coinDropAnimation = coinDropAnimation;
           _coinPool = coinPool;
           _workShopComponent = workShopComponent;
           _res = res;
            CreateProducer(_AIConnector);
           _startBuilding += Build;
            DropBeforePickUp += DropBeforePickUpCoin;
            _workShopComponent.SetAction(_startBuilding, DropBeforePickUp);
        }
        private void DropBeforePickUpCoin(int count)
        {
            _coinDropAnimation.RandomCoinJump(_workShopComponent.GetBuildPositions()[0].position, count, _workShopComponent.GetBuildPositions()[0].position, _coinPool, canPickUp);
        }
        public void Build()
        {
            if(_workShopComponent.ChekMaxLvl())
            {
                producer.CanProduce();
            }
            else
            {
                _workShopComponent.SetCountForGold(_workShopComponent.GetCurCountOFGold() * 2);
            }
        }

        public void CreateProducer(AIConnector _AIConnector)
        {
            var produceComponent = _workShopComponent.gameObject.GetComponent<ProduceComponent>();
            if (produceComponent != null)
            {
                producer = new Produce(_workShopComponent.gameObject.GetComponent<ProduceComponent>(), _AIConnector, _coinPool, _coinDropAnimation, _workShopComponent);
            }
        }

        public bool CheckForDay()
        {
            return false;
        }

        public bool GetBuildingState()
        {
            return isBuilded;
        }

        public void SetBuilder(IWork worker)
        {
            return; //no builder in fast build building
        }
    }
}

