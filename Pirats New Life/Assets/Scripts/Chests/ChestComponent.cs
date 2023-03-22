using DG.Tweening;
using UnityEngine;

namespace GameInit.Component
{
    public class ChestComponent : MonoBehaviour
    {
        [field: SerializeField] public int GoldAmount { get; private set; }

        public bool IsCollided { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out HeroComponent hero))
            {
                IsCollided = true;
            }
        }
        public MonoBehaviour GetMono()
        {
            return this;
        }
    }
}
