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
        private AIWorkConnector _AIWorkConnector;
        public BuilderConnectors(Pools coinPool, GameCyrcle cyrcle)
        {
            _AIConnector = new AIConnector(coinPool, cyrcle);
            _AIWorkConnector = new AIWorkConnector(coinPool, cyrcle);
            cyrcle.Add(_AIConnector);
            cyrcle.AddDayChange(_AIConnector);
            cyrcle.Add(_AIWorkConnector);
            cyrcle.AddDayChange(_AIWorkConnector);
        }
        public AIConnector GetAiConnector()
        {
            return _AIConnector;
        }
        public AIWorkConnector GetAIWorkConnector()
        {
            return _AIWorkConnector;
        }
    }
}


