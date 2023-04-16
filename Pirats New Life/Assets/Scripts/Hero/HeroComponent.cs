using UnityEngine;
using UnityEngine.AI;

public class HeroComponent : MonoBehaviour
{
   // [field: SerializeField] public ParticleSystem ParticleSystemMoveTo { get; private set; }

    [SerializeField] private float _currentStamina;
    public Transform Transform { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    private Coin coin;

    public float maxStamina = 100f;
    public float staminaDepletionRate = 15f;
    public float staminaRecoveryRate = 5f;
    public float speed = 3.5f;
    public float sprintSpeed = 5f;
    private float currentStamina;
    private bool isSprinting;

    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0f)
        {
            isSprinting = true;
            currentStamina -= staminaDepletionRate * Time.deltaTime;
        }
        else
        {
            isSprinting = false;
        }

        Agent.speed = isSprinting ? sprintSpeed : speed;
        _currentStamina = currentStamina;

        if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 100, 20), "Stamina: " + currentStamina);
    }
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
    public MonoBehaviour GetMono()
    {
        return this;
    }
}