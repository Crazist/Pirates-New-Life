using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.GameCyrcleModule;

namespace GameInit.Building
{
    public class WorkShop : IBuilding, IDayChange
    {
        private bool isBuilded = false;
        private bool inBuildProgress = false;

        private List<IBuilding> _buildingsList;
        private GameCyrcle _cyrcle;

        public WorkShop(WorkShopComponent workShopComponent, GameCyrcle cyrcle, List<IBuilding> buildingsList)
        {
            _buildingsList = buildingsList;
            _cyrcle = cyrcle;

            AddToList();
        }

        public void AddToList()
        {
            _buildingsList.Add(this);
        }

        public void Build()
        {
            throw new System.NotImplementedException();
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

