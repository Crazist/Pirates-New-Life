using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Connector;
using GameInit.Pool;
using GameInit.GameCyrcleModule;

namespace GameInit.Builders 
{
    public class BuilderConnectors
    {
        private AIConnector _AIConnector;
        public BuilderConnectors(Pools coinPool, GameCyrcle cyrcle)
        {
            _AIConnector = new AIConnector(coinPool, cyrcle);
            cyrcle.Add(_AIConnector);
            cyrcle.AddDayChange(_AIConnector);
        }
        public AIConnector GetAiConnector()
        {
            return _AIConnector;
        }
    }
}


