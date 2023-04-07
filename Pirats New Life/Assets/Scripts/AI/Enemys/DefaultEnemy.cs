using GameInit.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Enemy
{
    public class DefaultEnemy : IKDTree, IEnemy
    {
        public bool InMove { get; set; }

        private AIComponent _AIComponent;
        private float _defY = 0.44f;

        private const bool _isEnemy = true;

        public DefaultEnemy(AIComponent AIComponent)
        {
            _AIComponent = AIComponent;

            
        }

        

        public int GetId()
        {
            throw new System.NotImplementedException();
        }

        public Vector2 GetPositionVector2()
        {
            Vector2 _positionOnVector2;
            _positionOnVector2.x = _AIComponent.GetTransform().position.x;
            _positionOnVector2.y = _AIComponent.GetTransform().position.z;

            return _positionOnVector2;
        }

        public void GiveDamage()
        {
            throw new System.NotImplementedException();
        }

        public void Move(Vector3 position, Action action)
        {
            InMove = true;
            _AIComponent.GeNavMeshAgent().destination = position;
        }

        public bool CheckIfEnemy()
        {
            return _isEnemy;
        }
    }
}

