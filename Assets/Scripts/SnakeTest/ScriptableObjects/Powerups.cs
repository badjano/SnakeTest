using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeTest.Objects
{
    [CreateAssetMenu(fileName = "Powerups", menuName = "ScriptableObjects/Powerups", order = 1)]
    public class Powerups : ScriptableObject
    {
        [Serializable]
        public struct PowerupOdds
        {
            public float odds;
            public GameObject powerup;
        }

        public List<PowerupOdds> powerups;
    }
}