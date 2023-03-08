using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance;

    private List<GameObject> _units = new List<GameObject>(100);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnAdd(GameObject go)
    {
        _units.Add(go);
    }

    private void OnDeath(GameObject go)
    {
        _units.Remove(go);
    }

    private void OnEnable()
    {
        Events.Unit.onUnitSpawn += OnAdd;
        Events.Unit.onUnitDeath += OnDeath;
    }

    private void OnDisable()
    {
        Events.Unit.onUnitSpawn -= OnAdd;
        Events.Unit.onUnitDeath -= OnDeath;
    }

    public List<GameObject> GetUnitsList()
    {
        return _units;
    }
}
