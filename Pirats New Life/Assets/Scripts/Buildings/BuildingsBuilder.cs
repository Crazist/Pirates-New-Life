using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Building;
using GameInit.GameCyrcleModule;
using System;
using GameInit.Component;
using System.ComponentModel;
using GameInit.Connector;
using GameInit.Pool;
using GameInit.Animation;

namespace GameInit.Builders
{
    public class BuildingsBuilder
    {
        private List<IBuilding> _buildingsList;
        private GameCyrcle _cyrcle;
       
        public BuildingsBuilder(GameCyrcle cyrcle, ResourceManager resourceManager, BuilderConnectors builderConnectors, HeroComponent heroComponent, Pools pool, CoinDropAnimation coinDropAnimation)
        {
            _cyrcle = cyrcle;
            _buildingsList = new List<IBuilding>();

            var allBuildingsComponents = UnityEngine.Object.FindObjectsOfType<BuildingComponent>();
            
            CreateBuildings(allBuildingsComponents, resourceManager, builderConnectors.GetAiConnector(), heroComponent, pool, coinDropAnimation);
        }

        private void CreateBuildings(BuildingComponent[] buildingsComponenets, ResourceManager resourceManager, AIConnector _AIConnector, HeroComponent heroComponent, Pools pool, CoinDropAnimation coinDropAnimation)
        {
            foreach (var component in buildingsComponenets)
            {
                switch (component.getType())
                {
                    case BuildingsType.Wall:
                        var building = new Wall(component, resourceManager, _AIConnector, _cyrcle);
                        _cyrcle.AddDayChange(building);
                        break;
                    case BuildingsType.Farm:
                        var farm = new Farm(component, resourceManager, _AIConnector, _cyrcle, heroComponent, pool, coinDropAnimation);
                        _cyrcle.AddDayChange(farm);
                        _cyrcle.Add(farm);
                        break;
                    default:
                        _buildingsList.Add(new ProductionBuilding(component, resourceManager, _AIConnector));
                        break;
                }
            }
        }
    }
}

