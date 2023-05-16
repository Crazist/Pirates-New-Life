using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Connector;
using GameInit.Pool;
using GameInit.GameCyrcleModule;
using GameInit.Hero;

namespace GameInit.Builders 
{
    public class BuilderConnectors
    {
        private AIConnector _AIConnector;
        private AIWarConnector _AIWarConnector;
        public BuilderConnectors(Pools coinPool, GameCyrcle cyrcle, ResourceManager resourceManager, ArrowPool arrowPool, ArrowPool arrowRedPool)
        {
            _AIConnector = new AIConnector(coinPool, cyrcle);
            _AIWarConnector = new AIWarConnector(coinPool, cyrcle, resourceManager, _AIConnector, arrowPool, arrowRedPool);
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
        public void GetHeroComponent(HeroComponent heroComponent, HeroMove HeroMove)
        {
            _AIWarConnector.GetHeroComponent(HeroMove);
            _AIConnector.GetHeroComponent(heroComponent);
        }
    }
}


