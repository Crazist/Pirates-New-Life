using GameInit.AI.Spawner;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.Enemy;
using GameInit.Builders;
using GameInit.GameCyrcleModule;
using System.Collections.Generic;

namespace GameInit.AI
{
    public class AISpawner : IDayChange
    {
        public List<IKDTree> EnemySpawnersList { get; set; }
        public List<AllySpawner> AllySpawnersList { get; set; }

        private GameCyrcle _cyrcle;
        private List<EnemySpawner> _EnemySpawnersPrivateList;


        public AISpawner(CampComponent[] camps, BuilderConnectors builderConnectors, Pools pool, CoinDropAnimation coinDropAnimation, HeroComponent heroComponent, EnemySpawnComponent[] _enemySpawnComponents, GameCyrcle cyrcle, EnemyPool EnemyPool)
        {
            EnemySpawnersList = new List<IKDTree>();
            _EnemySpawnersPrivateList = new List<EnemySpawner>();
            AllySpawnersList = new List<AllySpawner>();

            _cyrcle = cyrcle;

            CreateEnemySpawners(_enemySpawnComponents, cyrcle, EnemyPool, builderConnectors);
            CreateAllySpawners(camps, pool, builderConnectors, coinDropAnimation, heroComponent);
        }
        private void CreateEnemySpawners(EnemySpawnComponent[] _enemySpawnComponents, GameCyrcle cyrcle, EnemyPool EnemyPool, BuilderConnectors builderConnectors)
        {
            foreach (var spawner in _enemySpawnComponents)
            {
                var enemySpawner = new EnemySpawner(spawner, cyrcle, EnemyPool, builderConnectors);

                EnemySpawnersList.Add(enemySpawner);
                _EnemySpawnersPrivateList.Add(enemySpawner);
            }
        }
        private void CreateAllySpawners(CampComponent[] camps, Pools pool, BuilderConnectors builderConnectors, CoinDropAnimation coinDropAnimation, HeroComponent heroComponent)
        {
            foreach (var spawner in camps)
            {
                var enemySpawner = new AllySpawner(spawner, builderConnectors, pool, coinDropAnimation, heroComponent);

                AllySpawnersList.Add(enemySpawner);
            }
        }

        public void OnDayChange()
        {
            if (!_cyrcle.ChekIfDay())
            {
                foreach (var enemyySpawner in _EnemySpawnersPrivateList)
                {
                    enemyySpawner.DayChange();
                }
                foreach (var allySpawner in AllySpawnersList)
                {
                    allySpawner.DayChange();
                }
            }
        }
    }
}

