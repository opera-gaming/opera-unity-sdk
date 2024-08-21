using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Opera
{
    public class PressedButtonIndicator : MonoBehaviour
    {
        public KeyCode key;

        private Image image;

        private void Awake()
        {
            image = GetComponentInChildren<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(key))
            {
                image.color = Color.red;
            }
            else
            {
                image.color = Color.white;
            }
        }
    }
}
