using GameInit.Enemy;
using GameInit.GameCyrcleModule;
using System;
using UnityEngine;
using GameInit.Builders;
using GameInit.Connector;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace GameInit.AI.Spawner
{
    public class EnemySpawner : IKDTree
    {
        public EntityType Type { get; set; } = EntityType.EnemySpawner;
        public SideType Side { get; set; } = SideType.None;
        public int HP { get; set; } = 10;

        private GameCyrcle _cyrcle;
        private EnemySpawnComponent _EnemySpawnComponent;
        private EnemyPool _EnemyPool;
        private AIWarConnector _AIWarConnector;

        private float heightPosition = 0.46f;
        private int maxEnemies = 50; // максимальное количество врагов
        private int currentEnemies = 0; // изначальное количество врагов
        private int multiplier = 1; // количество умножений на 2\
        private bool _canDamage = true;
        private float _delayForAttack = 3;
        private CancellationToken token;
        private bool _isAlive = true;

        private const bool _isEnemy = true;
        
        public EnemySpawner(EnemySpawnComponent EnemySpawnComponent, GameCyrcle cyrcle, EnemyPool EnemyPool, BuilderConnectors builderConnectors)
        {
            _cyrcle = cyrcle;
            _EnemySpawnComponent = EnemySpawnComponent;
            _EnemyPool = EnemyPool;
            _AIWarConnector = builderConnectors.GetAIWarConnector();

            CancellationTokenSource cts = new CancellationTokenSource();
            token = cts.Token;

            multiplier = EnemySpawnComponent.Multiplier;
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
            if (_isAlive)
            {
                Spawnenemy();
            }
        }
        public void Attack()
        {
            if (_canDamage)
            {
                AttackDelay();
            }
        }
        private async void AttackDelay()
        {
            _canDamage = false;

            var position = _EnemySpawnComponent.GetTransformSpawn().position;

            DefaultEnemy enemy = (DefaultEnemy)_EnemyPool.GetFreeElements(position);

            (enemy as IEnemy).CanStayInDay = true;

            _AIWarConnector.PointsInWorld.Add(enemy);
            _AIWarConnector.EnemyList.Add(enemy);
            _AIWarConnector.UpdateTree();

            await UniTask.Delay(TimeSpan.FromSeconds(_delayForAttack), cancellationToken: token);
            
            _canDamage = true;
        }

        public bool CheckIfCanDamage()
        {
            return _canDamage;
        }

        public bool CheckIfEnemy()
        {
            return _isEnemy;
        }

        public int CountOFDamage()
        {
            return 0; //damage enother way
        }

        public void GetDamage(int damage)
        {
            if (HP - damage <= 0)
            {
                Die();
                HP = 0;
            }
            else
            {
                HP = HP - damage;
            }
        }
        private void Die()
        {
            _isAlive = false;
            _EnemySpawnComponent.gameObject.SetActive(false);
            _EnemySpawnComponent.enabled = false;
            _AIWarConnector.PointsInWorld.Remove(this);
        }

        public Vector2 GetPositionVector2()
        {
            Vector2 _positionOnVector2;
            _positionOnVector2.x = _EnemySpawnComponent.GetTransformSpawn().position.x;
            _positionOnVector2.y = _EnemySpawnComponent.GetTransformSpawn().position.z;

            return _positionOnVector2;
        }
    }

}
