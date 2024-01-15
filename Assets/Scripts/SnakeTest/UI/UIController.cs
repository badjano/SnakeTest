using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SnakeTest.Game;
using SnakeTest.Objects;
using SnakeTest.Player;
using TMPro;
using UnityEngine;

namespace SnakeTest.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameEvents _events;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _text2;
        [SerializeField] private GameOverController _gameOver;

        private Dictionary<string, int> _gear;
        private Dictionary<string, int> _rams;
        private string _player1;
        private string _player2;

        // Start is called before the first frame update
        void OnEnable()
        {
            _events.OnGearCaptured += OnGearCaptured;
            _events.OnRamsCaptured += OnRamsCaptured;
            _events.OnGameOver += OnGameOver;
            _events.OnSnakeStart += OnSnakeStart;
        }

        private void OnDisable()
        {
            _events.OnGearCaptured -= OnGearCaptured;
            _events.OnRamsCaptured -= OnRamsCaptured;
            _events.OnGameOver -= OnGameOver;
            _events.OnSnakeStart -= OnSnakeStart;
        }

        private void OnSnakeStart(string uid, SnakeController.SnakeType snakeType)
        {
            switch (snakeType)
            {
                case SnakeController.SnakeType.Player1:
                    _player1 = uid;
                    break;
                case SnakeController.SnakeType.Player2:
                    _player2 = uid;
                    break;
            }

            if (_gear == null)
            {
                _gear = new Dictionary<string, int>();
                _rams = new Dictionary<string, int>();
            }

            _gear.Add(uid, 0);
            _rams.Add(uid, 0);
            UpdateText();
        }

        private void OnGameOver(int playerIndex)
        {
            _gameOver.gameObject.SetActive(true);
            _gameOver.SetMessage($"Player {playerIndex + 1} WINS!");
        }

        private void OnRamsCaptured(int increment, string uid)
        {
            _rams[uid] += increment;
            _events.OnRamsChange(_rams[uid], uid);
            if (increment > 0)
                _events.OnPowerupCaptured();
            UpdateText();
        }

        private void OnGearCaptured(int increment, string uid)
        {
            _gear[uid] += increment;
            _events.OnGearChange(_gear[uid], uid);
            if (increment > 0)
                _events.OnPowerupCaptured();
            UpdateText();
        }

        private void UpdateText()
        {
            if (_player1 != null)
                _text.text = $"PLAYER 1\nENGINES: {_gear[_player1]:000}\nRAMS: {_rams[_player1]:000}";
            if (_player2 != null)
                _text2.text = $"PLAYER 2\nENGINES: {_gear[_player2]:000}\nRAMS: {_rams[_player2]:000}";
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}