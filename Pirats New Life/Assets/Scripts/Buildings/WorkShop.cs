using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.GameCyrcleModule;
using System;

namespace GameInit.Building
{
    public class WorkShop : IBuilding, IDayChange
    {
        private bool isBuilded = false;
        private bool inBuildProgress = false;

        private List<IBuilding> _buildingsList;
        private GameCyrcle _cyrcle;
        private Action _startBuilding;
        private BuildingComponent _workShopComponent;
        private ResourceManager _res;
        private Produce producer;

        public WorkShop(BuildingComponent workShopComponent, GameCyrcle cyrcle, ResourceManager res)
        {
           _workShopComponent = workShopComponent;
           _cyrcle = cyrcle;
           _res = res;
            CreateProducer();
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

        public void CreateProducer()
        {
             producer = new Produce(_workShopComponent.gameObject.GetComponent<ProduceComponent>());
            
        }

        public bool CheckForDay()
        {
            return _cyrcle.ChekIfDay();
        }

        public void OnDayChange()
        {
            throw new System.NotImplementedException();
        }

        public bool GetBuildingState()
        {
            return isBuilded;
        }

    }
}

