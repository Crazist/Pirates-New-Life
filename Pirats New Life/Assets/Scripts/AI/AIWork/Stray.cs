using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameInit.AI
{
    public class Stray : IWork
    {
        private AIComponent _AIComponent;
        
        public Stray(AIComponent component)
        {
            _AIComponent = component;
        }

        public Transform getTransform()
        {
            return _AIComponent.GeTransform();
        }

        public void Move(Vector3 position)
        {
            _AIComponent.GeNavMeshAgent().SetDestination(position);
        }
    }
}

