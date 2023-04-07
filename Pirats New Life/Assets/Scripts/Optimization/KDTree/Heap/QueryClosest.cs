using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Optimization.KDTree
{
    public partial class KDQuery
    {

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

            int smallestIndex = 0;
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

                        if(points[index].CheckIfEnemy() != _isEnemy)
                        {
                            sqrDist = Vector2.SqrMagnitude(points[index].GetPositionVector2() - queryPosition.GetPositionVector2());

                            if (sqrDist <= SSR)
                            {
                                SSR = sqrDist;
                                smallestIndex = index;
                            }
                        }
                    }

                }
            }

            resultIndices.Add(smallestIndex);

            if (resultDistances != null)
            {
                resultDistances.Add(SSR);
            }

        }

    }
}
