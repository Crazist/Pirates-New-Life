using GameInit.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Enemy
{
    public class DefaultEnemy : IKDTree
    {
        private Vector2 _positionOnVector2;
        private float _defY = 0.44f;
        public DefaultEnemy(Transform position)
        {
            _positionOnVector2.x = position.position.x;
            _positionOnVector2.y = position.position.z;
        }
        public int GetId()
        {
            throw new System.NotImplementedException();
        }

        public Vector2 GetPositionVector2()
        {
            return _positionOnVector2;
        }

        public void GiveDamage()
        {
            throw new System.NotImplementedException();
        }
    }
}

