using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZyroX
{
    [CreateAssetMenu(fileName = "ObstacleSet", menuName = "ScriptableObjects/ObstacleSet", order = 1)]
    public class ObstacleSet : ScriptableObject
    {
        public GameObject[] Obstacles;
        public int PoolSize = 5;
    }
}

