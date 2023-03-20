using UnityEngine;

namespace GameInit.AI
{
    public interface IWork
    {
        public Transform getTransform();
        public void Move(Vector3 position);
    }
}

