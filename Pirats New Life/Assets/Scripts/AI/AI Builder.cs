using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Connector;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.Builders;

namespace GameInit.AI
{
    public class AIBuilder
    {
        public AIBuilder(AIConnector AIConnector, Pools pool, CoinDropAnimation coinDropAnimation, HeroBuilder _heroBuilder)
        {
            var camps = UnityEngine.Object.FindObjectsOfType<CampComponent>();
            AISpawner spawner = new AISpawner(camps, AIConnector, pool, coinDropAnimation, _heroBuilder.GetHeroComponent());
        }

    }
}

