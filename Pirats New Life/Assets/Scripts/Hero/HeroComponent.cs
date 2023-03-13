using UnityEngine;
using UnityEngine.AI;

public class HeroComponent : MonoBehaviour
{
   // [field: SerializeField] public ParticleSystem ParticleSystemMoveTo { get; private set; }

    public Transform Transform { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    private Coin coin;

    private void Awake()
    {
        Transform = transform;
        Agent = GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var _coin = other.gameObject.GetComponent<Coin>();
        if (_coin && _coin.CanPickUp)
        {
            coin = other.gameObject.GetComponent<Coin>();
        }
    }
    public Coin GetCoin()
    {
        return coin;
    }
    public void ForgetCoin()
    {
        coin = null;
    }
}