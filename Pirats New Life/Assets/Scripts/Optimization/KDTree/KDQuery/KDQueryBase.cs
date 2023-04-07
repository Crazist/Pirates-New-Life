using System;
using UnityEngine;
using GameInit.Optimization;

namespace GameInit.Optimization.KDTree
{
    public partial class KDQuery
    {

        protected KDQueryNode[] queueArray;  // queue array
        protected Heap.MinHeap<KDQueryNode> minHeap; //heap for k-nearest
        protected int count = 0;             // size of queue
        protected int queryIndex = 0;        // current index at stack

        /// <summary>
        /// Returns initialized node from stack that also acts as a pool
        /// The returned reference to node stays in stack
        /// </summary>
        /// <returns>Reference to pooled node</returns>
        private KDQueryNode PushGetQueue()
        {

            KDQueryNode node = null;

            if (count < queueArray.Length)
            {

                if (queueArray[count] == null)
                    queueArray[count] = node = new KDQueryNode();
                else
                    node = queueArray[count];
            }
            else
            {

                // automatic resize of pool
                Array.Resize(ref queueArray, queueArray.Length * 2);
                node = queueArray[count] = new KDQueryNode();
            }

            count++;

            return node;
        }

        protected void PushToQueue(KDNode node, IKDTree tempClosestPoint)
        {

            var queryNode = PushGetQueue();
            queryNode.node = node;
            queryNode.tempClosestPoint = tempClosestPoint.GetPositionVector2();
        }

        protected void PushToHeap(KDNode node, Vector2 tempClosestPoint, IKDTree queryPosition)
        {

            var queryNode = PushGetQueue();
            queryNode.node = node;
            queryNode.tempClosestPoint = tempClosestPoint;

            float sqrDist = Vector2.SqrMagnitude(tempClosestPoint - queryPosition.GetPositionVector2());
            queryNode.distance = sqrDist;
            minHeap.PushObj(queryNode, sqrDist);
        }

        protected int LeftToProcess
        {

            get
            {
                return count - queryIndex;
            }
        }

        // just gets unprocessed node from stack
        // increases queryIndex
        protected KDQueryNode PopFromQueue()
        {

            var node = queueArray[queryIndex];
            queryIndex++;

            return node;
        }

        protected KDQueryNode PopFromHeap()
        {

            KDQueryNode heapNode = minHeap.PopObj();

            queueArray[queryIndex] = heapNode;
            queryIndex++;

            return heapNode;
        }

        protected void Reset()
        {

            count = 0;
            queryIndex = 0;
            minHeap.Clear();
        }

        public KDQuery(int queryNodesContainersInitialSize = 2048)
        {
            queueArray = new KDQueryNode[queryNodesContainersInitialSize];
            minHeap = new Heap.MinHeap<KDQueryNode>(queryNodesContainersInitialSize);
        }

    }

}