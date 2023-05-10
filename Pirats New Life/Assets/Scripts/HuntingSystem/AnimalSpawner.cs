using GameInit.AI;
using Unity.Collections;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] private int _countRabbits;
    [SerializeField] private float _spawnDiffMaxX = 3;
    [SerializeField] private float _spawnDiffMinX = 1;
    [SerializeField] private float _spawnDiffMaxZ = 3;
    [SerializeField] private float _spawnDiffMinZ = 1;
    [SerializeField] private float _offsetFromTheWall = 20;
    [SerializeField] private float _radius = 1;
    [SerializeField] private AIComponent _AIComponent;

    [SerializeField, ReadOnly] private int _curCount = 0;

    public void AddCurCount()
    {
        _curCount++;
    }
    public void RemoveCurCount()
    {
        if(_curCount > 0)
        _curCount--;
    }
    public int GetCurCount()
    {
        return _curCount;
    }
    public float GetRadius()
    {
        return _radius;
    }
    public AIComponent GetAiComponent()
    {
        return _AIComponent;
    }
    public Transform GetTransform()
    {
        return transform;
    }
    public AIComponent GetEnemy()
    {
        return _AIComponent;
    }
    public float GetSpawnDiffMaxX()
    {
        return _spawnDiffMaxX;
    }
    public float GetSpawnDiffMinX()
    {
        return _spawnDiffMinX;
    }
    public float GetSpawnDiffMaxZ()
    {
        return _spawnDiffMaxZ;
    }
    public float GetSpawnDiffMinZ()
    {
        return _spawnDiffMinZ;
    }
    public int GetCountRabbits()
    {
        return _countRabbits;
    }
    public float GetOffsetFromTheWall()
    {
        return _offsetFromTheWall;
    }
}
