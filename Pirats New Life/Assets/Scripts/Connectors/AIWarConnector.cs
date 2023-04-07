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
        private float lastUpdateTime = 0.0f;
        private const float updateInterval = 0.5f;

        private const int _minDistance = 5;
        private const float _minimalDistanceToHero = 1f;
        private const float _heightPosition = 0.44f;

        public AIWarConnector(Pools pool, GameCyrcle cyrcle, ResourceManager resourceManager, AIConnector AIConnector)
        {
            PointsInWorld = new List<IKDTree>();

            SwordManList = new List<IWork>();
            ArcherList = new List<IWork>();
            EnemyList = new List<IEnemy>();
            

            _AIConnector = AIConnector;
            _resourceManager = resourceManager;
            _gameCyrcle = cyrcle;
            _pool = pool;

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

                _tree.Rebuild(3);
                foreach (var item in EnemyList)
                {
                    List<int> index = new List<int>();
                    _treeQuery.ClosestPoint(_tree, (IKDTree)item, index);
                    
                    Vector3 target = new Vector3();

                    target.x = PointsInWorld[index[0]].GetPositionVector2().x;
                    target.z = PointsInWorld[index[0]].GetPositionVector2().y;
                    target.y = _heightPosition;

                    item.Move(target, null);
                }
                
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
