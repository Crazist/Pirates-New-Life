using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.AI;

namespace GameInit.Connector
{
    public class AIConnector
    {
        public List<IWork> StrayList { get; set; }

        public void MoveToClosestAI(Vector3 targetPosition)
        {
            float minDistance = Mathf.Infinity;
            Vector3 closestPosition = Vector3.zero;
            IWork _stray = null;

            foreach (var stray in StrayList)
            {
                float distance = Vector3.Distance(stray.getTransform().position, targetPosition);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPosition = stray.getTransform().position;
                    _stray = stray;
                }
            }

            _stray.Move(closestPosition);
        }

        public void InitConnector()
        {
            StrayList = new List<IWork>();
        }

    }
}

