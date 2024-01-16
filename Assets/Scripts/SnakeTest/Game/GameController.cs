using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SnakeTest.Objects;
using SnakeTest.Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace SnakeTest.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameEvents _events;
        [SerializeField] private Powerups _powerups;
        [SerializeField] private SnakeParts _parts;
        [SerializeField] private GameObject _snakePrefab;
        [SerializeField] private GameObject _particlesPrefab;
        [SerializeField] private Material[] _particlesMaterials;
        
        private float _totalOdds;
        private Dictionary<string, List<SnakeState>> _snakeStates;

        private void Start()
        {
            _events.OnGameStart?.Invoke();
            _totalOdds = _powerups.powerups
                .Select(x => x.odds)
                .Aggregate((x, y) => x + y);
            CreateNewPowerUP();
        }

        private void OnEnable()
        {
            _events.OnPowerupCaptured += OnPowerUpCaptured;
            _events.OnExplosion += OnExplosion;
            _events.OnClockCaptured += OnClockCaptured;
            _events.OnClockDeath += OnClockDeath;
        }

        private void OnDisable()
        {
            _events.OnPowerupCaptured -= OnPowerUpCaptured;
            _events.OnExplosion -= OnExplosion;
            _events.OnClockCaptured -= OnClockCaptured;
            _events.OnClockDeath -= OnClockDeath;
        }

        private void OnClockCaptured(int increment, string uid)
        {
            if (increment > 0)
                SaveState(uid);
        }

        private void OnClockDeath(string uid)
        {
            LoadState(uid);
            Debug.Log($"<color=lime>clock Used = false</color>");
            _powerups.powerups.ForEach(x => x.Used = false);
        }

        private void OnExplosion(Vector3 position, int materialIndex = 0)
        {
            var particles = Instantiate(_particlesPrefab, position, Quaternion.identity);
            if (_particlesMaterials.Length == 0)
            {
                return;
            }

            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.material = _particlesMaterials[materialIndex];
        }

        private void OnPowerUpCaptured()
        {
            CreateNewPowerUP();
        }

        private void CreateNewPowerUP()
        {
            var rand = Random.Range(0f, _totalOdds);
            for (int i = 0; i < _powerups.powerups.Count; i++)
            {
                var powerup = _powerups.powerups[i];
                if (rand <= powerup.odds && !powerup.Used)
                {
                    var x = Random.Range(-10, 11);
                    var y = Random.Range(-7, 8);
                    var pos = new Vector3(x, y, 0);
                    var newPowerUp = Instantiate(powerup.powerup, pos, Quaternion.identity);
                    if (powerup.once)
                        powerup.Used = true;
                    if (powerup.powerup.name == "clock")
                        Debug.Log($"<color=red>clock Used = true</color>");
                    _events.OnNewPowerup?.Invoke(newPowerUp);
                    return;
                }

                rand -= powerup.odds;
            }

            Debug.LogError("No powerup found");
        }

        public static void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static void EndGame()
        {
            Application.Quit();
        }

        public void SaveState(string key)
        {
            if (_snakeStates == null)
                _snakeStates = new Dictionary<string, List<SnakeState>>();
            if (!_snakeStates.ContainsKey(key))
                _snakeStates.Add(key, new List<SnakeState>());
            _snakeStates[key].Clear();
            var snakes = SnakeController.GetSnakes();
            for (int i = 0; i < snakes.Count; i++)
            {
                var snake = snakes[i];
                var state = new SnakeState();
                state.snakeType = snake.GetSnakeType();
                state.name = snake.name;
                state.parent = snake.transform.parent;
                state.snakeDir = snake.GetDirection();
                state.forward = snake.GetHeadForward();
                var parts = snake.GetParts();
                state.blocks = new List<SnakeController.SnakeBlock>();
                for (int j = 0; j < parts.Count; j++)
                {
                    var newBlock = parts[j];
                    newBlock.block = null;
                    newBlock.arguments.position = parts[j].block.transform.position;
                    state.blocks.Add(newBlock);
                }

                _snakeStates[key].Add(state);
            }
        }

        public void LoadState(string key)
        {
            if (_snakeStates == null)
                return;
            if (!_snakeStates.ContainsKey(key))
                return;
            
            Debug.Log($"Loading state for {key}");
            
            while (SnakeController.GetSnakes().Count > 0)
            {
                SnakeController.GetSnakes()[0].DestroySnake(1);
            }

            for (int snakeIndex = 0; snakeIndex < _snakeStates[key].Count; snakeIndex++)
            {
                var state = _snakeStates[key][snakeIndex];
                var snake = Instantiate(_snakePrefab).GetComponent<SnakeController>();
                snake.name = state.name;
                snake.transform.parent = state.parent;
                snake.SetSnakeType(state.snakeType);
                snake.SetDirection(state.snakeDir);
                snake.SetHeadForward(state.forward);
                for (int i = 0; i < state.blocks.Count; i++)
                {
                    var stateBlock = state.blocks[i];
                    stateBlock.block = _parts.GetPart(stateBlock.arguments);
                    state.blocks[i] = stateBlock;
                }

                snake.SetParts(new List<SnakeController.SnakeBlock>(state.blocks));
            }
        }
    }

    public struct SnakeState
    {
        [FormerlySerializedAs("_snakeDir")] public IndexDirection snakeDir;
        public List<SnakeController.SnakeBlock> blocks;
        public SnakeController.SnakeType snakeType;
        public string name;
        public Transform parent;
        public Vector3 forward;
    }
}