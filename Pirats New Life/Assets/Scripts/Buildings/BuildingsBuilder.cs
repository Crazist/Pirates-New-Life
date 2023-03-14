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

        public BuildingsBuilder(GameCyrcle cyrcle, ResourceManager resourceManager)
        {
            _buildingsList = new List<IBuilding>();

            var workShopComponents = UnityEngine.Object.FindObjectsOfType<WorkShopComponent>();

            CreateBuildings(workShopComponents);
        }

        private void CreateBuildings(WorkShopComponent[] buildingsComponenets)
        {
            foreach (var component in buildingsComponenets)
            {
               _buildingsList.Add(new WorkShop(component));
            }
        }
    }
}

