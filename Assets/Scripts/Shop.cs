using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public ShipList shipList;
    public Transform ShipView;

    public Dictionary<string, ShopItem> shipPreviews = new Dictionary<string, ShopItem>();

    void Start()
    {
        ShowShop();
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
