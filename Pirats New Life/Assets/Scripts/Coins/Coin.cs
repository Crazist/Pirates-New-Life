using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float _untouchableTime = 4f;
    [SerializeField] private Rigidbody rb;
    public bool CanPickUp { get; set; }

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

    private void OnEnable()
    {
        StartCoroutine(UntouchableCoroutine());
    }
    private void OnDisable()
    {
        CanPickUp = false;
    }
    private IEnumerator UntouchableCoroutine()
    {
        yield return _waitForSeconds;
        rb.isKinematic = true;
        CanPickUp = true;
    }
}