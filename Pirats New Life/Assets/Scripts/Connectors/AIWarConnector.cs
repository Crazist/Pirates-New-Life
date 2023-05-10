using System.Collections.Generic;
using UnityEngine;
using GameInit.AI;
using System;
using GameInit.Pool;
using GameInit.RandomWalk;
using GameInit.Optimization.KDTree;
using GameInit.GameCyrcleModule;
using GameInit.Building;
using GameInit.Builders;
using GameInit.Connector;
using System.Diagnostics;
using GameInit.Enemy;
using System.Linq;
using GameInit.Hero;

namespace GameInit.Connector
{
    public class AIWarConnector : IUpdate, IDayChange
    {
        public List<IWork> SwordManList { get; set; }
        public List<IWork> ArcherList { get; set; }
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
        private float lastUpdateTime = 0.0f;
        private const float updateInterval = 0.5f;
        private Transform[] _EnemySpawnerPsoition;
        private Wall lastRightWall = null;
        private Wall lastLeftWall = null;

        private float radiusRandomWalkInDay = 10;
        private float radiusRandomWalkInNight = 3;

        private const float _wallOffset = 22f;
        private const float radiusRandomWalkAnimal = 20f;
        private const int offsetSwordMan = 4;
        private const int offsetArcher = 7;
        private const int offsetArcherInday = 10;
        public AIWarConnector(Pools pool, GameCyrcle cyrcle, ResourceManager resourceManager, AIConnector AIConnector, ArrowPool arrowPool)
        {
            PointsInWorld = new List<IKDTree>();

            SwordManList = new List<IWork>();
            ArcherList = new List<IWork>();
            EnemyList = new List<IEnemy>();
            AnimalList = new List<IAnimal>();

            RightWall = new List<Wall>();
            LeftWall = new List<Wall>();

            _AIConnector = AIConnector;
            _resourceManager = resourceManager;
            _gameCyrcle = cyrcle;
            _pool = pool;
            _arrowPool = arrowPool;

            _tree = new KDTree();
            _treeQuery = new KDQuery();

            InitEnemySpawnerPosition();
        }
        
        private void InitEnemySpawnerPosition()
        {
            var  _enemySpawnComponent = UnityEngine.Object.FindObjectsOfType<EnemySpawnComponent>();

            _EnemySpawnerPsoition = new Transform[_enemySpawnComponent.Length];

            for (int i = 0; i < _enemySpawnComponent.Length; i++)
            {
                _EnemySpawnerPsoition[i] = _enemySpawnComponent[i].transform;
            }
            _treeQuery.SetEnemySpawner(_gameCyrcle, _EnemySpawnerPsoition, PointsInWorld, EnemyList, AnimalList);
        }

