using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowComponent : MonoBehaviour
{
    [SerializeField] private GameObject _gm;
    [SerializeField] private Transform _transform;
    [SerializeField] private MonoBehaviour _mono;

    public MonoBehaviour GetMono()
    {
        return _mono;
    }
    public GameObject GetGameObj()
    {
        return _gm;
    }
    public Transform GetTransform()
    {
        return _transform;
    }
}
