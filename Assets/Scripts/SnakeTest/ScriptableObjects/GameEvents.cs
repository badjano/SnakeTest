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
        public Action<int, string> OnClockCaptured;
        
        public Action OnPowerupCaptured;
        public Action OnAIDied;

        public Action<int, string> OnGearChange;
        public Action<int, string> OnRamsChange;
        public Action<int, string> OnClockChange;
        
        public Action<string> OnClockDeath;
        
        public Action<GameObject> OnNewPowerup;
        public GameObject currentPowerUp;
        public Action<Vector3, int> OnExplosion;
        public Action<string, SnakeController.SnakeType> OnSnakeStart;


        private void OnEnable()
        {
            OnNewPowerup += OnNewPowerupHandler;
        }

        private void OnDisable()
        {
            OnNewPowerup -= OnNewPowerupHandler;
        }

        private void OnNewPowerupHandler(GameObject powerup)
        {
            currentPowerUp = powerup;
        }
    }
}