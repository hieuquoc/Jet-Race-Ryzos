using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ZyroX
{
    public class InGameUI : UIPopUpBase
    {
        public TextMeshProUGUI TargetDistanceText;
        public PopUpMessage PopUpMessage;
        public static InGameUI Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public override void Show()
        {
            base.Show();
            TargetDistanceText.text = $"{PlayerData.RecordDistance:F1}m";
            PopUpMessage.HideAll();
        }

        public override void Hide()
        {
            base.Hide();
            PopUpMessage.HideAll();
        }

        public void ShowMessage(MessageType type)
        {
            PopUpMessage.ShowMessage(type);
        }

    }
}

