using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameInit.AI
{
    public class AIComponent : MonoBehaviour
    {
        [SerializeField] private Transform _positiontransform;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        public Transform GetTransform()
        {
            return _positiontransform;
        }
        public NavMeshAgent GeNavMeshAgent()
        {
            return _navMeshAgent;
        }

        public MonoBehaviour GetMonoBehaviour()
        {
            return this;
        }
        
    }
}

