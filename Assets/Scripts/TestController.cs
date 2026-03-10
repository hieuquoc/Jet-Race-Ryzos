using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZyroX
{
    public class TestController : MonoBehaviour
    {
        public void Add10000Coins()
        {
            PlayerData.AddCoins(10000);
        }
    }
}

