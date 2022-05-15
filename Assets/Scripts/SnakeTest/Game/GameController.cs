using System;
using System.Collections;
using System.Linq;
using SnakeTest.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace SnakeTest.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameEvents _events;
        [SerializeField] private Powerups _powerups;
        private float _totalOdds;

        [SerializeField]
        private void Start()
        {
            _events.OnGameStart?.Invoke();
            _events.OnPowerupCaptured += OnPowerUpCaptured;
            _totalOdds = _powerups.powerups
                .Select(x => x.odds)
                .Aggregate((x, y) => x + y);
            CreateNewPowerUP();
        }

        private void OnDestroy()
        {
            _events.OnPowerupCaptured -= OnPowerUpCaptured;
        }

        private void OnPowerUpCaptured()
        {
            var powerup = CreateNewPowerUP();
            if (powerup != null)
            {
                _events.OnNewPowerup?.Invoke(powerup);
            }
        }

        private GameObject CreateNewPowerUP()
        {
            var rand = Random.Range(0f, _totalOdds);
            for (int i = 0; i < _powerups.powerups.Count; i++)
            {
                var powerup = _powerups.powerups[i];
                if (rand <= powerup.odds)
                {
                    Debug.Log($"_totalOdds: {_totalOdds} - Rand: {rand} - Creating powerup {powerup.powerup.name} {powerup.odds}");
                    var x = Random.Range(-10, 11);
                    var y = Random.Range(-7, 8);
                    var pos = new Vector3(x, y, 0);
                    var newPowerUp = Instantiate(powerup.powerup, pos, Quaternion.identity);
                    return newPowerUp;
                }
                rand -= powerup.odds;
            }

            return null;
        }

        public static void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public static void EndGame()
        {
            Application.Quit();
        }
    }
}