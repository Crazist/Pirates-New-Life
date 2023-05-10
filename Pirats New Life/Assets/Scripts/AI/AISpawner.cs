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
using System;

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
        private int maxEnemies = 50; // максимальное количество врагов
        private int currentEnemies = 0; // изначальное количество врагов
        private int multiplier = 1; // количество умножений на 2\
        private int _countOfDays = 1;
        private int _spawnCountOfStrayPerDays = 2;
        private Action<EnemySpawnComponent> _closeToPortal;

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
            _closeToPortal += SpawnEnemyIfToClose;

            multiplier = _EnemySpawnComponents[0].Multiplier;

            SetActionInEnemys();
            SpawnStray();
        }

        private void SpawnEnemyIfToClose(EnemySpawnComponent _enemySpawnPoint)
        {
            if (!_cyrcle.ChekIfDay())
            {
                var position = _enemySpawnPoint.GetTransformSpawn().position;

                DefaultEnemy enemy = (DefaultEnemy)_EnemyPool.GetFreeElements(position);

                _AIWarConnector.PointsInWorld.Add(enemy);
                _AIWarConnector.EnemyList.Add(enemy);
                _AIWarConnector.UpdateTree();
            }
        }
        private void SetActionInEnemys()
        {
            foreach (var item in _EnemySpawnComponents)
            {
                item.SetAction(_closeToPortal);
            }
        }
        private void SpawnStrayPerDays()
        {
            if(_countOfDays < _minDayToSpawnStray)
            {
                return;
            }
            else
            {
                foreach (var camp in _CampComponents)
                {
                    for (int i = 0; i < _spawnCountOfStrayPerDays; i++)
                    {
                        var pos = camp.GetTransformSpawn().position;
                        var diffmax = camp.GetSpawnDiffMax();
                        var diffmin = camp.GetSpawnDiffMin();

                        var x = UnityEngine.Random.Range(diffmin, diffmax);
                        var z = UnityEngine.Random.Range(diffmin, diffmax);

                        if (UnityEngine.Random.Range(0, 2) == 1)
                        {
                            x = -x;
                        }
                        if (UnityEngine.Random.Range(0, 2) == 1)
                        {
                            z = -z;
                        }

                        var position = new Vector3(pos.x + x, heightPosition, pos.z + z);

                        var _AIComponent = GameObject.Instantiate(camp.GetCitizenPrefab(), position, Quaternion.identity);

                        var randomWalker = new RandomWalker();
                        Stray stray = new Stray(_AIComponent, _AIConnector.GenerateId(), _pool, _coinDropAnimation, _heroComponent, randomWalker);


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

                    var x = UnityEngine.Random.Range(diffmin, diffmax);
                    var z = UnityEngine.Random.Range(diffmin, diffmax);

                    if (UnityEngine.Random.Range(0, 2) == 1)
                    {
                        x = -x;
                    }
                    if (UnityEngine.Random.Range(0, 2) == 1)
                    {
                        z = -z;
                    }

                    var position = new Vector3(pos.x + x, heightPosition, pos.z + z);

                    var _AIComponent = GameObject.Instantiate(camp.GetCitizenPrefab(), position, Quaternion.identity);
                    
                    var randomWalker = new RandomWalker();
                    Stray stray = new Stray(_AIComponent, _AIConnector.GenerateId(), _pool, _coinDropAnimation, _heroComponent, randomWalker);


                    _AIConnector.StrayList.Add(stray);
                }
            }
        }
        private void Spawnenemy()
        {
            // Проверяем, можем ли мы создать еще врага
            if (_AIWarConnector.EnemyList.Count < maxEnemies && _AIWarConnector.EnemyList.Count + multiplier <= maxEnemies)
            {
                currentEnemies = currentEnemies + multiplier; // умножаем количество врагов на значение valueToMultiply
                
                if(multiplier > maxEnemies)
                {
                    multiplier = maxEnemies;
                }
            }

            foreach (var camp in _EnemySpawnComponents)
            {
                if (camp.isActiveAndEnabled)
                {
                    camp.SetCount(currentEnemies);
                    for (int i = 0; i < camp.GetCount(); i++)
                    {
                        var pos = camp.GetTransformSpawn().position;
                        var diffmax = camp.GetSpawnDiffMax();
                        var diffmin = camp.GetSpawnDiffMin();

                        var x = UnityEngine.Random.Range(diffmin, diffmax);
                        var z = UnityEngine.Random.Range(diffmin, diffmax);

                        var position = new Vector3(pos.x + x, heightPosition, pos.z + z);

                        DefaultEnemy enemy = (DefaultEnemy)_EnemyPool.GetFreeElements(position);

                        _AIWarConnector.PointsInWorld.Add(enemy);
                        _AIWarConnector.EnemyList.Add(enemy);
                        _AIWarConnector.UpdateTree();
                    }
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

