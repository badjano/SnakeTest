using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnakeTest.Game
{
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        [SerializeField] private TMP_Text _message;
        // Start is called before the first frame update
        void Start()
        {
            _yesButton.onClick.AddListener(() =>
            {
                GameController.RestartGame();
            });
            _noButton.onClick.AddListener(() =>
            {
                GameController.EndGame();
            });
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void SetMessage(string message)
        {
            _message.text = message;
        }
    }
}