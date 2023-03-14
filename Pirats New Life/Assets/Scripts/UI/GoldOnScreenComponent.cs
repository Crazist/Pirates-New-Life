using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameInit.Component
{
    public class GoldOnScreenComponent : MonoBehaviour
    {
        [SerializeField] private TMP_Text goldText;

        public void SetText(string str)
        {
            goldText.text = str;
        }
    }
}

