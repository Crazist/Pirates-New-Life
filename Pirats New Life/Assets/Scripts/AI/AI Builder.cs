using GameInit.Animation;
using GameInit.Pool;
using GameInit.Builders;
using GameInit.Enemy;
using GameInit.GameCyrcleModule;

namespace GameInit.AI
{
    public class AIBuilder
    {
        public AIBuilder(BuilderConnectors builderConnectors, Pools pool, CoinDropAnimation coinDropAnimation, HeroBuilder _heroBuilder, GameCyrcle cyrcle, EnemyPool _EnemyPool)
        {
            var camps = UnityEngine.Object.FindObjectsOfType<CampComponent>();
            var enemySpawnComponent = UnityEngine.Object.FindObjectsOfType<EnemySpawnComponent>();
            cyrcle.AddDayChange(new AISpawner(camps, builderConnectors, pool, coinDropAnimation, _heroBuilder.GetHeroComponent(), enemySpawnComponent, cyrcle, _EnemyPool));
            cyrcle.AddDayChange(new HuntingSpawner(coinDropAnimation, pool, builderConnectors, cyrcle));
        }

    }
}

