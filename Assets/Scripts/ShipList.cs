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

        
        public ShipData GetShipData(string id)
        {
            return Ships.Find(s => s.Id == id);
        }
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

