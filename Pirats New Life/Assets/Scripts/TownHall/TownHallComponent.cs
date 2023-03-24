using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.MainPositions
{
    public class TownHallComponent : MonoBehaviour
    {
        private Transform _transform;
        void Start()
        {
            _transform = GetComponent<Transform>();
        }

        public Transform GetTransform()
        {
            return _transform;
        }
    }
}

