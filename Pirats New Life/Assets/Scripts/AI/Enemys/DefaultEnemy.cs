using GameInit.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameInit.Enemy
{
    public class DefaultEnemy : IKDTree, IEnemy
    {
        public bool InMove { get; set; }

        private AIComponent _AIComponent;
        private float _defY = 0.44f;
        private float _delayForAttack = 1;
        private bool _canDamage = true;
        private int _damage = 1;
        public int HP { get; set; } = 1;

        private const bool _isEnemy = true;

        public DefaultEnemy(AIComponent AIComponent)
        {
            _AIComponent = AIComponent;
            _AIComponent.GetGm().SetActive(false);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(_AIComponent.GetTransform().position, out hit, 10.0f, NavMesh.AllAreas))
            {
                _AIComponent.GetTransform().position = hit.position;
            }
            _AIComponent.GetGm().SetActive(true);
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

        public void Move(Vector3 position)
        {
            if (_AIComponent.GeNavMeshAgent().isOnNavMesh)
            {
                InMove = true;
                _AIComponent.GeNavMeshAgent().SetDestination(position);
            }
        }

        public void GetDamage(int damage)
        {
            if (HP - damage <= 0)
            {
                Die();
                HP = 0;
            }
            else
            {
                HP = HP - damage;
            }
        }

        private void Die()
        {
            _AIComponent.GetGm().SetActive(false);
        }
        public bool CheckIfEnemy()
        {
            return _isEnemy;
        }

        public bool CheckIfCanDamage()
        {
            return _canDamage;
        }

        public int CountOFDamage()
        {
            return _damage;
        }
        private IEnumerator AttackDelay()
        {
            _canDamage = false;
            yield return new WaitForSecondsRealtime(_delayForAttack);
            _canDamage = true;
        }

        public void Attack()
        {
            _AIComponent.StartCoroutine(AttackDelay());
            //animation
        }

        public AIComponent GetAiComponent()
        {
            return _AIComponent;
        }
    }
}

