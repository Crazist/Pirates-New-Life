using System.Collections;
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

namespace GameInit.Connector
{
    public class AIWarConnector : IUpdate, IDayChange
    {
        public List<IWork> SwordManList { get; set; }
        public List<IWork> ArcherList { get; set; }
        public List<IEnemy> EnemyList { get; set; }
        public List<Wall> RightWall { get; set; }
        public List<Wall> LeftWall { get; set; }

        public List<IKDTree> PointsInWorld { get; set; }

        private Pools _pool;
        private HeroComponent _heroComponent;
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
        
        private const float radiusRandomWalk = 1.5f;
        private const int offset = 4;
        public AIWarConnector(Pools pool, GameCyrcle cyrcle, ResourceManager resourceManager, AIConnector AIConnector, ArrowPool arrowPool)
        {
            PointsInWorld = new List<IKDTree>();

            SwordManList = new List<IWork>();
            ArcherList = new List<IWork>();
            EnemyList = new List<IEnemy>();

            RightWall = new List<Wall>();
            LeftWall = new List<Wall>();


            _AIConnector = AIConnector;
            _resourceManager = resourceManager;
            _gameCyrcle = cyrcle;
            _pool = pool;
            _arrowPool = arrowPool;

            _tree = new KDTree();
            _treeQuery = new KDQuery();
        }

        public void GetHeroComponent(HeroComponent heroComponent)
        {
            _heroComponent = heroComponent;
        }
        private bool isFirst = true;
        public void OnUpdate()
        {
            if (Time.time - lastUpdateTime > updateInterval && PointsInWorld.Count != 0)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                _tree.Rebuild(3);
               
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    _treeQuery.EnemyRunForAttack(_tree, EnemyList[i]);
                }
                
                for (int i = 0; i < ArcherList.Count; i++)
                {
                    _treeQuery.ShootClosest(_tree, (IKDTree)ArcherList[i], PointsInWorld, EnemyList, _arrowPool, _treeQuery);
                }
               
                for (int i = 0; i < PointsInWorld.Count; i++)
                {
                    if (PointsInWorld[i].HP > 0)
                        _treeQuery.DamageClosest(_tree, PointsInWorld[i], PointsInWorld, EnemyList);
                }

                stopwatch.Stop(); // останавливаем таймер
                UnityEngine.Debug.Log(stopwatch.Elapsed.TotalMilliseconds.ToString("F2") + " microseconds for tree to build"); // выводим результат в милисекунд
                lastUpdateTime = Time.time;
            }
           
        }
        public void SetSwordManToNewPosition()
        {
            Wall lastRightWall = null;
            Wall lastLeftWall = null;

            // Находим последнюю незавершенную стену справа и слева
            for (int i = 0; i < RightWall.Count; i++)
            {
                if (RightWall[i].isBuilded)
                {
                    lastRightWall = RightWall[i];
                }
            }
            for (int i = 0; i < LeftWall.Count; i++)
            {
                if (LeftWall[i].isBuilded)
                {
                    lastLeftWall = LeftWall[i];
                }
            }

            // Перемещаем SwordMan по последовательности стен
            for (int i = 0; i < SwordManList.Count; i++)
            {
                Vector3 targetWall = Vector3.zero;
                if (i % 2 == 0 && lastRightWall != null)  // Четные SwordMan передвигаются к lastRightWall, нечетные - к lastLeftWall
                {
                    targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offset, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                }
                else if(lastLeftWall != null)
                {
                    targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offset, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                }

                if (targetWall != Vector3.zero)
                {
                    SwordManList[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalk);
                }
            }

            // Последний SwordMan передвигается к lastLeftWall, если она есть
            if (SwordManList.Count > 0 && lastLeftWall != null)
            {
                Vector3 targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offset, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                SwordManList[SwordManList.Count - 1].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalk);
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
    }
}
