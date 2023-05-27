using System.Collections.Generic;
using UnityEngine;
using GameInit.AI;
using System;
using GameInit.Pool;
using GameInit.Optimization.KDTree;
using GameInit.GameCyrcleModule;
using GameInit.Builders;
using System.Diagnostics;
using GameInit.Enemy;
using GameInit.Hero;
using GameInit.Building;
using UnityEngine.AI;

namespace GameInit.Connector
{
    public class AIWarConnector : IUpdate, IDayChange
    {
        public List<IWork> SwordManList { get; set; }
        public List<IWork> ArcherList { get; set; }
        public List<IKDTree> TowerList { get; set; }
        public List<IEnemy> EnemyList { get; set; }
        public List<IAnimal> AnimalList { get; set; }
        public List<Wall> RightWall { get; set; }
        public List<Wall> LeftWall { get; set; }

        public List<IKDTree> PointsInWorld { get; set; }

        private Pools _pool;
        private HeroMove _HeroMove;
        private List<Action> lateMove;
        private GameCyrcle _gameCyrcle;
        private BuildingsBuilder _buildingsBuilder;
        private ResourceManager _resourceManager;
        private AIConnector _AIConnector;
        private KDTree _tree;
        private KDQuery _treeQuery;
        private ArrowPool _arrowPool;
        private ArrowPool _arrowRedPool;
        private float lastUpdateTime = 0.0f;
        private const float updateInterval = 0.5f;
        private List<EnemySpawnComponent> _EnemySpawnerPsoition;
        private SideCalculation _SideCalculation;
        
        public AIWarConnector(Pools pool, GameCyrcle cyrcle, ResourceManager resourceManager, AIConnector AIConnector, ArrowPool arrowPool, ArrowPool arrowRedPool)
        {
            PointsInWorld = new List<IKDTree>();

            SwordManList = new List<IWork>();
            ArcherList = new List<IWork>();
            EnemyList = new List<IEnemy>();
            AnimalList = new List<IAnimal>();
            TowerList = new List<IKDTree>();

            RightWall = new List<Wall>();
            LeftWall = new List<Wall>();

            _AIConnector = AIConnector;
            _resourceManager = resourceManager;
            _gameCyrcle = cyrcle;
            _pool = pool;
            _arrowPool = arrowPool;
            _arrowRedPool = arrowRedPool;

            _tree = new KDTree();
            _treeQuery = new KDQuery();

            InitEnemySpawnerPosition();

            _SideCalculation = new SideCalculation(this,cyrcle);
        }
        
