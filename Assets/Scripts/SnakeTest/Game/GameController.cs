using System;
using System.Collections;
using System.Linq;
using SnakeTest.Objects;
using UnityEngine;
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
            _events.OnRamsChange += OnPowerUpChange;
            _events.OnGearChange += OnPowerUpChange;
            CreateNewPowerUP();
            _totalOdds = _powerups.powerups
                .Select(x => x.odds)
                .Aggregate((x, y) => x + y);
        }

        private void OnPowerUpChange(int obj)
        {
            var powerup = CreateNewPowerUP();
        }

        private void Update()
        {
        }

        private GameObject CreateNewPowerUP()
        {
            var rand = Random.Range(0, _totalOdds);
            for (int i = 0; i < _powerups.powerups.Count; i++)
            {
                var powerup = _powerups.powerups[i];
                if (rand < powerup.odds)
                {
                    var x = Random.Range(-10, 11);
                    var y = Random.Range(-7, 8);
                    var pos = new Vector3(x, y, 0);
                    var newPowerUp = Instantiate(powerup.powerup, pos, Quaternion.identity);
                    _events.OnNewPowerup?.Invoke(newPowerUp);
                    return newPowerUp;
                }
            }

            return null;
        }
    }
}