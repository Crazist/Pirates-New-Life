using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Optimization.KDTree 
{
    public class KDTree
    {
        public KDNode rootNode { get; set; }

        public IKDTree[] points { get { return _points; } }
        private IKDTree[] _points;

        public int[] permutation { get { return _permutation; } }
        private int[] _permutation;

        public int Count { get; private set; }

        private int _maxPointsPerLeafNoded = 5;

        private KDNode[] _nodesStack;
        private int _nodesCount = 0;

        public KDTree(int maxPointPerLeaf = 5)
        {
            Count = 0;
            _points = new IKDTree[0];
            _permutation = new int[0];

            _nodesStack = new KDNode[30];

            _maxPointsPerLeafNoded = maxPointPerLeaf;

        }
        public KDTree(IKDTree[] items, int maxPointPerLeaf = 5)
        {
            Count = items.Length;

            _points = items;
            _permutation = new int[points.Length];

            _nodesStack = new KDNode[30];

            _maxPointsPerLeafNoded = maxPointPerLeaf;

            Rebuild();
        }

        public void Build(IKDTree[] items, int maxPointPerLeaf = -1)
        {
            SetCount(items.Length);

            for (int i = 0; i < Count; i++)
            {
                points[i] = items[i];
            }

            Rebuild(maxPointPerLeaf);
        }
        public void Build(List<IKDTree> items, int maxPointPerLeaf = -1)
        {
            SetCount(items.Count);

            for (int i = 0; i < Count; i++)
            {
                points[i] = items[i];
            }

            Rebuild(maxPointPerLeaf);
        }
        public void Rebuild(int maxPointsPerLeaf = -1)
        {
            for (int i = 0; i < Count; i++)
            {
                permutation[i] = i;
            }

            if (_maxPointsPerLeafNoded > 0)
            {
                _maxPointsPerLeafNoded = maxPointsPerLeaf;
            }

            BuildTree();
        }
        public void SetCount(int newSize)
        {

            Count = newSize;
            // upsize internal arrays
            if (Count > points.Length)
            {

                Array.Resize(ref _points, Count);
                Array.Resize(ref _permutation, Count);
            }
        }
        private void BuildTree()
        {

            ResetKDNodeStack();

            rootNode = GetKDNode();
            rootNode.bounds = MakeBounds();
            rootNode.start = 0;
            rootNode.end = Count;

            SplitNode(rootNode);
        }
        private KDNode GetKDNode()
        {

            KDNode node = null;

            if (_nodesCount < _nodesStack.Length)
            {

                if (_nodesStack[_nodesCount] == null)
                {
                    _nodesStack[_nodesCount] = node = new KDNode();
                }
                else
                {
                    node = _nodesStack[_nodesCount];
                    node.Clear();
                    node.partitionAxis = -1;
                }
            }
            else
            {

                // automatic resize of KDNode pool array
                Array.Resize(ref _nodesStack, _nodesStack.Length * 2);
                node = _nodesStack[_nodesCount] = new KDNode();
            }

            _nodesCount++;

            return node;
        }
        private void ResetKDNodeStack()
        {
            _nodesCount = 0;
        }

        private KDBounds MakeBounds()
        {

            Vector2 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector2 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            int even = Count & ~1; // calculate even Length

            // min, max calculations
            // 3n/2 calculations instead of 2n
            for (int i0 = 0; i0 < even; i0 += 2)
            {

                int i1 = i0 + 1;

                var x0 = points[i0].GetPositionVector2().x;
                var y0 = points[i0].GetPositionVector2().y;

                var x1 = points[i1].GetPositionVector2().x;
                var y1 = points[i1].GetPositionVector2().y;


                // X Coords
                if (x0 > x1)
                {
                    // i0 is bigger, i1 is smaller
                    if (x1 < min.x)
                        min.x = x1;

                    if (x0 > max.x)
                        max.x = x0;
                }
                else
                {
                    // i1 is smaller, i0 is bigger
                    if (x0 < min.x)
                        min.x = x0;

                    if (x1 > max.x)
                        max.x = x1;
                }

                if (y0 > y1)
                {
                    // i0 is bigger, i1 is smaller
                    if (y1 < min.y)
                        min.y = y1;

                    if (y0 > max.y)
                        max.y = y0;
                }
                else
                {
                    // i1 is smaller, i0 is bigger
                    if (y0 < min.y)
                        min.y = y0;

                    if (y1 > max.y)
                        max.y = y1;
                }
            }

            // if array was odd, calculate also min/max for the last element
            if (even != Count)
            {
                var x = points[even].GetPositionVector2().x;
                var y = points[even].GetPositionVector2().y;
                // X
                if (min.x > x)
                    min.x = x;

                if (max.x < x)
                    max.x = x;
                // Z
                if (min.y > y)
                    min.y = y;

                if (max.y < y)
                    max.y = y;
            }

            KDBounds b = new KDBounds();
            b.min = min;
            b.max = max;

            return b;
        }

        private void SplitNode(KDNode parent)
        {

            // center of bounding box
            KDBounds parentBounds = parent.bounds;
            Vector2 parentBoundsSize = parentBounds.size;

            // Find axis where bounds are largest
            int splitAxis = 0;
            float axisSize = parentBoundsSize.x;

            if (axisSize < parentBoundsSize.y)
            {
                splitAxis = 1;
                axisSize = parentBoundsSize.y;
            }


            // Our axis min-max bounds
            float boundsStart = parentBounds.min[splitAxis];
            float boundsEnd = parentBounds.max[splitAxis];

            // Calculate the spliting coords
            float splitPivot = CalculatePivot(parent.start, parent.end, boundsStart, boundsEnd, splitAxis);

            parent.partitionAxis = splitAxis;
            parent.partitionCoord = splitPivot;

            // 'Spliting' array to two subarrays
            int splittingIndex = Partition(parent.start, parent.end, splitPivot, splitAxis);

            // Negative / Left node
            Vector3 negMax = parentBounds.max;
            negMax[splitAxis] = splitPivot;

            KDNode negNode = GetKDNode();
            negNode.bounds = parentBounds;
            negNode.bounds.max = negMax;
            negNode.start = parent.start;
            negNode.end = splittingIndex;
            parent.negativeChild = negNode;

            // Positive / Right node
            Vector3 posMin = parentBounds.min;
            posMin[splitAxis] = splitPivot;

            KDNode posNode = GetKDNode();
            posNode.bounds = parentBounds;
            posNode.bounds.min = posMin;
            posNode.start = splittingIndex;
            posNode.end = parent.end;
            parent.positiveChild = posNode;

            // check if we are actually splitting it anything
            // this if check enables duplicate coordinates, but makes construction a bit slower
#if KDTREE_DUPLICATES
            if(negNode.Count != 0 && posNode.Count != 0) {
#endif
            // Constraint function deciding if split should be continued
            if (ContinueSplit(negNode))
                SplitNode(negNode);


            if (ContinueSplit(posNode))
                SplitNode(posNode);

#if KDTREE_DUPLICATES
            }
#endif
        }
        private float CalculatePivot(int start, int end, float boundsStart, float boundsEnd, int axis)
        {

            //! sliding midpoint rule
            float midPoint = (boundsStart + boundsEnd) / 2f;

            bool negative = false;
            bool positive = false;

            float negMax = Single.MinValue;
            float posMin = Single.MaxValue;

            // this for loop section is used both for sorted and unsorted data
            for (int i = start; i < end; i++)
            {

                if (_points[permutation[i]].GetPositionVector2()[axis] < midPoint)
                    negative = true;
                else
                    positive = true;

                if (negative == true && positive == true)
                    return midPoint;
            }

            if (negative)
            {

                for (int i = start; i < end; i++)
                    if (negMax < _points[permutation[i]].GetPositionVector2()[axis])
                        negMax = _points[permutation[i]].GetPositionVector2()[axis];

                return negMax;
            }
            else
            {

                for (int i = start; i < end; i++)
                    if (posMin > _points[permutation[i]].GetPositionVector2()[axis])
                        posMin = _points[permutation[i]].GetPositionVector2()[axis];

                return posMin;
            }
        }
        private int Partition(int start, int end, float partitionPivot, int axis)
        {

            // note: increasing right pointer is actually decreasing!
            int LP = start - 1; // left pointer (negative side)
            int RP = end;       // right pointer (positive side)

            int temp;           // temporary var for swapping permutation indexes

            while (true)
            {

                do
                {
                    // move from left to the right until "out of bounds" value is found
                    LP++;
                }
                while (LP < RP && _points[permutation[LP]].GetPositionVector2()[axis] < partitionPivot);

                do
                {
                    // move from right to the left until "out of bounds" value found
                    RP--;
                }
                while (LP < RP && _points[permutation[RP]].GetPositionVector2()[axis] >= partitionPivot);

                if (LP < RP)
                {
                    // swap
                    temp = permutation[LP];
                    permutation[LP] = permutation[RP];
                    permutation[RP] = temp;
                }
                else
                {

                    return LP;
                }
            }
        }
        private bool ContinueSplit(KDNode node)
        {
            return (node.Count > _maxPointsPerLeafNoded);
        }

        private const float _defY = 0.44f;
        public void DrawNode(KDNode node)
        {
            if (node == null) return;
            if (node.isLeaf)
            {
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.color = Color.white;
            }

            var _min = node.bounds.min;
            var _max = node.bounds.max;
            var size = new Vector3(_max.x - _min.x, 0, _max.y - _min.y);

            // Calculate the center position of the square
            var center = new Vector3((_min.x + _max.x) / 2f, _defY, (_min.y + _max.y) / 2f);

            // Draw the wireframe square at the center position
            Gizmos.DrawWireCube(center, size);

            if (node.negativeChild != null)
            {
                DrawNode(node.negativeChild);
            }

            if (node.positiveChild != null)
            {
                DrawNode(node.positiveChild);
            }

        }
    }
}


