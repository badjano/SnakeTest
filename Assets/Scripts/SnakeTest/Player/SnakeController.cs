using System;
using System.Collections;
using System.Collections.Generic;
using SnakeTest.Objects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SnakeTest.Player
{
    public class SnakeController : MonoBehaviour
    {
        private Controls _controls;

        [SerializeField] private SnakeParts _parts;
        [SerializeField] private GameEvents _events;

        private int _snakeDir;

        private Vector3 _headForward = Vector3.up;
        private float _snakeInterval = 1.0f;

        private Quaternion _defaultRotation = Quaternion.Euler(0, 0, 90);

        struct BlockContents
        {
            public GameObject block;
            public int direction;
            public Vector3 forward;
        }

        private List<BlockContents> _snakeParts;

        void Start()
        {
            _snakeParts = new List<BlockContents>();
            var startParts = new[]
            {
                SnakeParts.BodyType.Head,
                SnakeParts.BodyType.Body,
                SnakeParts.BodyType.Tail,
            };
            for (int i = 0; i < startParts.Length; i++)
            {
                var position = new Vector3(0, -i, 0);
                _snakeParts.Add(new BlockContents()
                {
                    block = _parts.GetPart(startParts[i], 0, position),
                    direction = 0,
                    forward = Vector3.up
                });
            }

            var methods = new Dictionary<InputAction, Action<InputAction.CallbackContext>>()
            {
                {_controls.Player01.Left, ctx => _snakeDir = -1},
                {_controls.Player01.Right, ctx => _snakeDir = 1},
            };
            foreach (KeyValuePair<InputAction, Action<InputAction.CallbackContext>> kv in methods)
            {
                kv.Key.started += kv.Value;
                kv.Key.performed += kv.Value;
                kv.Key.canceled += kv.Value;
            }

            StartCoroutine(nameof(OnSnakeTick));
        }

        IEnumerator OnSnakeTick()
        {
            while (true)
            {
                yield return new WaitForSeconds(_snakeInterval);
                Move();
                _snakeDir = 0;
            }
        }

        private void Move()
        {
            switch (_snakeDir)
            {
                case -1:
                    TurnLeft();
                    break;
                case 1:
                    TurnRight();
                    break;
                default:
                    for (int i = 0; i < _snakeParts.Count; i++)
                    {
                        _snakeParts[i].block.transform.position += _snakeParts[i].forward;
                    }
                    break;
            }
        }

        private void TurnRight()
        {
            _headForward = Quaternion.Inverse(_defaultRotation) * _headForward;
            var direction = (_snakeParts[0].direction + 5) % 4;
            var position = _snakeParts[0].block.transform.position + _headForward;
            var newBlock = Instantiate(_parts.HeadPrefabs[direction], position, quaternion.identity);
            ReplaceBlock(0, new BlockContents()
            {
                block = newBlock,
                direction = direction,
                forward = _headForward
            });
        }

        private void TurnLeft()
        {
            _headForward = _defaultRotation * _headForward;
            var direction = (_snakeParts[0].direction + 3) % 4;
            var position = _snakeParts[0].block.transform.position + _headForward;
            var newBlock = Instantiate(_parts.HeadPrefabs[direction], position, quaternion.identity);
            ReplaceBlock(0, new BlockContents()
            {
                block = newBlock,
                direction = direction,
                forward = _headForward
            });
        }

        private void ReplaceBlock(int index, BlockContents contents)
        {
            Destroy(_snakeParts[index].block);
            _snakeParts[index] = contents;
        }

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
            }

            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}