using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZyroX
{
    public static class PlayerData 
{
    public const string HighScoreKey = "HighScore";
    public const string CoinsKey = "Coins";
    public const string OwnedShipsKey = "1";
    public const string SelectedShipKey = "SelectedShip";
    public const string RecordDistanceKey = "RecordDistance";
    

    public static int HighScore
    {
        get => PlayerPrefs.GetInt(HighScoreKey, 0);
        set => PlayerPrefs.SetInt(HighScoreKey, value);
    }

    public static int Coins
    {
        get => PlayerPrefs.GetInt(CoinsKey, 0);
        set => PlayerPrefs.SetInt(CoinsKey, value);
    }

    public static float RecordDistance
    {
        get => PlayerPrefs.GetFloat(RecordDistanceKey, 0f);
        set => PlayerPrefs.SetFloat(RecordDistanceKey, value);
    }

    public static void AddCoins(int amount)
    {
        var currentCoins = Coins + amount;
        Coins = currentCoins;
        UIManager.Instance.InGameUI.CoinText.text = $"{Coins}";
    }

    public static List<string> OwnedShips
    {
        get
        {
            if(!PlayerPrefs.HasKey(OwnedShipsKey))
            {
                PlayerPrefs.SetString(OwnedShipsKey, "1");
            }
            var ownedShips = PlayerPrefs.GetString(OwnedShipsKey, "1");
            return new List<string>(ownedShips.Split(','));
        }
        set
        {
            var ownedShips = string.Join(",", value);
            PlayerPrefs.SetString(OwnedShipsKey, ownedShips);
        }
    }

    public static string SelectedShip
    {
        get {
            if(!PlayerPrefs.HasKey(SelectedShipKey))
            {
                PlayerPrefs.SetString(SelectedShipKey, "1");
            }
            return PlayerPrefs.GetString(SelectedShipKey, "1");
        }
        set => PlayerPrefs.SetString(SelectedShipKey, value);
    }

    public static bool OwnsShip(string shipId)
    {
        return OwnedShips.Contains(shipId);
    }

    public static void AddShip(string shipId)
    {
        var ships = OwnedShips;
        if (!ships.Contains(shipId))
        {
            ships.Add(shipId);
            OwnedShips = ships;
        }
        string ownedShips = "";
        foreach (var ship in ships)
        {
            ownedShips += ship + ",";
        }
        PlayerPrefs.SetString(OwnedShipsKey, ownedShips);
    }

    public static void UpdateRecordDistance(float distance)
    {
        if (distance > RecordDistance)
        {
            RecordDistance = distance;
        }
    }
}

}
