using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Building;
using GameInit.GameCyrcleModule;
using System;
using GameInit.Component;
using System.ComponentModel;
using GameInit.Connector;

namespace GameInit.Builders
{
    public class BuildingsBuilder
    {
        private List<IBuilding> _buildingsList;
        private GameCyrcle _cyrcle;
       
        public BuildingsBuilder(GameCyrcle cyrcle, ResourceManager resourceManager, AIConnector _AIConnector)
        {
            _cyrcle = cyrcle;
            _buildingsList = new List<IBuilding>();

            var workShopComponents = UnityEngine.Object.FindObjectsOfType<BuildingComponent>();
            
            CreateBuildings(workShopComponents, resourceManager, _AIConnector);
        }

        private void CreateBuildings(BuildingComponent[] buildingsComponenets, ResourceManager resourceManager, AIConnector _AIConnector)
        {
            foreach (var component in buildingsComponenets)
            {
                switch (component.getType())
                {
                    case BuildingsType.WorkShopType:
                        _buildingsList.Add(new WorkShop(component, _cyrcle, resourceManager, _AIConnector));
                        break;
                }
            }
        }
    }
}