        public void RandomAnimalPosition()
        {
            List<IAnimal> AnimalListSorted = new List<IAnimal>(AnimalList.Count);

            Vector3 wallPos = Vector3.zero;
            
            if (lastRightWall != null)
            {
                wallPos = lastRightWall.GetPositionVector3();
            }
            else if (lastLeftWall != null)
            {
                wallPos = lastLeftWall.GetPositionVector3();
            }
            else
            {
                return;
            }

            // Sort animals by distance to the wall position
            for (int i = 0; i < AnimalList.Count; i++)
            {
                IAnimal closestAnimal = null;
                float closestDistance = Mathf.Infinity;

                foreach (IAnimal animal in AnimalList)
                {
                    if (AnimalListSorted.Contains(animal))
                    {
                        continue;
                    }

                    float distance = Vector2.SqrMagnitude(new Vector2(animal.GetAiComponent().GetTransform().position.x, animal.GetAiComponent().GetTransform().position.z) - new Vector2(wallPos.x, wallPos.z));

                    if (distance < closestDistance)
                    {
                        closestAnimal = animal;
                        closestDistance = distance;
                    }
                }

                AnimalListSorted.Add(closestAnimal);
            }

            Vector3 targetWall = Vector3.zero;
            // Перемещаем Archer по последовательности стен
            if (lastRightWall != null)
            {
                for (int i = 0; i < AnimalListSorted.Count / 2; i++)
                {
                    targetWall = new Vector3(lastRightWall.GetPositionVector3().x + _wallOffset, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    

                    if (targetWall != Vector3.zero)
                    {
                        AnimalListSorted[i].ChangePositionForRandomWallk(targetWall, radiusRandomWalkAnimal);
                    }
                }
            }
            if (lastLeftWall != null)
            {
                int sort = 0;
                int count = 0;

                if (lastRightWall != null)
                {
                    sort = AnimalListSorted.Count / 2;
                    count = AnimalListSorted.Count;
                }
                else
                {
                    count = AnimalListSorted.Count / 2;
                }
                
                for (int i = sort; i < count; i++)
                {
                    
                    targetWall = new Vector3(lastLeftWall.GetPositionVector3().x - _wallOffset, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    
                    if (targetWall != Vector3.zero)
                    {
                        AnimalListSorted[i].ChangePositionForRandomWallk(targetWall, radiusRandomWalkAnimal);
                    }
                }
            }
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

        private void UpdateArcherMoveAndShoot()
        {
            for (int i = 0; i < ArcherList.Count; i++)
            {
                _treeQuery.ArcherMove(_tree, (IKDTree)ArcherList[i]);
                _treeQuery.ShootClosest(_tree, (IKDTree)ArcherList[i], _arrowPool, _treeQuery);
            }
        }

        private void UpdateDamageClosest()
        {
            for (int i = 0; i < PointsInWorld.Count; i++)
            {
                if (PointsInWorld[i].HP > 0)
                    _treeQuery.DamageClosest(_tree, PointsInWorld[i]);
            }
        }

        private void UpdateEnimalsRunAway()
        {
           if(_HeroMove != null)
            _treeQuery.EnimalsRunAway(_tree, (IKDTree)_HeroMove);
        }
        public void SetSwordManToNewPosition()
        {
            Wall _lastRightWall = null;
            Wall _lastLeftWall = null;

            // Находим последнюю незавершенную стену справа и слева
            for (int i = 0; i < RightWall.Count; i++)
            {
                if (RightWall[i].isBuilded)
                {
                    _lastRightWall = RightWall[i];
                }
            }
            for (int i = 0; i < LeftWall.Count; i++)
            {
                if (LeftWall[i].isBuilded)
                {
                    _lastLeftWall = LeftWall[i];
                }
            }

            lastRightWall = _lastRightWall;
            lastLeftWall = _lastLeftWall;

            MoveSwordMans(_lastRightWall, _lastLeftWall);
            MoveArcherMans(_lastRightWall, _lastLeftWall);
        }
        private void MoveSwordMans(Wall lastRightWall, Wall lastLeftWall)
        {
            // Перемещаем SwordMan по последовательности стен
            for (int i = 0; i < SwordManList.Count; i++)
            {
                Vector3 targetWall = Vector3.zero;
                if (i % 2 == 0 && lastRightWall != null)  // Четные SwordMan передвигаются к lastRightWall, нечетные - к lastLeftWall
                {
                    targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetSwordMan, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                }
                else if (lastLeftWall != null)
                {
                    targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetSwordMan, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                }

                if (targetWall != Vector3.zero)
                {
                    SwordManList[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                }
            }

            // Последний SwordMan передвигается к lastLeftWall, если она есть
            if (SwordManList.Count > 0 && lastLeftWall != null)
            {
                Vector3 targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetSwordMan, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                SwordManList[SwordManList.Count - 1].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
            }
            else if (SwordManList.Count > 0 && lastRightWall != null)
            {
                Vector3 targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetSwordMan, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                SwordManList[SwordManList.Count - 1].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
            }
        }
        private void MoveArcherMans(Wall lastRightWall, Wall lastLeftWall)
        {
            // Перемещаем Archer по последовательности стен

            if (_gameCyrcle.ChekIfDay())
            {

                for (int i = 0; i < ArcherList.Count; i++)
                {
                    Vector3 targetWall = Vector3.zero;
                    if (i % 2 == 0 && lastRightWall != null)  // Четные Archer передвигаются к lastRightWall, нечетные - к lastLeftWall
                    {
                        targetWall = new Vector3(lastRightWall.GetPositionVector3().x + offsetArcherInday, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    }
                    else if (lastLeftWall != null)
                    {
                        targetWall = new Vector3(lastLeftWall.GetPositionVector3().x - offsetArcherInday, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    }

                    if (targetWall != Vector3.zero)
                    {
                        ArcherList[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInDay);
                    }
                }
                // Последний Archer передвигается к lastLeftWall, если она есть
                if (ArcherList.Count > 0 && lastLeftWall != null)
                {
                    Vector3 targetWall = new Vector3(lastLeftWall.GetPositionVector3().x - offsetArcherInday, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    ArcherList[ArcherList.Count - 1].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInDay);
                }
                else if (ArcherList.Count > 0 && lastRightWall != null)
                {
                    Vector3 targetWall = new Vector3(lastRightWall.GetPositionVector3().x + offsetArcherInday, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    ArcherList[ArcherList.Count - 1].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInDay);
                }
            }
            else
            {

                for (int i = 0; i < ArcherList.Count; i++)
                {
                    Vector3 targetWall = Vector3.zero;
                    if (i % 2 == 0 && lastRightWall != null)  // Четные Archer передвигаются к lastRightWall, нечетные - к lastLeftWall
                    {
                        targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetArcher, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    }
                    else if (lastLeftWall != null)
                    {
                        targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetArcher, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    }

                    if (targetWall != Vector3.zero)
                    {
                        ArcherList[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                    }
                }
                // Последний Archer передвигается к lastLeftWall, если она есть
                if (ArcherList.Count > 0 && lastLeftWall != null)
                {
                    Vector3 targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetArcher, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    ArcherList[ArcherList.Count - 1].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                }
                else if (ArcherList.Count > 0 && lastRightWall != null)
                {
                    Vector3 targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetArcher, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    ArcherList[ArcherList.Count - 1].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
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
        public void OnDayChange()
        {
             SetSwordManToNewPosition();
        }
        public Wall GetLastRightWall()
        {
            return lastRightWall;
        }
        public Wall GetLastLeftWall()
        {
            return lastLeftWall;
        }
    }
}
