using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI CostText;
    public TextMeshProUGUI StatusText;
    public GameObject NotEnoughOverlay;

    public void Setup(ShipData shipData)
    {
        Icon.sprite = Resources.Load<Sprite>("Icons/" + shipData.Id);
        NameText.text = shipData.Name;
        CostText.text = shipData.Cost.ToString() + " Coins";
        if (PlayerData.OwnsShip(shipData.Id))
        {
            StatusText.text = "Owned";
        }
        else
        {
            StatusText.text = "Purchase";
            StatusText.gameObject.SetActive(shipData.Cost <= PlayerData.Coins);
            NotEnoughOverlay.SetActive(shipData.Cost > PlayerData.Coins);
        }
    }
}
