using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.AI
{
    public class CampComponent : MonoBehaviour
    {
        [SerializeField] private AIComponent CitizenPrefab;
        [SerializeField] private int count = 3;
        [SerializeField] private float spawnDiffMax = 3;
        [SerializeField] private float spawnDiffMin = 1;
        [SerializeField] private Transform transformSpawnPosition;

        private List<IWork> _strays;
        
        private void Start()
        {
            _strays = new List<IWork>();
        }
        public List<IWork> GetStrayList()
        {
            return _strays;
        }

        public AIComponent GetCitizenPrefab()
        {
            return CitizenPrefab;
        }

        public int GetCount()
        {
            return count;
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

