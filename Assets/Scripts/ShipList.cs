using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipList : MonoBehaviour
{
    public List<ShipData> Ships;
    public string CurrentShipId;
}

[Serializable]
public struct ShipData
{
    public string Id;
    public string Name;
    public int Cost;
}
