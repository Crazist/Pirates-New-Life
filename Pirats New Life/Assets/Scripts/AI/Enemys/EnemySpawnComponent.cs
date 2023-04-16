using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Enemy
{
    public class EnemySpawnComponent : MonoBehaviour
    {
        [SerializeField] private int _count = 2;
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private float spawnDiffMax = 3;
        [SerializeField] private float spawnDiffMin = 1;
        [SerializeField] private Transform transformSpawnPosition;
        public void SetCount(int count)
        {
            _count = count;
        }
        public int GetCount()
        {
            return _count;
        }
        public GameObject GetEnemy()
        {
            return _enemyPrefab;
        }

        public Transform GetTransformSpawn()
        {
            return transformSpawnPosition;
        }

        public float GetSpawnDiffMax()
        {
            return spawnDiffMax;
        }
        public float GetSpawnDiffMin()
        {
            return spawnDiffMin;
        }
        
    }
}

