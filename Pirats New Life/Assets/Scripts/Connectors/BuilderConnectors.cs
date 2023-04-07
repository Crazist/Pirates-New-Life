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
        private AIWarConnector _AIWarConnector;
        public BuilderConnectors(Pools coinPool, GameCyrcle cyrcle, ResourceManager resourceManager)
        {
            _AIConnector = new AIConnector(coinPool, cyrcle);
            _AIWarConnector = new AIWarConnector(coinPool, cyrcle, resourceManager, _AIConnector);
            cyrcle.Add(_AIConnector);
            cyrcle.AddDayChange(_AIConnector);
            cyrcle.Add(_AIWarConnector);
            cyrcle.AddDayChange(_AIWarConnector);
        }
        public AIConnector GetAiConnector()
        {
            return _AIConnector;
        }
        public AIWarConnector GetAIWarConnector()
        {
            return _AIWarConnector;
        }
    }
}


