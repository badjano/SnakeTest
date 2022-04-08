using System.Collections;
using System.Collections.Generic;
using SnakeTest.Objects;
using TMPro;
using UnityEngine;

namespace SnakeTest.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameEvents _events;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private GameObject _gameOver;
        
        private int _gear;
        private int _rams;

        // Start is called before the first frame update
        void Start()
        {
            _events.OnGearChange += OnGearChange;
            _events.OnRamsChange += OnRamsChange;
            _events.OnGameOver += OnGameOver;
        }

        private void OnGameOver()
        {
            _gameOver.SetActive(true);
        }

        private void OnRamsChange(int increment)
        {
            _rams += increment;
            UpdateText();
        }

        private void OnGearChange(int increment)
        {
            _gear += increment;
            UpdateText();
        }

        private void UpdateText()
        {
            _text.text = $"ENGINES: {_gear}\nRAMS: {_rams}";
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}