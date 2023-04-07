using UnityEngine;

namespace GameInit.Optimization.KDTree
{
    public class KDNode
    {
        public float partitionCoord { get; set; }
        public int partitionAxis { get; set; } = -1;


        public KDNode negativeChild { get; set; }
        public KDNode positiveChild { get; set; }

        public int start { get; set; }
        public int end { get; set; }

        public int Count { get { return end - start; } }
        public bool isLeaf { get { return partitionAxis == -1; } }

        public KDBounds bounds; 

        public void Clear()
        {
            partitionCoord = 0;
            partitionAxis = 0;

            negativeChild = null;
            positiveChild = null;

            start = 0;
            end = 0;

            bounds = new KDBounds();
        }
    }
}
