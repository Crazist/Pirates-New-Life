using GameInit.Connector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.MainPositions
{
    public class TownHallComponent : MonoBehaviour
    {
        private AIWarConnector _AIWarConnector;
        
        public void GetWar(AIWarConnector AIWarConnector)
        {
            _AIWarConnector = AIWarConnector;
        }
        public Transform GetTransform()
        {
            return transform;
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