        private void InitEnemySpawnerPosition()
        {
            var  _enemySpawnComponent = UnityEngine.Object.FindObjectsOfType<EnemySpawnComponent>();

            _EnemySpawnerPsoition = new List<EnemySpawnComponent>();

            for (int i = 0; i < _enemySpawnComponent.Length; i++)
            {
                _EnemySpawnerPsoition.Add(_enemySpawnComponent[i]);
            }
            _treeQuery.SetEnemySpawner(_gameCyrcle, _EnemySpawnerPsoition, PointsInWorld, EnemyList, AnimalList);
        }
        public void GetHeroComponent(HeroMove HeroMove)
        {
            _HeroMove = HeroMove;
        }
        public void OnUpdate()
        {
            if (Time.time - lastUpdateTime > updateInterval && PointsInWorld.Count != 0)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                _tree.Rebuild(3);

                UpdateEnemyRunForAttack();
                UpdateArcherMoveAndShoot();
                UpdateAllyRunForAttack();
                TowerShoot();
                UpdateDamageClosest();
                UpdateEnimalsRunAway();

                stopwatch.Stop(); // останавливаем таймер
                UnityEngine.Debug.Log(stopwatch.Elapsed.TotalMilliseconds.ToString("F2") + " microseconds for tree to build"); // выводим результат в милисекунд
                lastUpdateTime = Time.time;
            }
        }
        private void UpdateEnemyRunForAttack()
        {
            for (int i = 0; i < EnemyList.Count; i++)
            {
                _treeQuery.EnemyRunForAttack(_tree, EnemyList[i]);
            }
        }
        private void UpdateAllyRunForAttack()
        {
            for (int i = 0; i < SwordManList.Count; i++)
            {
                _treeQuery.ShildAllyRunForAttack(_tree, (IKDTree)SwordManList[i]);
            }
        }
        private void UpdateArcherMoveAndShoot()
        {
            for (int i = 0; i < ArcherList.Count; i++)
            {
                _treeQuery.ArcherMove(_tree, (IKDTree)ArcherList[i]);
                _treeQuery.ShootClosest(_tree, (IKDTree)ArcherList[i], _arrowPool, _treeQuery);
            }
        }
        private void TowerShoot()
        {
            for (int i = 0; i < TowerList.Count; i++)
            {
                _treeQuery.ShootClosest(_tree, TowerList[i], _arrowRedPool, _treeQuery);
            }
        }
        private void UpdateDamageClosest()
        {
            for (int i = 0; i < PointsInWorld.Count; i++)
            {
                if (PointsInWorld[i].HP > 0 && PointsInWorld[i].CheckIfCanDamage())
                    _treeQuery.DamageClosest(_tree, PointsInWorld[i]);
            }
        }
        private void UpdateEnimalsRunAway()
        {
           if(_HeroMove != null)
            _treeQuery.EnimalsRunAway(_tree, (IKDTree)_HeroMove);
        }
        public void UpdateAttack(List<IKDTree> army, bool isRight, AttackTypes type)
        {
            float offset = 20;

            if (type == AttackTypes.ArcherType)
            {
                offset = 10;
            }
            else
            {
                offset = 7;
            }

            EnemySpawnComponent rightSpawner = null;
            EnemySpawnComponent leftSpawner = null;

            foreach (var item in _EnemySpawnerPsoition)
            {
                if (item.IsRight)
                {
                    rightSpawner = item;
                }
                else
                {
                    leftSpawner = item;
                }
            }

            List<Vector3> occupiedPositions = new List<Vector3>(); // Список занятых позиций

            if (isRight)
            {
                foreach (var item in army)
                {
                    var unit = (IAttack)item;
                    unit.InAttack = true;

                    var iWorkArcmy = (IWork)item;
                    iWorkArcmy.GetRandomWalker().StopRandomWallk();

                    var targetPos = new Vector3(rightSpawner.transform.position.x - offset, 0.46f, rightSpawner.transform.position.z);

                    while (occupiedPositions.Contains(targetPos))
                    {
                        // Если позиция уже занята, смещаем юнита немного выше по оси Z
                        targetPos += new Vector3(0, 0.1f, 0);

                        // Проверяем, занята ли новая позиция после смещения
                        if (!occupiedPositions.Contains(targetPos))
                        {
                            targetPos = GetFreeNavMeshPosition(targetPos);
                            targetPos = new Vector3(targetPos.x - offset, 0.46f, targetPos.z);
                            break; // Если новая позиция свободна, выходим из цикла
                        }
                    }

                    iWorkArcmy.Move(targetPos, null);

                    occupiedPositions.Add(targetPos); // Добавляем позицию в список занятых позиций
                }
            }
            else
            {
                foreach (var item in army)
                {
                    var unit = (IAttack)item;
                    unit.InAttack = true;

                    var iWorkArcmy = (IWork)item;
                    iWorkArcmy.GetRandomWalker().StopRandomWallk();
                    
                    var targetPos = new Vector3(leftSpawner.transform.position.x + offset, 0.46f, leftSpawner.transform.position.z);

                    while (occupiedPositions.Contains(targetPos))
                    {
                        // Если позиция уже занята, смещаем юнита немного выше по оси Z
                        targetPos += new Vector3(0, 0.1f, 0);

                        // Проверяем, занята ли новая позиция после смещения
                        if (!occupiedPositions.Contains(targetPos))
                        {
                            targetPos = GetFreeNavMeshPosition(targetPos);
                            targetPos = new Vector3(targetPos.x + offset, 0.46f, targetPos.z);
                            break; // Если новая позиция свободна, выходим из цикла
                        }
                    }

                    iWorkArcmy.Move(targetPos, null);

                    occupiedPositions.Add(targetPos); // Добавляем позицию в список занятых позиций
                }
            }
        }
        private Vector3 GetFreeNavMeshPosition(Vector3 targetPos)
        {
            // Проверьте, доступна ли целевая позиция на NavMesh
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                return hit.position; // Возвращаем целевую позицию, так как она свободна на NavMesh
            }
            else
            {
                // Целевая позиция недоступна, ищем ближайшую свободную позицию на краю NavMesh
                if (NavMesh.FindClosestEdge(targetPos, out NavMeshHit closestEdge, NavMesh.AllAreas))
                {
                    return closestEdge.position; // Возвращаем позицию на краю NavMesh
                }
                else
                {
                    return Vector3.zero; // Возвращаем нулевой вектор, если не удалось найти свободную позицию
                }
            }
        }
        public void UpdateTree()
        {
            _tree.Build(PointsInWorld, 3);
        }
        public void DrawGiz()
        {
            _tree.DrawNode(_tree.rootNode);
        }
        public SideCalculation GetSideCalculation()
        {
            return _SideCalculation;
        }
        public void OnDayChange()
        {
            _SideCalculation.SetSwordManToNewPosition();
            // SetSwordManToNewPosition();
        }
    }
}
