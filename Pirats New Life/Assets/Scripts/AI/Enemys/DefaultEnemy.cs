using GameInit.AI;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GameInit.Enemy
{
    public class DefaultEnemy : IKDTree, IEnemy
    {
        public bool InMove { get; set; }
        public bool RefreshSkill { get; set; } = false;
        public float DistanceForStartSpell { get; } = 70;

        private AIComponent _AIComponent;
        private float _delayForAttack = 3;
        private bool _canDamage = true;
        private int _damage = 1;
        private float _spellSpeed = 30f;
        private float _slowReturnSpeed= 2f;
        private float _standartSpeed;
        private float _returnOffset = 6f;
        private float _spellWaitForCast = 0.5f;
        private float _originalAcceleration;
        private float _spellAcceleration = 10f;
        private float _originalStopingDistance;
        private float _spellStopingDistance = 2.6f;
        private Vector3 _curTargetPosition;
        private float _maxOffset = 1;
        private float _runTime = 2.5f;
        private bool _needToRunAway = false;
        private int _damageInRun = 2;
        private int _standartDamage = 1;
        public int HP { get; set; } = 1;
        public EntityType Type { get; } = EntityType.Enemy;

        private const float _heightPosition = 0.46f;
        private const float _zeroSpeed = 0f;
        private const bool _isEnemy = true;
        private const float _minStopingDistanceForEnemy = 2.7f;

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
            _standartSpeed = _AIComponent.GeNavMeshAgent().speed;
            _originalAcceleration = _AIComponent.GeNavMeshAgent().acceleration;
            _originalStopingDistance = _AIComponent.GeNavMeshAgent().stoppingDistance;
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
            _AIComponent.StopAllCoroutines();
            _canDamage = true;
            RefreshSkill = false;
            _AIComponent.GeNavMeshAgent().speed = _standartSpeed;
            _AIComponent.GeNavMeshAgent().acceleration = _originalAcceleration;
            _AIComponent.GeNavMeshAgent().stoppingDistance = _originalStopingDistance;
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
            if (_canDamage)
                _AIComponent.StartCoroutine(AttackDelay());
            //animation
        }

        public AIComponent GetAiComponent()
        {
            return _AIComponent;
        }

        private IEnumerator SpellDeley(Vector3 position)
        {
            RefreshSkill = true;
            
            yield return new WaitForSeconds(_spellWaitForCast);

            _AIComponent.GeNavMeshAgent().speed = _zeroSpeed;

            Move(position);
            _AIComponent.GeNavMeshAgent().speed = _spellSpeed;
            _AIComponent.GeNavMeshAgent().acceleration = _spellAcceleration;
            _AIComponent.GeNavMeshAgent().stoppingDistance = _spellStopingDistance;

            yield return new WaitForSeconds(_runTime);

            _AIComponent.GeNavMeshAgent().speed = _standartSpeed;
            _AIComponent.GeNavMeshAgent().acceleration = _originalAcceleration;
            _AIComponent.GeNavMeshAgent().stoppingDistance = _originalStopingDistance;
            _damage = _standartDamage;

            if (_needToRunAway)
            {
                _AIComponent.GetMonoBehaviour().StartCoroutine(ReturnDeley());
            }
            else
            {
                RefreshSkill = false;
            }
            
        }
        private IEnumerator ReturnDeley()
        {
            if(Vector2.SqrMagnitude(_AIComponent.GeNavMeshAgent().transform.position - _curTargetPosition) < DistanceForStartSpell)
            {
                Vector3 target = _curTargetPosition + new Vector3(Random.Range(-_maxOffset, _maxOffset), 0, Random.Range(-_maxOffset, _maxOffset));

                if (_curTargetPosition.x > 0)
                {
                    target.x = _curTargetPosition.x + _returnOffset;
                }
                else
                {
                    target.x = _curTargetPosition.x - _returnOffset;
                }

                target.z = _curTargetPosition.z;
                target.y = _heightPosition;

                _AIComponent.GeNavMeshAgent().speed = _slowReturnSpeed;

                Move(target);

                yield return new WaitForSeconds(_runTime);
            }
           
            _AIComponent.GeNavMeshAgent().speed = _standartSpeed;

            RefreshSkill = false;
        }

        public void UseSpell(Vector3 position, bool runAway)
        {
            _needToRunAway = runAway;
            _curTargetPosition = position;
            if (!RefreshSkill)
            {
                _damage = _damageInRun;
                Move(_AIComponent.GetTransform().position);
                _AIComponent.GetMonoBehaviour().StartCoroutine(SpellDeley(position));
            }
        }

        public void StopSpell()
        {
            _AIComponent.GetMonoBehaviour().StopAllCoroutines();
            _damage = _standartDamage;
            RefreshSkill = false;
            _AIComponent.GeNavMeshAgent().speed = _standartSpeed;
            _AIComponent.GeNavMeshAgent().acceleration = _originalAcceleration;
            _AIComponent.GeNavMeshAgent().stoppingDistance = _originalStopingDistance;
        }
    }
}

