using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Connector;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.Builders;
using GameInit.Enemy;

namespace GameInit.AI
{
    public class AIBuilder
    {
        public AIBuilder(BuilderConnectors builderConnectors, Pools pool, CoinDropAnimation coinDropAnimation, HeroBuilder _heroBuilder)
        {
            var camps = UnityEngine.Object.FindObjectsOfType<CampComponent>();
            var enemySpawnComponent = UnityEngine.Object.FindObjectsOfType<EnemySpawnComponent>();
            AISpawner spawner = new AISpawner(camps, builderConnectors, pool, coinDropAnimation, _heroBuilder.GetHeroComponent(), enemySpawnComponent);
        }

    }
}

