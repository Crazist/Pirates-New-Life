using GameInit.AI;
using GameInit.Enemy;
using GameInit.GameCyrcleModule;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameInit.Optimization.KDTree
{
    public partial class KDQuery
    {
        private GameCyrcle _GameCyrcle;
        private List<EnemySpawnComponent> _EnemySpawnComponents;
        private List<IKDTree> PointsInWorld;
        private List<IEnemy> EnemyList;
        private List<IAnimal> AnimalList;

        private const float _offsetToAttackEnemy = 2.2f;
        private const float _spellOffset = 1.2f;
        private const float _heightPosition = 0.46f;
        private const float _minDistanceForEndSpell = 40;

        public void SetEnemySpawner(GameCyrcle _cyrcle, List<EnemySpawnComponent> EnemySpawnComponents, List<IKDTree> _PointsInWorld, List<IEnemy> _EnemyList, List<IAnimal> _AnimalList)
        {
            PointsInWorld = _PointsInWorld;
            EnemyList = _EnemyList;
            AnimalList = _AnimalList;
            _GameCyrcle = _cyrcle;
            _EnemySpawnComponents = EnemySpawnComponents;
        }

        public KDNode FindClosestNode(KDTree tree, IKDTree queryPosition)
        {
            Reset();

            var _isEnemy = queryPosition.CheckIfEnemy();
            /// Smallest Squared Radius

            var rootNode = tree.rootNode;

            Vector2 rootClosestPoint = rootNode.bounds.ClosestPoint(queryPosition);

            PushToHeap(rootNode, rootClosestPoint, queryPosition);

            KDQueryNode queryNode = null;
            KDNode node = null;

            int partitionAxis;
            float partitionCoord;

            Vector3 tempClosestPoint;

            // searching
            while (minHeap.Count > 0)
            {

                queryNode = PopFromHeap();

                node = queryNode.node;

                if (!node.isLeaf && queryNode.obj.CheckIfEnemy() != _isEnemy)
                {

                    partitionAxis = node.partitionAxis;
                    partitionCoord = node.partitionCoord;

                    tempClosestPoint = queryNode.tempClosestPoint;

                    if ((tempClosestPoint[partitionAxis] - partitionCoord) < 0)
                    {

                        // we already know we are on the side of negative bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        PushToHeap(node.negativeChild, tempClosestPoint, queryPosition);
                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        if (node.positiveChild.Count != 0)
                        {

                            PushToHeap(node.positiveChild, tempClosestPoint, queryPosition);
                        }

                    }
                    else
                    {

                        // we already know we are on the side of positive bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        PushToHeap(node.positiveChild, tempClosestPoint, queryPosition);
                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        if (node.positiveChild.Count != 0)
                        {

                            PushToHeap(node.negativeChild, tempClosestPoint, queryPosition);
                        }

                    }
                }
            }
            return node;
        }

        public void EnemyRunForAttack(KDTree tree, IEnemy _queryPosition, float minDist = 0)
        {
            IKDTree queryPosition = (IKDTree)_queryPosition;

            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;
            float _minDist = 0;

            float sqrDist;
            int[] permutation = tree.permutation;
            var _isEnemy = queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            int smallestIndex = -1;

            var rootNode = tree.rootNode;

            KDNode node = FindClosestNode(tree, queryPosition);

            // LEAF
            for (int i = node.start; i < node.end; i++)
            {
                int index = permutation[i];
                var defender = points[index];

                if (defender.CheckIfEnemy() != _isEnemy && defender.HP > 0 && queryPosition.CheckIfCanDamage() && queryPosition.HP > 0)
                {
                    sqrDist = Vector2.SqrMagnitude(defender.GetPositionVector2() - queryPosition.GetPositionVector2());

                    if (sqrDist <= SSR)
                    {
                        SSR = sqrDist;
                        smallestIndex = index;
                    }
                }
            }

            if (smallestIndex != -1)
            {
                var _defender = points[smallestIndex];

                IEnemy _enemy = (IEnemy)queryPosition;
                _minDist = _enemy.DistanceForStartSpell;

                if (!_GameCyrcle.ChekIfDay())
                {
                    if (_minDist != 0 && (SSR >= _minDist || SSR <= _minDistanceForEndSpell) && _defender.Type != EntityType.Wall && !_enemy.RefreshSkill)
                    {
                        Vector3 target = new Vector3();

                        target.x = _defender.GetPositionVector2().x;
                        target.z = _defender.GetPositionVector2().y;
                        target.y = _heightPosition;

                        _enemy.StopSpell();
                        _enemy.Move(target);
                    }
                    else if (_minDist != 0 && SSR >= _minDist && !_enemy.RefreshSkill && _defender.Type == EntityType.Wall)
                    {
                        Vector3 target = new Vector3();

                        if (_defender.GetPositionVector2().x > 0)
                        {
                            target.x = _defender.GetPositionVector2().x + _offsetToAttackEnemy;
                            target.z = _defender.GetPositionVector2().y;
                        }
                        else if (_defender.GetPositionVector2().x < 0)
                        {
                            target.x = _defender.GetPositionVector2().x - _offsetToAttackEnemy;
                            target.z = _defender.GetPositionVector2().y;
                        }

                        target.y = _heightPosition;

                        _enemy.StopSpell();
                        _enemy.Move(target);
                    }
                    else if (_minDist != 0 && SSR < _minDist && !_enemy.RefreshSkill && _defender.Type == EntityType.Wall)
                    {
                        Vector3 target = new Vector3();

                        if (_defender.GetPositionVector2().x > 0)
                        {
                            target.x = _defender.GetPositionVector2().x + _offsetToAttackEnemy;
                            target.z = _defender.GetPositionVector2().y;
                        }
                        else if (_defender.GetPositionVector2().x < 0)
                        {
                            target.x = _defender.GetPositionVector2().x - _offsetToAttackEnemy;
                            target.z = _defender.GetPositionVector2().y;
                        }

                        target.y = _heightPosition;

                        _enemy.UseSpell(target, true);
                    }
                    else if (!_enemy.RefreshSkill)
                    {
                        Vector3 target = new Vector3();

                        if (_defender.GetPositionVector2().x > 0)
                        {
                            target.x = _defender.GetPositionVector2().x + _spellOffset;
                        }
                        else
                        {
                            target.x = _defender.GetPositionVector2().x - _spellOffset;
                        }

                        target.z = _defender.GetPositionVector2().y;
                        target.y = _heightPosition;

                        _enemy.UseSpell(target, false);
                    }
                }
                else
                {
                    float minDistance = Mathf.Infinity;
                    Transform nearestSpawner = null;

                    for (int g = 0; g < _EnemySpawnComponents.Count; g++)
                    {
                        Vector2 pos1 = new Vector2(_enemy.GetAiComponent().GetTransform().position.x, _enemy.GetAiComponent().GetTransform().position.z);
                        Vector2 pos2 = new Vector2(_EnemySpawnComponents[g].transform.position.x, _EnemySpawnComponents[g].transform.position.z);
                        float distance = Vector2.SqrMagnitude(pos1 - pos2);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestSpawner = _EnemySpawnComponents[g].transform;
                        }
                    }

                    if (nearestSpawner != null)
                    {
                        Vector3 pos = new Vector3(nearestSpawner.position.x, _heightPosition, nearestSpawner.position.z);
                        _enemy.StopSpell();
                        _enemy.MoveToBase(pos, () =>
                        {
                            _enemy.Disable();

                            PointsInWorld.Remove((IKDTree)_enemy);
                            EnemyList.Remove(_enemy);
                            tree.Build(PointsInWorld, 3);
                        });
                    }
                }
            }
        }
        public void ShildAllyRunForAttack(KDTree tree, IKDTree _queryPosition, float minDist = 0)
        {

            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;
            float _minDist = 100;

            float sqrDist;
            int[] permutation = tree.permutation;
            var _isEnemy = _queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            int smallestIndex = -1;

            var rootNode = tree.rootNode;

            KDNode node = FindClosestNode(tree, _queryPosition);

            // LEAF
            for (int i = node.start; i < node.end; i++)
            {
                int index = permutation[i];
                var defender = points[index];

                if ((defender.CheckIfEnemy() != _isEnemy || defender.Type == EntityType.Wall) && defender.HP > 0 && _queryPosition.CheckIfCanDamage() && _queryPosition.HP > 0 && defender.Type != EntityType.Animals && defender.Type != EntityType.EnemySpawner)
                {
                    sqrDist = Vector2.SqrMagnitude(defender.GetPositionVector2() - _queryPosition.GetPositionVector2());

                    if (sqrDist <= SSR && sqrDist < _minDist)
                    {
                        SSR = sqrDist;
                        smallestIndex = index;
                    }
                }
            }

            if (smallestIndex != -1)
            {
                var _defender = points[smallestIndex];

                if(_defender.Type != EntityType.Wall)
                {
                    IWork _ally = (IWork)_queryPosition;

                    Vector3 target = new Vector3();

                    target.x = _defender.GetPositionVector2().x;
                    target.z = _defender.GetPositionVector2().y;
                    target.y = _heightPosition;

                    _ally.Move(target, null);
                }
            }
        }
        public void ArcherMove(KDTree tree, IKDTree queryPosition)
        {

            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;
            float _minDist = 200;

            float offset = 20;
            float _minDistForMoveClose = 200;
            float _minDistForMoveFar = 50;

            float sqrDist;
            int[] permutation = tree.permutation;
            var _isEnemy = queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            int smallestIndex = -1;

            var rootNode = tree.rootNode;

            KDNode node = FindClosestNode(tree, queryPosition);

            // LEAF
            for (int i = node.start; i < node.end; i++)
            {
                int index = permutation[i];
                var defender = points[index];

                var archer = (IWork)queryPosition;

                if (defender.CheckIfEnemy() != _isEnemy && queryPosition.CheckIfCanDamage() && defender.HP > 0 && queryPosition.HP > 0 && defender.Type == EntityType.Animals && !archer.InMove)
                {
                    sqrDist = Vector2.SqrMagnitude(defender.GetPositionVector2() - queryPosition.GetPositionVector2());

                    if (sqrDist <= SSR && sqrDist < _minDist)
                    {
                        SSR = sqrDist;
                        smallestIndex = index;
                    }
                }
            }

            if (smallestIndex != -1)
            {
                var _defender = points[smallestIndex];
                var archer = (IWork)queryPosition;

                if (_GameCyrcle.ChekIfDay())
                {
                    if (SSR >= _minDistForMoveClose)
                    {
                        // Get current player position
                        Vector2 playerPos = new Vector2(archer.getTransform().position.x, archer.getTransform().position.z);

                        // Determine direction to move
                        int direction = _defender.GetPositionVector2().x > 0 ? -1 : 1;

                        // Calculate new position
                        Vector2 targetPos = new Vector2(_defender.GetPositionVector2().x + direction * offset, _defender.GetPositionVector2().y);

                        // Check if target position is on NavMesh
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(new Vector3(targetPos.x, _heightPosition, targetPos.y), out hit, 1f, NavMesh.AllAreas))
                        {
                            // Set target position to NavMesh position
                            Vector3 target = hit.position;

                            archer.Move(target, null);
                        }
                    }
                    else if (SSR < _minDistForMoveFar)
                    {
                        Vector2 _archer = new Vector2(archer.getTransform().position.x, archer.getTransform().position.z);
                        Vector2 enemyPos = _defender.GetPositionVector2();

                        Vector2 direction = enemyPos - _archer;

                        Vector2 targetPos = enemyPos - direction.normalized * offset; // отходим на 5 единиц

                        Vector3 _target = new Vector3(targetPos.x, _heightPosition, targetPos.y);

                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(new Vector3(_target.x, _heightPosition, _target.y), out hit, 1f, NavMesh.AllAreas))
                        {
                            // Set target position to NavMesh position
                            Vector3 target = hit.position;

                            archer.Move(target, null);
                        }
                    }
                }
            }
        }
        public void EnimalsRunAway(KDTree tree, IKDTree hero)
        {

            /// Smallest Squared Radius
            float _minDistForRun = 50;

            float offset = 5;

            float sqrDist = Single.PositiveInfinity;
            int[] permutation = tree.permutation;
            var _isEnemy = hero.CheckIfEnemy();
            IKDTree[] points = tree.points;

            KDNode node = FindClosestNode(tree, hero);

            for (int i = node.start; i < node.end; i++)
            {
                int index = permutation[i];
                var defender = points[index];

                if (defender.Type == EntityType.Animals)
                {
                    IAnimal animal = (IAnimal)defender;

                    if (defender.CheckIfEnemy() != _isEnemy && defender.HP > 0)
                    {
                        sqrDist = Vector2.SqrMagnitude(defender.GetPositionVector2() - hero.GetPositionVector2());

                        if (sqrDist < _minDistForRun)
                        {
                            Vector2 playerPos = hero.GetPositionVector2();
                            Vector2 enemyPos = defender.GetPositionVector2();

                            Vector2 direction = enemyPos - playerPos;

                            Vector2 targetPos = enemyPos + direction.normalized * offset; // двигаемся в противоположном направлении

                            Vector3 target = new Vector3(targetPos.x, _heightPosition, targetPos.y);

                            NavMeshHit hit;
                            if (NavMesh.SamplePosition(target, out hit, 1f, NavMesh.AllAreas))
                            {
                                // Set target position to NavMesh position
                                Vector3 navMeshTarget = hit.position;

                                var _defender = (IAnimal)defender;

                                _defender.Move(navMeshTarget);
                            }
                        }
                    }
                }
            }
        }
        public void DamageClosest(KDTree tree, IKDTree queryPosition)
        {
            float sqrDist;
            int[] permutation = tree.permutation;
            var _isEnemy = queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            int smallestIndex = -1;

            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;
            float _minDistForDamage = 3f;
            float _minDistForDamageWall = 10f;
            float _minDistForSpawner = 1000f;
            float _minDistForDamageArrow = 1.5f;

            var rootNode = tree.rootNode;

            KDNode node = FindClosestNode(tree, queryPosition);


            // LEAF
            for (int i = node.start; i < node.end; i++)
            {
                int index = permutation[i];
                var defender = points[index];

                if (defender.CheckIfEnemy() != _isEnemy && queryPosition.CheckIfCanDamage() && defender.HP > 0 && (queryPosition.HP > 0)  || (queryPosition.Type == EntityType.Arrow && defender.Type != EntityType.Wall))
                {
                    sqrDist = Vector2.SqrMagnitude(defender.GetPositionVector2() - queryPosition.GetPositionVector2());

                    float _distanceAttack = queryPosition.Type == EntityType.Arrow ? _minDistForDamageArrow : _minDistForDamage;
                    _distanceAttack = defender.Type == EntityType.Wall ? _minDistForDamageWall : _distanceAttack;
                    _distanceAttack = queryPosition.Type == EntityType.EnemySpawner ? _minDistForSpawner : _distanceAttack;

                    if (sqrDist <= SSR && sqrDist <= _distanceAttack)
                    {
                        SSR = sqrDist;
                        smallestIndex = index;
                    }
                }
            }

            if (smallestIndex != -1)
            {
                var defender = points[smallestIndex];

                if (queryPosition.HP == -1)
                {
                    defender.GetDamage(queryPosition.CountOFDamage());
                    if (defender.HP <= 0)
                    {
                        PointsInWorld.Remove(defender);
                        if (defender.CheckIfEnemy() && defender.Type == EntityType.Enemy)
                        {
                            EnemyList.Remove((IEnemy)defender);
                        }
                        else if (defender.CheckIfEnemy() && defender.Type == EntityType.Animals)
                        {
                            AnimalList.Remove((IAnimal)defender);
                        }
                        tree.Build(PointsInWorld, 3);
                    }
                }
                else
                {
                    queryPosition.Attack();

                    if(queryPosition.CountOFDamage() > 0)
                    {
                        defender.GetDamage(queryPosition.CountOFDamage());
                    }
                    
                    if (defender.HP <= 0 && defender.Type != EntityType.Wall && defender.Type != EntityType.Animals)
                    {
                        PointsInWorld.Remove(defender);
                        if (defender.CheckIfEnemy() && defender.Type != EntityType.EnemySpawner)
                        {
                            EnemyList.Remove((IEnemy)defender);
                        }
                        tree.Build(PointsInWorld, 3);
                    }
                }
            }
        }
        public void ShootClosest(KDTree tree, IKDTree queryPosition, ArrowPool _arrowPool, KDQuery _treeQuery)
        {
            float sqrDist;
            int[] permutation = tree.permutation;
            var _isEnemy = queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            float SSR = Single.PositiveInfinity;
            float _minDistForDamageTower = 350f;
            float _minDistForDamage = 150f;
            float _minCloseDistForDamage = 50f;
            int smallestIndex = -1;

            KDNode node = FindClosestNode(tree, queryPosition);
            // LEAF
            for (int i = node.start; i < node.end; i++)
            {
                int index = permutation[i];
                var defender = points[index];

                if (defender.CheckIfEnemy() != _isEnemy && queryPosition.CheckIfCanDamage() && defender.HP > 0 && queryPosition.Type == EntityType.Tower && defender.Type != EntityType.Animals)
                {
                    sqrDist = Vector2.SqrMagnitude(defender.GetPositionVector2() - queryPosition.GetPositionVector2());

                    if (sqrDist <= SSR && sqrDist <= _minDistForDamageTower && sqrDist > _minCloseDistForDamage)
                    {
                        SSR = sqrDist;
                        smallestIndex = index;
                    }
                }
                else if (defender.CheckIfEnemy() != _isEnemy && queryPosition.CheckIfCanDamage() && defender.HP > 0 && queryPosition.HP > 0)
                {
                    sqrDist = Vector2.SqrMagnitude(defender.GetPositionVector2() - queryPosition.GetPositionVector2());

                    if (sqrDist <= SSR && sqrDist <= _minDistForDamage && sqrDist > _minCloseDistForDamage)
                    {
                        SSR = sqrDist;
                        smallestIndex = index;
                    }
                }
            }

            if (smallestIndex != -1)
            {
                var defender = points[smallestIndex];
                queryPosition.Attack();

                Vector3 endPosition = new Vector3(defender.GetPositionVector2().x, _heightPosition, defender.GetPositionVector2().y);

                Vector3 startPosition = new Vector3(queryPosition.GetPositionVector2().x, _heightPosition, queryPosition.GetPositionVector2().y);

                var arrow = _arrowPool.GetFreeElements(startPosition);

                if (queryPosition.Type == EntityType.Tower)
                {
                    arrow.ShootTower(queryPosition, endPosition, tree, PointsInWorld, EnemyList, _treeQuery);
                }
                else
                {
                    arrow.Shoot(queryPosition, endPosition, tree, PointsInWorld, EnemyList, _treeQuery);
                }

                if (queryPosition.HP <= 0)
                {
                    PointsInWorld.Remove(queryPosition);
                    if (queryPosition.CheckIfEnemy())
                    {
                        EnemyList.Remove((IEnemy)queryPosition);
                    }
                    tree.Build(PointsInWorld, 3);
                }
            }
        }
    }
}

