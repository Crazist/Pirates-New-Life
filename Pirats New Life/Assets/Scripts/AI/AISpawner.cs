using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Connector;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.RandomWalk;
using GameInit.Enemy;
using GameInit.Builders;
using GameInit.GameCyrcleModule;

namespace GameInit.AI
{
    public class AISpawner : IDayChange
    {
        private EnemySpawnComponent[] _EnemySpawnComponents;
        private CampComponent[] _CampComponents;
        private AIWarConnector _AIWarConnector;
        private AIConnector _AIConnector;
        private Pools _pool;
        private HeroComponent _heroComponent;
        private CoinDropAnimation _coinDropAnimation;
        private GameCyrcle _cyrcle;
        private EnemyPool _EnemyPool;
        private float heightPosition = 0.46f;
        private int maxEnemies = 100; // максимальное количество врагов
        private int currentEnemies = 0; // изначальное количество врагов
        private int multiplier = 1; // количество умножений на 2\
        private int _countOfDays = 0;
        private int _spawnCountOfStrayPerDays = 2;

        private const int _minDayToSpawnStray = 3;

        public AISpawner(CampComponent[] camps, BuilderConnectors builderConnectors, Pools pool, CoinDropAnimation coinDropAnimation, HeroComponent heroComponent, EnemySpawnComponent[] _enemySpawnComponents, GameCyrcle cyrcle, EnemyPool EnemyPool)
        {
            _AIConnector = builderConnectors.GetAiConnector();
            _AIWarConnector = builderConnectors.GetAIWarConnector();

            _EnemyPool = EnemyPool;
            _EnemySpawnComponents = _enemySpawnComponents;
            _CampComponents = camps;
            _pool = pool;
            _heroComponent = heroComponent;
            _coinDropAnimation = coinDropAnimation;
            _cyrcle = cyrcle;

            multiplier = _EnemySpawnComponents[0].Multiplier;

            SpawnStray();
        }

        private void SpawnStrayPerDays()
        {
            if(_countOfDays < _minDayToSpawnStray)
            {
                return;
            }
            else
            {
                _countOfDays = 0;


                foreach (var camp in _CampComponents)
                {
                    int needToSpawn = 0;

                    var strays = camp.GetStrayList();

                    for (int i = strays.Count - 1; i >= 0; i--)
                    {
                        if (!strays[i].HasCoin())
                        {
                            strays.RemoveAt(i);
                        }
                    }

                    if (strays.Count < 2)
                    {
                        needToSpawn = _spawnCountOfStrayPerDays;
                    }

                    for (int i = 0; i < needToSpawn; i++)
                    {
                        var pos = camp.GetTransformSpawn().position;
                        var diffmax = camp.GetSpawnDiffMax();
                        var diffmin = camp.GetSpawnDiffMin();

                        var x = Random.Range(diffmin, diffmax);
                        var z = Random.Range(diffmin, diffmax);

                        if (Random.Range(0, 2) == 1)
                        {
                            x = -x;
                        }
                        if (Random.Range(0, 2) == 1)
                        {
                            z = -z;
                        }

                        var position = new Vector3(pos.x + x, heightPosition, pos.z + z);

                        var obj = GameObject.Instantiate(camp.GetCitizenPrefab(), position, Quaternion.identity);
                        var _AIComponent = obj.GetComponent<AIComponent>();

                        var randomWalker = new RandomWalker();
                        Stray stray = new Stray(_AIComponent, _AIConnector.GenerateId(), _pool, _coinDropAnimation, _heroComponent, randomWalker);

                        strays.Add(stray);

                        _AIConnector.StrayList.Add(stray);
                    }
                }
            }
        }
        private void SpawnStray()
        {
            foreach (var camp in _CampComponents)
            {
                for (int i = 0; i < camp.GetCount(); i++)
                {
                    var pos = camp.GetTransformSpawn().position;
                    var diffmax = camp.GetSpawnDiffMax();
                    var diffmin = camp.GetSpawnDiffMin();

                    var x = Random.Range(diffmin, diffmax);
                    var z = Random.Range(diffmin, diffmax);

                    if (Random.Range(0, 2) == 1)
                    {
                        x = -x;
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        z = -z;
                    }

                    var position = new Vector3(pos.x + x, heightPosition, pos.z + z);

                    var obj = GameObject.Instantiate(camp.GetCitizenPrefab(), position, Quaternion.identity);
                    var _AIComponent = obj.GetComponent<AIComponent>();

                    var randomWalker = new RandomWalker();
                    Stray stray = new Stray(_AIComponent, _AIConnector.GenerateId(), _pool, _coinDropAnimation, _heroComponent, randomWalker);


                    _AIConnector.StrayList.Add(stray);
                }
            }
        }
        private void Spawnenemy()
        {
            // Проверяем, можем ли мы создать еще врага
            if (currentEnemies < maxEnemies && currentEnemies + multiplier <= maxEnemies)
            {
                currentEnemies = currentEnemies + multiplier; // умножаем количество врагов на значение valueToMultiply
            }

            foreach (var camp in _EnemySpawnComponents)
            {
                camp.SetCount(currentEnemies);
                for (int i = 0; i < camp.GetCount(); i++)
                {
                    var pos = camp.GetTransformSpawn().position;
                    var diffmax = camp.GetSpawnDiffMax();
                    var diffmin = camp.GetSpawnDiffMin();

                    var x = Random.Range(diffmin, diffmax);
                    var z = Random.Range(diffmin, diffmax);

                    if (Random.Range(0, 2) == 1)
                    {
                        x = -x;
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        z = -z;
                    }

                    var position = new Vector3(pos.x + x, heightPosition, pos.z + z);

                    DefaultEnemy enemy = (DefaultEnemy)_EnemyPool.GetFreeElements(position);

                    _AIWarConnector.PointsInWorld.Add(enemy);
                    _AIWarConnector.EnemyList.Add(enemy);
                    _AIWarConnector.UpdateTree();
                }
            }
        }

        public void OnDayChange()
        {
            if (!_cyrcle.ChekIfDay())
            {
                _countOfDays++;
                Spawnenemy();
                SpawnStrayPerDays();
            }
        }
    }
}

