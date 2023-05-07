using DG.Tweening;
using UnityEngine;

namespace GameInit.Component
{
    public class ChestComponent : MonoBehaviour
    {
        [field: SerializeField] public int GoldAmount { get; private set; }
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _particlePrefab;

        public bool IsCollided { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out HeroComponent hero))
            {
                IsCollided = true;
            }
        }
        public GameObject GetParticlePrefab()
        {
            return _particlePrefab;
        }
        public MonoBehaviour GetMono()
        {
            return this;
        }
        public Animator GetAnimator()
        {
            return _animator;
        }
    }
}
