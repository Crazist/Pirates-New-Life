using GameInit.Connector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.MainPositions
{
    public class TownHallComponent : MonoBehaviour
    {
        private Transform _transform;
        private AIWarConnector _AIWarConnector;
        void Start()
        {
            _transform = GetComponent<Transform>();
        }
        public void GetWar(AIWarConnector AIWarConnector)
        {
            _AIWarConnector = AIWarConnector;
        }
        public Transform GetTransform()
        {
            return _transform;
        }
        private void OnDrawGizmos()
        {
            if(_AIWarConnector != null)
            {
                _AIWarConnector.DrawGiz();
            }
        }
    }
}

