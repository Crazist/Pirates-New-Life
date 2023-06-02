using GameInit.Animation;
using GameInit.Pool;
using GameInit.Builders;
using GameInit.Enemy;
using GameInit.GameCyrcleModule;

namespace GameInit.AI
{
    public class AIBuilder
    {
        private AISpawner _aiSpawner;
        public AIBuilder(BuilderConnectors builderConnectors, Pools pool, CoinDropAnimation coinDropAnimation, HeroBuilder _heroBuilder, GameCyrcle cyrcle, EnemyPool _EnemyPool, WorkChecker _workChecker)
        {
            var camps = UnityEngine.Object.FindObjectsOfType<CampComponent>();
            var enemySpawnComponent = UnityEngine.Object.FindObjectsOfType<EnemySpawnComponent>();
            _aiSpawner = new AISpawner(camps, builderConnectors, pool, coinDropAnimation, _heroBuilder.GetHeroComponent(), enemySpawnComponent, cyrcle, _EnemyPool, _workChecker);
            cyrcle.AddDayChange(_aiSpawner);
            cyrcle.AddDayChange(new HuntingSpawner(coinDropAnimation, pool, builderConnectors, cyrcle));
        }

        public AISpawner GetAISpawner()
        {
            return _aiSpawner;
        }
    }
}

