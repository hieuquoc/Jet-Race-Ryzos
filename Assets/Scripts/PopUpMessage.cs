using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZyroX
{
    public class PopUpMessage : MonoBehaviour
    {
        public GameObject[] Messages;
        public TextMeshProUGUI BonusText;
        public TextMeshProUGUI EffectText;
        public Image EffectTimerFill;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowMessage(MessageType type)
        {
            int index = (int)type;
            if (index >= 0 && index < Messages.Length)
            {
                Messages[index].SetActive(true);
            }
            else
            {
                Debug.LogWarning("Invalid MessageType index: " + index);
            }
        }
    }

    public enum MessageType
    {
        LookOut,
        Bonus,
        Effects
    }

}
