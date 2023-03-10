using UnityEngine;

public class ChestComponent : MonoBehaviour
{
    [field: SerializeField] public int GoldAmount { get; private set; }
    [field: SerializeField] public float ColliderRadius { get; private set; }

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