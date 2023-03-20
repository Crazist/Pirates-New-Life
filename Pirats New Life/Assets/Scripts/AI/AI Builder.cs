using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.AI
{
    public class AIBuilder
    {
        public AIBuilder()
        {
            var camps = UnityEngine.Object.FindObjectsOfType<CampComponent>();
            AISpawner spawner = new AISpawner(camps);
        }

    }
}

