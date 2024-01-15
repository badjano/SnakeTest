using System.Collections;
using System.Collections.Generic;
using SnakeTest.Objects;
using SnakeTest.Player;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private GameEvents _events;
    [SerializeField] private SnakeController _aiPrefab;
    // Start is called before the first frame update

    void OnEnable()
    {
        _events.OnAIDied += OnAIDeath;
    }

    private void OnDisable()
    {
        _events.OnAIDied -= OnAIDeath;
    }

    private void OnAIDeath()
    {
        StartCoroutine(nameof(RespawnAI));
    }

    private IEnumerator RespawnAI()
    {
        yield return new WaitForSeconds(3f);
        Instantiate(_aiPrefab, Vector3.zero, Quaternion.identity);
    }
}