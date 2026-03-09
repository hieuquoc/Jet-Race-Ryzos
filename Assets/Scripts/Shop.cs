using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZyroX
{
    public class Shop : UIPopUpBase
    {
        public ShipList shipList;
        public Transform ShipView;

        public TextMeshProUGUI CoinText;

        public Dictionary<string, ShopItem> shipPreviews = new Dictionary<string, ShopItem>();

        void Start()
        {

        }

        public override void Show()
        {
            base.Show();
            ShowShop();
        }


        public void ShowShop()
        {
            foreach (var ship in shipList.Ships)
            {
                if (!shipPreviews.ContainsKey(ship.Id))
                {
                    var newPreview = Instantiate(Resources.Load<GameObject>("UIs/ShopItem"), ShipView);
                    var shopItem = newPreview.GetComponent<ShopItem>();
                    shopItem.Setup(ship);
                    shipPreviews.Add(ship.Id, shopItem);
                    shopItem.BuyButton.onClick.AddListener(() => BuyShip(ship.Id));
                }
                else
                {
                    shipPreviews[ship.Id].Setup(ship);
                }
            }
            CoinText.SetText($"{PlayerData.Coins}");
        }

        public void BuyShip(string shipId)
        {
            var ship = shipList.Ships.Find(s => s.Id == shipId);
            if (PlayerData.IsShipOwned(shipId))
            {
                Debug.Log("Selected " + ship.Id);
                PlayerData.SelectedShip = shipId;
                Debug.Log("Selected " + ship.Name);
            }
            else
            {
                if (ship.Cost <= PlayerData.Coins)
                {
                    PlayerData.Coins -= ship.Cost;
                    PlayerData.AddShip(shipId);
                    PlayerData.SelectedShip = shipId;
                    Debug.Log("Bought " + ship.Name);
                }
                else
                {
                    Debug.Log("Not enough coins to buy " + ship.Name);
                }
            }

            ShowShop();
            AudioManager.Instance.PlayButtonClick();
        }

        public void BackToHome()
        {
            UIManager.Instance.ShowPopUp(UIManager.Instance.PreviousPopUp);
            AudioManager.Instance.PlayButtonClick();
        }
    }

}
