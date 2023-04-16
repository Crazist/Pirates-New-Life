using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Optimization.KDTree
{
    public partial class KDQuery
    {
        private const float _earthY = 0.46f;
        public void ClosestPoint(KDTree tree, IKDTree queryPosition, List<int> resultIndices, List<float> resultDistances = null)
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
                        var attacker = points[index];

                        if (attacker.CheckIfEnemy() != _isEnemy && attacker.HP > 0)
                        {
                            sqrDist = Vector2.SqrMagnitude(attacker.GetPositionVector2() - queryPosition.GetPositionVector2());

                            if (sqrDist <= SSR)
                            {
                                SSR = sqrDist;
                                smallestIndex = index;
                            }
                        }
                    }

                }
            }

            if(smallestIndex != -1)
            resultIndices.Add(smallestIndex);

            if (resultDistances != null)
            {
                resultDistances.Add(SSR);
            }

        }
        public void DamageClosest(KDTree tree, IKDTree queryPosition, List<IKDTree> PointsInWorld, List<IEnemy> EnemyList)
        {

            Reset();

            var _isEnemy = queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            int[] permutation = tree.permutation;

            if (points.Length == 0)
            {
                return;
            }

            int smallestIndex = 0;
            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;
            float _minDistForDamage = 1f;
            float _minDistForDamageArrow = 2f;

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
                        var attacker = points[index];

                        if(queryPosition.HP == -1)
                        {
                            if (attacker.CheckIfEnemy() != _isEnemy && attacker.HP > 0)
                            {
                                sqrDist = Vector2.SqrMagnitude(attacker.GetPositionVector2() - queryPosition.GetPositionVector2());

                                if (sqrDist <= _minDistForDamageArrow)
                                {
                                    attacker.GetDamage(queryPosition.CountOFDamage());
                                    if (attacker.HP <= 0)
                                    {
                                        PointsInWorld.Remove(attacker);
                                        if (attacker.CheckIfEnemy())
                                        {
                                            EnemyList.Remove((IEnemy)attacker);
                                        }
                                        tree.Build(PointsInWorld, 3);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (attacker.CheckIfEnemy() != _isEnemy && attacker.CheckIfCanDamage() && attacker.HP > 0 && queryPosition.HP > 0)
                            {
                                sqrDist = Vector2.SqrMagnitude(attacker.GetPositionVector2() - queryPosition.GetPositionVector2());

                                if (sqrDist <= _minDistForDamage)
                                {
                                    attacker.Attack();
                                    queryPosition.GetDamage(attacker.CountOFDamage());
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
        }
        public void ShootClosest(KDTree tree, IKDTree queryPosition, List<IKDTree> PointsInWorld, List<IEnemy> EnemyList, ArrowPool _arrowPool, KDQuery _treeQuery)
        {

            Reset();

            var _isEnemy = queryPosition.CheckIfEnemy();
            IKDTree[] points = tree.points;
            int[] permutation = tree.permutation;

            if (points.Length == 0)
            {
                return;
            }

            int smallestIndex = 0;
            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;
            float _minDistForDamage = 400f;
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
                        var attacker = points[index];

                        if (attacker.CheckIfEnemy() != _isEnemy && queryPosition.CheckIfCanDamage() && attacker.HP > 0 && queryPosition.HP > 0)
                        {
                            sqrDist = Vector2.SqrMagnitude(attacker.GetPositionVector2() - queryPosition.GetPositionVector2());

                            if (sqrDist <= _minDistForDamage && sqrDist > _minCloseDistForDamage)
                            {
                                queryPosition.Attack();
                                
                                Vector3 endPosition = new Vector3(attacker.GetPositionVector2().x, _earthY, attacker.GetPositionVector2().y);

                                Vector3 startPosition = new Vector3(queryPosition.GetPositionVector2().x, _earthY, queryPosition.GetPositionVector2().y);

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
    }
}
