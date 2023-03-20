using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.AI
{
    public class Stray : IWork, IUpdate
    {
        private AIComponent _AIComponent;
        public Stray(AIComponent component)
        {
            _AIComponent = component;
        }
        public void OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}

