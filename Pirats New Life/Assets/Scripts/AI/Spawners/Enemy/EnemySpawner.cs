using GameInit.Enemy;
using GameInit.GameCyrcleModule;
using System;
using UnityEngine;
using GameInit.Builders;
using GameInit.Connector;

namespace GameInit.AI.Spawner
{
    public class EnemySpawner : IKDTree
    {
        public EntityType Type { get; set; }
        public int HP { get; set; }

        private Action _closeToPortal;
        private GameCyrcle _cyrcle;
        private EnemySpawnComponent _EnemySpawnComponent;
        private EnemyPool _EnemyPool;
        private AIWarConnector _AIWarConnector;

        private float heightPosition = 0.46f;
        private int maxEnemies = 50; // максимальное количество врагов
        private int currentEnemies = 0; // изначальное количество врагов
        private int multiplier = 1; // количество умножений на 2\


        public EnemySpawner(EnemySpawnComponent EnemySpawnComponent, GameCyrcle cyrcle, EnemyPool EnemyPool, BuilderConnectors builderConnectors)
        {
            _cyrcle = cyrcle;
            _EnemySpawnComponent = EnemySpawnComponent;
            _EnemyPool = EnemyPool;
            _AIWarConnector = builderConnectors.GetAIWarConnector();

            multiplier = EnemySpawnComponent.Multiplier;

            _closeToPortal += SpawnEnemyIfToClose;

            EnemySpawnComponent.SetAction(_closeToPortal);
        }

        private void SpawnEnemyIfToClose()
        {
            if (!_cyrcle.ChekIfDay())
            {
                var position = _EnemySpawnComponent.GetTransformSpawn().position;

                DefaultEnemy enemy = (DefaultEnemy)_EnemyPool.GetFreeElements(position);

                _AIWarConnector.PointsInWorld.Add(enemy);
                _AIWarConnector.EnemyList.Add(enemy);
                _AIWarConnector.UpdateTree();
            }
        }
        private void Spawnenemy()
        {
            // Проверяем, можем ли мы создать еще врага
            if (_AIWarConnector.EnemyList.Count < maxEnemies && _AIWarConnector.EnemyList.Count + multiplier <= maxEnemies)
            {
                currentEnemies = currentEnemies + multiplier; // умножаем количество врагов на значение valueToMultiply

                if (multiplier > maxEnemies)
                {
                    multiplier = maxEnemies;
                }
            }

            if (_EnemySpawnComponent.isActiveAndEnabled)
            {
                _EnemySpawnComponent.SetCount(currentEnemies);
                for (int i = 0; i < _EnemySpawnComponent.GetCount(); i++)
                {
                    var pos = _EnemySpawnComponent.GetTransformSpawn().position;
                    var diffmax = _EnemySpawnComponent.GetSpawnDiffMax();
                    var diffmin = _EnemySpawnComponent.GetSpawnDiffMin();

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
        public void DayChange()
        {
            Spawnenemy();
        }
        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        public bool CheckIfCanDamage()
        {
            throw new System.NotImplementedException();
        }

        public bool CheckIfEnemy()
        {
            throw new System.NotImplementedException();
        }

        public int CountOFDamage()
        {
            throw new System.NotImplementedException();
        }

        public void GetDamage(int damage)
        {
            throw new System.NotImplementedException();
        }

        public Vector2 GetPositionVector2()
        {
            throw new System.NotImplementedException();
        }
    }

}
