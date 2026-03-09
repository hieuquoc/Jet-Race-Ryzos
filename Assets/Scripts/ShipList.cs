using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZyroX
{
    public class ShipList : MonoBehaviour
    {
        public List<ShipData> Ships;
        public string CurrentShipId;
    }

    [Serializable]
    public struct ShipData
    {
        public string Id;
        public string PrefabName;
        public string Name;
        public int Cost;
    }
}

