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
        public List<IKDTree> SwordManList { get; set; }
        public List<IKDTree> ArcherList { get; set; }
        public List<IKDTree> EnemyList { get; set; }
        public List<IKDTree> Buildings { get; set; }
        public List<IKDTree> Workers { get; set; }

        public List<List<IKDTree>> ListOfLists { get; set; }

        private Pools _pool;
        private HeroComponent _heroComponent;
        private List<Action> lateMove;
        private GameCyrcle _gameCyrcle;
        private BuildingsBuilder _buildingsBuilder;
        private ResourceManager _resourceManager;
        private AIConnector _AIConnector;
        private KDTree _tree;

        private const int _minDistance = 5;
        private const float _minimalDistanceToHero = 1f;

        public AIWarConnector(Pools pool, GameCyrcle cyrcle, ResourceManager resourceManager, AIConnector AIConnector)
        {
            ListOfLists = new List<List<IKDTree>>();

            ListOfLists.Add(SwordManList = new List<IKDTree>());
            ListOfLists.Add(ArcherList = new List<IKDTree>());
            ListOfLists.Add(EnemyList = new List<IKDTree>());
            ListOfLists.Add(Buildings = new List<IKDTree>());
            Workers = new List<IKDTree>();

            _AIConnector = AIConnector;
            _resourceManager = resourceManager;
            _gameCyrcle = cyrcle;
            _pool = pool;

            _tree = new KDTree();
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
