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

        private const int _minDistance = 5;
        private const float _minimalDistanceToHero = 1f;
        private const float _heightPosition = 0.44f;

        public AIWarConnector(Pools pool, GameCyrcle cyrcle, ResourceManager resourceManager, AIConnector AIConnector, ArrowPool arrowPool)
        {
            PointsInWorld = new List<IKDTree>();

            SwordManList = new List<IWork>();
            ArcherList = new List<IWork>();
            EnemyList = new List<IEnemy>();
            

            _AIConnector = AIConnector;
            _resourceManager = resourceManager;
            _gameCyrcle = cyrcle;
            _pool = pool;
            _arrowPool = arrowPool;

            _tree = new KDTree();
            _treeQuery = new KDQuery();
        }

        public void StartMove(IKDTree enemy)
        {

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
                        List<int> index = new List<int>();
                        _treeQuery.ClosestPoint(_tree, (IKDTree)EnemyList[i], index);
                    if (index.Count != 0)
                    {
                        Vector3 target = new Vector3();

                        target.x = PointsInWorld[index[0]].GetPositionVector2().x;
                        target.z = PointsInWorld[index[0]].GetPositionVector2().y;
                        target.y = _heightPosition;

                        EnemyList[i].Move(target);
                    }
                }
                for (int i = 0; i < SwordManList.Count; i++)
                {
                    List<int> index = new List<int>();
                    _treeQuery.ClosestPoint(_tree, (IKDTree)SwordManList[i], index);
                    if (index.Count != 0)
                    {
                        Vector3 target = new Vector3();

                        target.x = PointsInWorld[index[0]].GetPositionVector2().x;
                        target.z = PointsInWorld[index[0]].GetPositionVector2().y;
                        target.y = _heightPosition;

                        SwordManList[i].Move(target, null, ItemsType.None);
                    }
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
                UnityEngine.Debug.Log(stopwatch.Elapsed.TotalMilliseconds.ToString("F2") + " microseconds for tree to build"); // выводим результат в микросекундах
                lastUpdateTime = Time.time;
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
        }
    }
}
