using GameInit.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Enemy
{
    public class EnemySpawnComponent : MonoBehaviour
    {
        [SerializeField] private int _count = 2;
        [SerializeField] private AIComponent _AIComponent;
        [SerializeField] private float spawnDiffMax = 3;
        [SerializeField] private float spawnDiffMin = 1;
        [SerializeField] private Transform transformSpawnPosition;
        [SerializeField] private int _multiplier = 1;
        [SerializeField] private ParticleSystem _paticle;
        [SerializeField] private float _delayForSpawnEnemyIfInZone = 3f;
        
        private Action<EnemySpawnComponent> _closeToPortal;
        private bool _isPlaying = false;
        private bool _isPlayerInside = false;
        private Coroutine _coroutine;
        public int Multiplier { get { return _multiplier; } }

        public void SetAction(Action<EnemySpawnComponent> _action)
        {
            _closeToPortal = _action;
        }

        private void Start()
        {
            _paticle.Stop();
        }
        public void SetCount(int count)
        {
            _count = count;
        }
        public int GetCount()
        {
            return _count;
        }
        public AIComponent GetEnemy()
        {
            return _AIComponent;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out HeroComponent hero))
            {
                if (!_isPlaying)
                {
                    _paticle.Play();
                    _isPlaying = true;
                }
                
                _isPlayerInside = true;
                if (_coroutine == null)
                {
                    _coroutine = StartCoroutine(RepeatAction());
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out HeroComponent hero))
            {
                if (_isPlaying)
                {
                    _paticle.Stop();
                    _isPlaying = false;
                }
                
                _isPlayerInside = false;
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }
            }
        }
        public Transform GetTransformSpawn()
        {
            return transformSpawnPosition;
        }

        public float GetSpawnDiffMax()
        {
            return spawnDiffMax;
        }
        public float GetSpawnDiffMin()
        {
            return spawnDiffMin;
        }
        private IEnumerator RepeatAction()
        {
            while (_isPlayerInside)
            {
                yield return new WaitForSeconds(_delayForSpawnEnemyIfInZone);
                _closeToPortal.Invoke(this);
            }
        }
    }
}

