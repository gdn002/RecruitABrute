using System;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
    {
        public static Guide ActiveGuide;
        
        private Text text;

        private void Awake()
        {
            if (ActiveGuide == null)
            {
                ActiveGuide = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            text = gameObject.GetComponent<Text>();
            Debug.Log(gameObject.GetComponent<Text>());
        }

        public void DisplayPlaceUnitGuide()
        {
            text.text = "Place your brutes on the board";
        }

        public void ClearGuide()
        {
            text.text = "";
        }
    }