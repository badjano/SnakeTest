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
            public bool isTurn;
            public bool isFilled;
            public int directionOut;
            public int directionIn;
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
                SnakeParts.BodyType.Body,
                SnakeParts.BodyType.Body,
                SnakeParts.BodyType.Tail,
            };
            for (int i = 0; i < startParts.Length; i++)
            {
                var position = new Vector3(0, -i, 0);
                _snakeParts.Add(new BlockContents()
                {
                    block = _parts.GetPart(startParts[i], 0, -1, position),
                    directionOut = 0,
                    directionIn = i == startParts.Length - 1 ? -1 : 0,
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
                // kv.Key.started += kv.Value;
                kv.Key.performed += kv.Value;
                // kv.Key.canceled += kv.Value;
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
            int i = 0;
            switch (_snakeDir)
            {
                case -1:
                    i = 1;
                    Turn(false);
                    break;
                case 1:
                    i = 1;
                    Turn(true);
                    break;
            }

            for (; i < _snakeParts.Count; i++)
            {
                if (i > 0)
                {
                    var prevDirOut = _snakeParts[i - 1].directionOut;
                    var currentDirOut = _snakeParts[i].directionOut;
                    var prevDirIn = _snakeParts[i - 1].directionIn;
                    if (i == _snakeParts.Count - 1)
                    {
                        var position = _snakeParts[i].block.transform.position + _snakeParts[i].forward.normalized;
                        var newBlock = _parts.GetPart(SnakeParts.BodyType.Tail, prevDirIn, -1, position);
                        var fwd = prevDirIn != currentDirOut ? _snakeParts[i - 1].forward : _snakeParts[i].forward;
                        ReplaceBlock(i, new BlockContents()
                        {
                            block = newBlock,
                            directionOut = prevDirIn,
                            directionIn = -1,
                            forward = fwd
                        });
                        continue;
                    }

                    if (currentDirOut != prevDirIn)
                    {
                        var position = _snakeParts[i - 1].block.transform.position -
                                       _snakeParts[i - 1].forward.normalized;
                        var newBlock = _parts.GetPart(SnakeParts.BodyType.Turn, currentDirOut, prevDirOut, position);
                        ReplaceBlock(i, new BlockContents()
                        {
                            block = newBlock,
                            directionOut = prevDirOut,
                            directionIn = currentDirOut,
                            isTurn = true,
                            forward = _snakeParts[i - 1].forward
                        });
                        continue;
                    }

                    if (_snakeParts[i].isTurn)
                    {
                        var position = _snakeParts[i].block.transform.position +
                                       _snakeParts[i].forward.normalized;
                        var newBlock = _parts.GetPart(SnakeParts.BodyType.Body, currentDirOut, -1, position);
                        ReplaceBlock(i, new BlockContents()
                        {
                            block = newBlock,
                            directionOut = currentDirOut,
                            directionIn = currentDirOut,
                            forward = _snakeParts[i].forward
                        });
                        continue;
                    }
                }

                _snakeParts[i].block.transform.position += _snakeParts[i].forward.normalized;
            }
        }

        private void Turn(bool right)
        {
            int direction;
            if (right)
            {
                _headForward = Quaternion.Inverse(_defaultRotation) * _headForward;
                direction = (_snakeParts[0].directionOut + 5) % 4;
            }
            else
            {
                _headForward = _defaultRotation * _headForward;
                direction = (_snakeParts[0].directionOut + 3) % 4;
            }

            var position = _snakeParts[0].block.transform.position + _headForward;
            var newBlock = _parts.GetPart(SnakeParts.BodyType.Head, direction, -1, position);
            ReplaceBlock(0, new BlockContents()
            {
                block = newBlock,
                directionOut = direction,
                directionIn = direction,
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