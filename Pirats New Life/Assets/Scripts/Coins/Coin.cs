using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float _untouchableTime = 4f;
    public bool CanPickUp { get; set; }

    private WaitForSeconds _waitForSeconds;

    public void Hide()
    {
        // TODO: Hide effect

        gameObject.SetActive(false);
    }

    private void Awake()
    {
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
        CanPickUp = true;
    }
}