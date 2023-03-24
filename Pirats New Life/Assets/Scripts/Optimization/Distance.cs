using UnityEngine;
using System;

namespace GameInit.Optimization
{
    public static class Distance
    {
        /// <summary>
        /// Ignores Y axis.
        /// </summary>
        public static float Manhattan(Vector3 a, Vector3 b)
        {
            var x = Math.Abs(a.x - b.x);
            var z = Math.Abs(a.z - b.z);
            return x + z;
        }

        /// <summary>
        /// Returns square magnitude of direction. Ignores Y axis.
        /// </summary>
        public static float Square(Vector3 from, Vector3 to)
        {
            var direction = from - to;
            direction.y = 0f;
            return direction.sqrMagnitude;
        }
    }
}