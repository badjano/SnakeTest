using System;
using System.Collections;
using SnakeTest.Objects;
using UnityEngine;

namespace SnakeTest.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameEvents _events;
        private void Start()
        {
            _events.OnGameStart?.Invoke(0.0f);
        }

        private void Update()
        {
        }
    }
}