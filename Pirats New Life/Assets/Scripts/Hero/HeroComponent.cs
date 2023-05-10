using UnityEngine;
using UnityEngine.AI;

public class HeroComponent : MonoBehaviour
{
   // [field: SerializeField] public ParticleSystem ParticleSystemMoveTo { get; private set; }

    [SerializeField] private float _currentStamina;
    [SerializeField] private Animator _Animator;
    public Transform Transform { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    private Coin coin;

    public float maxStamina = 100f;
    public float staminaDepletionRate = 15f;
    public float staminaRecoveryRate = 6f;
    public float speed = 3.5f;
    public float sprintSpeed = 7f;
    private float currentStamina;
    private bool isSprinting;

    public Animator GetAnimator()
    {
        return _Animator;
    }
    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0f)
        {
            if (!isSprinting)
            {
                _Animator.SetBool("sprintStart", true);
                _Animator.SetBool("sprintEnd", false);
            }
            isSprinting = true;
            currentStamina -= staminaDepletionRate * Time.deltaTime;
        }
        else
        {
            if (isSprinting)
            {
                _Animator.SetBool("sprintEnd", true);
                _Animator.SetBool("sprintStart", false);
            }
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
    public Transform GetTransform()
    {
        return transform;
    }
}