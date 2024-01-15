using System;
using SnakeTest.Player;
using UnityEngine;

namespace SnakeTest.Objects
{
    [CreateAssetMenu(fileName = "GameController", menuName = "ScriptableObjects/GameController", order = 1)]
    public class GameEvents : ScriptableObject
    {
        public Action OnGameStart;
        public Action<int> OnGameOver;

        public Action<int, string> OnGearCaptured;
        public Action<int, string> OnRamsCaptured;
        public Action OnPowerupCaptured;
        public Action OnAIDied;

        public Action<int, string> OnGearChange;
        public Action<int, string> OnRamsChange;
        public Action<GameObject> OnNewPowerup;
        public GameObject currentPowerUp;
        public Action<Vector3> OnExplosion;
        public Action<string, SnakeController.SnakeType> OnSnakeStart;

    }
}