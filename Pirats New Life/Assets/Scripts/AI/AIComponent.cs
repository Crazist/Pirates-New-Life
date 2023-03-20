using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.AI
{
    public class AIComponent : MonoBehaviour
    {
        [SerializeField] private Transform positiontransform;

        public Transform GeTransform()
        {
            return positiontransform;
        }
    }
}

