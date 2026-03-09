using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ZyroX
{
    public class InGameUI : UIPopUpBase
    {
        public TextMeshProUGUI TargetDistanceText;

        public override void Show()
        {
            base.Show();
            TargetDistanceText.text = $"{PlayerData.RecordDistance:F1}m";
        }

        void Start()
        {

        }
    }
}

