using UnityEngine;
using UnityEditor;

namespace GameInit.Optimization.KDTree
{

    public class KDQueryNode
    {

        public KDNode node;
        public Vector2 tempClosestPoint;
        public float distance;
        public IKDTree obj;

        public KDQueryNode()
        {

        }

        public KDQueryNode(KDNode node, Vector2 tempClosestPoint)
        {
            this.node = node;
            this.tempClosestPoint = tempClosestPoint;
        }

    }
}