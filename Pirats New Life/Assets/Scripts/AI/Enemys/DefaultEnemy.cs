using Cysharp.Threading.Tasks;
using GameInit.AI;
using System;
using System.Collections;
using System.Threading;
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
        private Animator _Animator;
        private float _delayForAttack = 3;
        private bool _canDamage = true;
        private int _damage = 1;
        private float _spellSpeed = 40f;
        private float _slowReturnSpeed= 2f;
        private float _standartSpeed;
        private float _returnToBaseSpeed = 5;
        private float _returnOffset = 6f;
        private float _spellWaitForCast = 0.5f;
        private float _originalAcceleration;
        private float _spellAcceleration = 10f;
        private float _originalStopingDistance;
        private float _spellStopingDistance = 2.4f;
        private Vector3 _curTargetPosition;
        private float _maxOffset = 1;
        private float _runTime = 2.5f;
        private float _returnTime = 3f;
        private bool _needToRunAway = false;
        private int _damageInRun = 2;
        private int _standartDamage = 1;
        private bool _isDying = false;
        private CancellationToken token;
        public int HP { get; set; } = 1;
        public EntityType Type { get; } = EntityType.Enemy;

        private const float _heightPosition = 0.46f;
        private const float _zeroSpeed = 0f;
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
            CancellationTokenSource cts = new CancellationTokenSource();
            token = cts.Token;
            _AIComponent.GetGm().SetActive(true);
            _standartSpeed = _AIComponent.GeNavMeshAgent().speed;
            _originalAcceleration = _AIComponent.GeNavMeshAgent().acceleration;
            _originalStopingDistance = _AIComponent.GeNavMeshAgent().stoppingDistance;
            _Animator = _AIComponent.GetAnimator();
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
                if (!_isDying)
                {
                    _AIComponent.GetBloodSplash().Play();
                    _AIComponent.GetMonoBehaviour().StartCoroutine(BeforeDie());
                }
            }
            else
            {
                HP = HP - damage;
            }
        }

        private IEnumerator BeforeDie()
        {
            _isDying = true;
            HP = 0;
            yield return new WaitForSeconds(0.2f);
            Die();
            _isDying = false;
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
        
        private async void AttackDelay()
        {
            _canDamage = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_delayForAttack), cancellationToken: token);
            _canDamage = true;
        }

        public void Attack()
        {
            if (_canDamage)
            {
                _Animator.SetTrigger("attack");
                AttackDelay();
            }
        }

        public AIComponent GetAiComponent()
        {
            return _AIComponent;
        }

        private async void SpellDeley(Vector3 position)
        {
            RefreshSkill = true;
            _Animator.SetBool("iddle", true);

            await UniTask.Delay(TimeSpan.FromSeconds(_spellWaitForCast), cancellationToken: token);
            
            _Animator.SetBool("iddle", false);
            _Animator.SetBool("charge", true);

            _AIComponent.GeNavMeshAgent().speed = _zeroSpeed;

            Move(position);
            _AIComponent.GeNavMeshAgent().speed = _spellSpeed;
            _AIComponent.GeNavMeshAgent().acceleration = _spellAcceleration;
            _AIComponent.GeNavMeshAgent().stoppingDistance = _spellStopingDistance;


            await UniTask.Delay(TimeSpan.FromSeconds(_runTime), cancellationToken: token);

            _AIComponent.GeNavMeshAgent().speed = 0;

            _AIComponent.GeNavMeshAgent().acceleration = _originalAcceleration;
            _AIComponent.GeNavMeshAgent().stoppingDistance = _originalStopingDistance;
            _damage = _standartDamage;

            _Animator.SetBool("charge", false);
            _Animator.SetBool("walk", true);

            await UniTask.Yield(PlayerLoopTiming.Update);

            _AIComponent.GeNavMeshAgent().speed = _standartSpeed;

            _Animator.Update(0f);
            if (_needToRunAway)
            {
                ReturnDeley();
            }
            else
            {
                RefreshSkill = false;
            }
        }

        private async void ReturnDeley()
        {
            if (Vector2.SqrMagnitude(_AIComponent.GeNavMeshAgent().transform.position - _curTargetPosition) < DistanceForStartSpell)
            {
                Vector3 target = _curTargetPosition + new Vector3(UnityEngine.Random.Range(-_maxOffset, _maxOffset), 0, UnityEngine.Random.Range(-_maxOffset, _maxOffset));

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

                await UniTask.Delay(TimeSpan.FromSeconds(_returnTime), cancellationToken: token);
            }

            _AIComponent.GeNavMeshAgent().speed = _standartSpeed;

            RefreshSkill = false;
        }
        
        private IEnumerator Waiter(Action action, Vector3 position)
        {
            _AIComponent.GeNavMeshAgent().speed = _returnToBaseSpeed;

            Move(position);

           var agent = _AIComponent.GeNavMeshAgent();
            
            while (Vector2.SqrMagnitude(position - agent.transform.position) > 1)
            {
                yield return null;
            }

            _AIComponent.GeNavMeshAgent().speed = _standartSpeed;

            InMove = false;
            action?.Invoke();
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
                SpellDeley(position);
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

        public void MoveToBase(Vector3 position, Action _action)
        {
            if (!RefreshSkill)
            {
                RefreshSkill = true;
                _AIComponent.GetMonoBehaviour().StartCoroutine(Waiter(_action, position));
            }
        }

        void IEnemy.Disable()
        {
            Die();
        }
    }
}

