using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ZyroX
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public TextMeshProUGUI DistanceText;
        public HomeUI HomeUI;
        public InGameUI InGameUI;
        public Shop ShopUI;
        public GameOverUI GameOverUI;

        public UIPopUpBase CurrentPopUp { get; private set; }
        public UIPopUpBase PreviousPopUp { get; private set; }


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            InGameUI.gameObject.SetActive(false);
            ShopUI.gameObject.SetActive(false);
            GameOverUI.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            ShowPopUp(HomeUI);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlayGame()
        {
            ShowPopUp(InGameUI);
        }

        public void ShowPopUp(UIPopUpBase popUp)
        {
            if (CurrentPopUp != null)
            {
                CurrentPopUp.Hide();
                PreviousPopUp = CurrentPopUp;
            }
            CurrentPopUp = popUp;
            CurrentPopUp.Show();
        }
    }



}
