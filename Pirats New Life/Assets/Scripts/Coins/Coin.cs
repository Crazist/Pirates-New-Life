using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float _untouchableTime = 4f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform _transform;
    public bool CanPickUp { get; set; }
    public bool SecondTouch { get; set; }

    private WaitForSeconds _waitForSeconds;
   
    public void Hide()
    {
        // TODO: Hide effect

        gameObject.SetActive(false);
    }

    public void Active()
    {
        gameObject.SetActive(true);
    }

    private void Awake()
    {
        rb.isKinematic = false;
        _waitForSeconds = new WaitForSeconds(_untouchableTime);
    }

    public void TurnOnRbKinematik()
    {
        rb.isKinematic = true;
    }

    private void OnEnable()
    {
        StartCoroutine(UntouchableCoroutine());
    }
    private void OnDisable()
    {
        rb.isKinematic = false;
        CanPickUp = false;
        SecondTouch = false;
    }
    private void Start()
    {
        _transform = GetComponent<Transform>();
        rb.isKinematic = false;
    }

    public Transform GetTransform()
    {
        return _transform;
    }
    private IEnumerator UntouchableCoroutine()
    {
        yield return _waitForSeconds;
        CanPickUp = true;
        yield return _waitForSeconds;
        rb.isKinematic = true;
    }
}