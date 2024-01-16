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
            public bool once;
            private bool _used;
            
            public bool Used
            {
                get => _used;
                set => _used = value;
            }
        }

        public List<PowerupOdds> powerups;
    }
}