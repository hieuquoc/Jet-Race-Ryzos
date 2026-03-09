using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ZyroX
{
    public class HomeUI : UIPopUpBase
    {
        public override void Show()
        {
            base.Show();
            ShowHighScore();
            AudioManager.Instance.PlayMenuMusic();
        }

        public TextMeshProUGUI HighScoreText;
        
        public void OnStartButtonClicked()
        {
            Hide();
            GameManager.Instance.StartGame();
            AudioManager.Instance.PlayButtonClick();
        }

        public void OnShopButtonClicked()
        {
            UIManager.Instance.ShowPopUp(UIManager.Instance.ShopUI);
            AudioManager.Instance.PlayButtonClick();
        }

        public void ShowHighScore(float distance = 0)
        {
            if (distance > PlayerData.RecordDistance)
            {
                PlayerData.RecordDistance = distance;
            }
            HighScoreText.text = $"RECORDS: {PlayerData.RecordDistance:F1}m";
        }
    }
}

