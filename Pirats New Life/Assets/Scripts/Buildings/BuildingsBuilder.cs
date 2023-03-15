using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Building;
using GameInit.GameCyrcleModule;
using System;
using GameInit.Component;
using System.ComponentModel;

namespace GameInit.Builders
{
    public class BuildingsBuilder
    {
        private List<IBuilding> _buildingsList;
        private GameCyrcle _cyrcle;
       
        public BuildingsBuilder(GameCyrcle cyrcle, ResourceManager resourceManager)
        {
            _cyrcle = cyrcle;
            _buildingsList = new List<IBuilding>();

            var workShopComponents = UnityEngine.Object.FindObjectsOfType<BuildingComponent>();
            
            CreateBuildings(workShopComponents, resourceManager);
        }

        private void CreateBuildings(BuildingComponent[] buildingsComponenets, ResourceManager resourceManager)
        {
            foreach (var component in buildingsComponenets)
            {
                switch (component.getType())
                {
                    case BuildingsType.WorkShopType:
                        _buildingsList.Add(new WorkShop(component, _cyrcle, resourceManager));
                        break;
                }
            }
        }
    }
}

