using System.Collections.Generic;
using UnityEngine;
using GameInit.Building;
using GameInit.GameCyrcleModule;
using GameInit.Pool;
using GameInit.Animation;
using GameInit.Optimization;

namespace GameInit.Builders
{
    public class BuildingsBuilder
    {
        private List<Wall> _buildingsList;
        private const int _baseCanProduce = 0;
        public BuildingsBuilder(GameCyrcle cyrcle, ResourceManager resourceManager, BuilderConnectors builderConnectors, HeroComponent heroComponent, Pools pool, CoinDropAnimation coinDropAnimation)
        {
            _buildingsList = new List<Wall>();

            BuildingTownHallComponent _townHallComponent = UnityEngine.Object.FindObjectOfType<BuildingTownHallComponent>();

            var allBuildingsComponents = UnityEngine.Object.FindObjectsOfType<BuildingComponent>();
            
            CreateBuildings(allBuildingsComponents, resourceManager, builderConnectors, heroComponent, pool, coinDropAnimation, cyrcle, _townHallComponent);
            CreateTownHall(_townHallComponent, coinDropAnimation, pool, builderConnectors, cyrcle);
        }

        private void CreateTownHall(BuildingTownHallComponent townHallComponent, CoinDropAnimation coinDropAnimation , Pools pool, BuilderConnectors builderConnectors, GameCyrcle _cyrcle)
        {
            TownHall _townHall = new TownHall(townHallComponent, coinDropAnimation, pool, _cyrcle, builderConnectors.GetAIWarConnector());
            _cyrcle.AddDayChange(_townHall);
        }
        private void CreateBuildings(BuildingComponent[] buildingsComponenets, ResourceManager resourceManager, 
            BuilderConnectors builderConnectors, HeroComponent heroComponent, Pools pool, CoinDropAnimation coinDropAnimation, GameCyrcle _cyrcle, BuildingTownHallComponent _townHallComponent)
        {
            var _AIConnector = builderConnectors.GetAiConnector();
            var _WarConnector = builderConnectors.GetAIWarConnector();

            var center = _townHallComponent.GetTransform().position;

            foreach (var component in buildingsComponenets)
            {
                switch (component.getType())
                {
                    case BuildingsType.Wall:
                        var building = new Wall(component, resourceManager, builderConnectors, _cyrcle, _buildingsList, pool, coinDropAnimation);
                        _buildingsList.Add(building);
                        _WarConnector.PointsInWorld.Add(building);
                        _WarConnector.UpdateTree();
                        _cyrcle.AddDayChange(building);
                        break;
                    case BuildingsType.Farm:
                        var farm = new Farm(component, resourceManager, _AIConnector, _cyrcle, heroComponent, pool, coinDropAnimation);
                        _cyrcle.AddDayChange(farm);
                        _cyrcle.Add(farm);
                        break;
                    default:
                       new ProductionBuilding(component, resourceManager, _AIConnector, pool, coinDropAnimation);
                        break;
                }
            }
            CalculateWallsPosition(center, builderConnectors);
        }

        private void CalculateWallsPosition(Vector3 center, BuilderConnectors builderConnectors)
        {
            List<Wall> rightWalls = new List<Wall>();
            List<Wall> leftWalls = new List<Wall>();

            var _WarConnector = builderConnectors.GetAIWarConnector();

            foreach (var wall in _buildingsList)
            {
                if (center.x > wall.GetPositionVector2().x)
                {
                    leftWalls.Add(wall);
                }
                else
                {
                    rightWalls.Add(wall);
                }
            }

            leftWalls.Sort((a, b) => Distance.Manhattan(a.GetPositionVector3(), center).CompareTo(Distance.Manhattan(b.GetPositionVector3(), center)));
            rightWalls.Sort((a, b) => Distance.Manhattan(a.GetPositionVector3(), center).CompareTo(Distance.Manhattan(b.GetPositionVector3(), center)));

            for (int i = 0; i < leftWalls.Count; i++)
            {
                leftWalls[i].CirclePosition = i;
                leftWalls[i].IsRight = false;
                if(i == _baseCanProduce)
                     leftWalls[i].GetWallComponent().SetCanProduce(true);
                else
                    leftWalls[i].GetWallComponent().SetCanProduce(false);
            }
            for (int i = 0; i < rightWalls.Count; i++)
            {
                rightWalls[i].CirclePosition = i;
                rightWalls[i].IsRight = true;
                if (i == _baseCanProduce)
                    rightWalls[i].GetWallComponent().SetCanProduce(true);
                else
                    rightWalls[i].GetWallComponent().SetCanProduce(false);
            }

            _WarConnector.RightWall = rightWalls;
            _WarConnector.LeftWall = leftWalls;
        }
    }
}

