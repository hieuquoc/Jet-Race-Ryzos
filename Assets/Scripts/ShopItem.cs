using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZyroX
{
    public class ShopItem : MonoBehaviour
    {
        public Image Icon;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI CostText;
        public TextMeshProUGUI StatusText;
        public GameObject NotEnoughOverlay;
        public Button BuyButton;

        public void Setup(ShipData shipData)
        {
            Icon.sprite = Resources.Load<Sprite>("Icons/" + shipData.PrefabName);
            NameText.text = shipData.Name;
            CostText.text = "Cost: " + shipData.Cost.ToString();
            if (PlayerData.IsShipOwned(shipData.Id))
            {
                if(PlayerData.SelectedShip == shipData.Id)
                {
                    StatusText.text = "Selected";
                }
                else
                    StatusText.text = "Owned";
                NotEnoughOverlay.SetActive(false);
            }
            else
            {
                StatusText.text = "Purchase";
                StatusText.gameObject.SetActive(shipData.Cost <= PlayerData.Coins);
                NotEnoughOverlay.SetActive(shipData.Cost > PlayerData.Coins);
                BuyButton.interactable = shipData.Cost <= PlayerData.Coins;
            }
        }
    }

}
