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

        public Transform GeTransform()
        {
            return _positiontransform;
        }
        public NavMeshAgent GeNavMeshAgent()
        {
            return _navMeshAgent;
        }
    }
}

