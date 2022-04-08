using System;
using UnityEngine;

namespace SnakeTest.Objects
{
    [CreateAssetMenu(fileName = "GameController", menuName = "ScriptableObjects/GameController", order = 1)]
    public class GameEvents : ScriptableObject
    {
        public Action OnGameStart;
        public Action OnGameOver;
        public Action<int> OnGearChange;
        public Action<int> OnRamsChange;
    }
}