using GameInit.GameCyrcleModule;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Optimization.KDTree
{
    public partial class KDQuery
    {
        private GameCyrcle _GameCyrcle;
        private Transform[] _EnemySpawnerPsoition;
        private List<IKDTree> PointsInWorld;
        private List<IEnemy> EnemyList;

        private const float _offsetToAttackEnemy = 2.2f;
        private const float _spellOffset = 1.2f;
        private const float _heightPosition = 0.46f;
        private const float _minDistanceForEndSpell = 40;

        public void SetEnemySpawner(GameCyrcle _cyrcle, Transform[] EnemySpawnerPsoition, List<IKDTree> _PointsInWorld, List<IEnemy> _EnemyList)
        {
            PointsInWorld = _PointsInWorld;
            EnemyList = _EnemyList;
            _GameCyrcle = _cyrcle;
            _EnemySpawnerPsoition = EnemySpawnerPsoition;
        }
        public void EnemyRunForAttack(KDTree tree, IEnemy _queryPosition, float minDist = 0)
        {
            IKDTree queryPosition = (IKDTree)_queryPosition;

            Reset();

            var _isEnemy = queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            int[] permutation = tree.permutation;

            if (points.Length == 0)
            {
                return;
            }
            float _minDist = 0;
            if (minDist != 0)
            {
                _minDist = minDist;
            }
            int smallestIndex = -1;
            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;


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

                if (queryNode.distance > SSR)
                    continue;

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
                else
                {

                    float sqrDist;
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

                            for (int g = 0; g < _EnemySpawnerPsoition.Length; g++)
                            {
                                Vector2 pos1 = new Vector2(_enemy.GetAiComponent().GetTransform().position.x, _enemy.GetAiComponent().GetTransform().position.z);
                                Vector2 pos2 = new Vector2(_EnemySpawnerPsoition[g].position.x, _EnemySpawnerPsoition[g].position.z);
                                float distance = Vector2.SqrMagnitude(pos1 - pos2);

                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    nearestSpawner = _EnemySpawnerPsoition[g];
                                }
                            }

                            if (nearestSpawner != null)
                            {
                                Vector3 pos = new Vector3(nearestSpawner.position.x, _heightPosition, nearestSpawner.position.z);
                                _enemy.StopSpell();
                                _enemy.MoveToBase(pos, () => { _enemy.Disable(); 
                                        
                                        PointsInWorld.Remove((IKDTree)_enemy);
                                        EnemyList.Remove(_enemy);
                                        tree.Build(PointsInWorld, 3);
                                });
                            }
                        }
                    }
                }
            }
        }

        public void DamageClosest(KDTree tree, IKDTree queryPosition)
        {

            Reset();

            var _isEnemy = queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            int[] permutation = tree.permutation;

            if (points.Length == 0)
            {
                return;
            }

            int smallestIndex = -1;
            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;
            float _minDistForDamage = 5f;
            float _minDistForDamageArrow = 1.5f;

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

                if (queryNode.distance > SSR)
                    continue;

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
                else
                {
                    float sqrDist;
                    // LEAF
                    for (int i = node.start; i < node.end; i++)
                    {
                        int index = permutation[i];
                        var defender = points[index];

                        if (defender.CheckIfEnemy() != _isEnemy && queryPosition.CheckIfCanDamage() && defender.HP > 0 && queryPosition.HP > 0 || queryPosition.Type == EntityType.Arrow)
                        {
                            sqrDist = Vector2.SqrMagnitude(defender.GetPositionVector2() - queryPosition.GetPositionVector2());

                            float _distanceAttack = queryPosition.Type == EntityType.Arrow ? _minDistForDamageArrow : _minDistForDamage;

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
                                if (defender.CheckIfEnemy())
                                {
                                    EnemyList.Remove((IEnemy)defender);
                                }
                                tree.Build(PointsInWorld, 3);
                            }
                        }
                        else
                        {
                            queryPosition.Attack();
                            defender.GetDamage(queryPosition.CountOFDamage());
                            if (defender.HP <= 0 && defender.Type != EntityType.Wall)
                            {
                                PointsInWorld.Remove(defender);
                                if (defender.CheckIfEnemy())
                                {
                                    EnemyList.Remove((IEnemy)defender);
                                }
                                tree.Build(PointsInWorld, 3);
                            }
                        }
                    }
                }
            }
        }
        public void ShootClosest(KDTree tree, IKDTree queryPosition, ArrowPool _arrowPool, KDQuery _treeQuery)
        {

            Reset();

            var _isEnemy = queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            int[] permutation = tree.permutation;

            if (points.Length == 0)
            {
                return;
            }

            int smallestIndex = -1;
            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;
            float _minDistForDamage = 150f;
            float _minCloseDistForDamage = 50f;

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

                if (queryNode.distance > SSR)
                    continue;

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
                else
                {

                    float sqrDist;
                    // LEAF
                    for (int i = node.start; i < node.end; i++)
                    {
                        int index = permutation[i];
                        var defender = points[index];

                        if (defender.CheckIfEnemy() != _isEnemy && queryPosition.CheckIfCanDamage() && defender.HP > 0 && queryPosition.HP > 0)
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

                        arrow.Shoot(queryPosition, endPosition, tree, PointsInWorld, EnemyList, _treeQuery);

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
    }
}
