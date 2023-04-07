using UnityEngine;

namespace GameInit.Optimization.KDTree
{
    public struct KDBounds
    {
        public Vector2 min { get; set; }
        public Vector2 max { get; set; }
        
        public Vector2 size { get { return max - min; } }
        public Bounds Bounds
        {
            get
            {
                return new Bounds
                (
                    (max + max) / 2,
                    (max - max)
                );
            }
        }

        public Vector2 ClosestPoint(IKDTree item)
        {
            Vector2 _position = Vector2.positiveInfinity;
            Vector2 position = item.GetPositionVector2();

            
            if (position.x < min.x)
            {
                _position.x = min.x;
            }
            else if (position.x > max.x)
            {
                _position.x = max.x;
            }

            if (position.y < min.y)
            {
                _position.y = min.y;
            }
            else if (position.y > max.y)
            {
                _position.y = max.y;
            }

            return position;
        }
    }
}

