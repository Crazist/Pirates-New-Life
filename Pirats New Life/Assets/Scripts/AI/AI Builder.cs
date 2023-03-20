using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Connector;

namespace GameInit.AI
{
    public class AIBuilder
    {
        public AIBuilder(AIConnector _AIConnector)
        {
            var camps = UnityEngine.Object.FindObjectsOfType<CampComponent>();
            AISpawner spawner = new AISpawner(camps, _AIConnector);
        }

    }
}

