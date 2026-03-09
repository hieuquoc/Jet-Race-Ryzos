using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZyroX
{
    public class Shop : UIPopUpBase
{
    public ShipList shipList;
    public Transform ShipView;

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
    }

    public void BuyShip(string shipId)
    {
        var ship = shipList.Ships.Find(s => s.Id == shipId);
        if (ship.Cost <= PlayerData.Coins)
        {
            PlayerData.Coins -= ship.Cost;
            PlayerData.AddShip(shipId);
            Debug.Log("Bought " + ship.Name);
        }
        else
        {
            Debug.Log("Not enough coins to buy " + ship.Name);
        }
        AudioManager.Instance.PlayButtonClick();
    }

    public void BackToHome()
    {
        UIManager.Instance.ShowPopUp(UIManager.Instance.PreviousPopUp);
        AudioManager.Instance.PlayButtonClick();
    }
}

}
