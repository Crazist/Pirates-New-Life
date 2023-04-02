using System.Collections.Generic;
using System;
using GameInit.Connector;
using GameInit.AI;

namespace GameInit.Building
{
    public class ProductionBuilding : IBuilding
    {
        private bool isBuilded = false;
        
        private Action _startBuilding;
        private BuildingComponent _workShopComponent;
        private ResourceManager _res;
        private Produce producer;

        public ProductionBuilding(BuildingComponent workShopComponent, ResourceManager res, AIConnector _AIConnector)
        {
           _workShopComponent = workShopComponent;
           _res = res;
            CreateProducer(_AIConnector);
           _startBuilding += Build;
           _workShopComponent.SetAction(_startBuilding);
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
                producer = new Produce(_workShopComponent.gameObject.GetComponent<ProduceComponent>(), _AIConnector);
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

