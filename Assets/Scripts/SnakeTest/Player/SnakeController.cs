using System;
using System.Collections;
using System.Collections.Generic;
using SnakeTest.Objects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SnakeTest.Player
{
    public class SnakeController : MonoBehaviour
    {
        enum SnakeType
        {
            Player1,
            Player2,
            AI
        }

        private Controls _controls;

        [SerializeField] private SnakeParts _parts;
        [SerializeField] private GameEvents _events;
        [SerializeField] private SnakeType _snakeType;
        private string _layerName;

        private int _snakeDir;

        private Vector3 _headForward = Vector3.up;
        private float _snakeInterval = 0.666f;

        private Quaternion _defaultRotation = Quaternion.Euler(0, 0, 90);

        struct BlockContents
        {
            public GameObject block;
            public bool isTurn;
            public bool isFilled;
            public int directionOut;
            public int directionIn;
            public Vector3 forward;
            public Vector3 lastForward;
        }

        private List<BlockContents> _snakeParts;
        private NavMeshAgent _agent;
        private Transform _target;
        private int? _gears;
        private int? _rams;
        private bool _addBody;

        private void Awake()
        {
            _events.OnGameOver += OnGameOver;
            _events.OnNewPowerup += OnNewPowerup;
            _events.OnGearChange += OnGearChange;
            _events.OnRamsChange += OnRamsChange;
        }

        private void OnDestroy()
        {
            _events.OnGameOver -= OnGameOver;
            _events.OnNewPowerup -= OnNewPowerup;
            _events.OnGearChange -= OnGearChange;
            _events.OnRamsChange -= OnRamsChange;
        }

        private void OnRamsChange(int rams)
        {
            _rams = rams;
        }

        private void OnGearChange(int gears)
        {
            _gears = gears;
        }

        void Start()
        {
            var methods = new Dictionary<InputAction, Action<InputAction.CallbackContext>>();
            switch (_snakeType)
            {
                case SnakeType.Player2:
                    _layerName = "Player2";
                    methods.Add(_controls.Player02.Left, ctx => _snakeDir = -1);
                    methods.Add(_controls.Player02.Right, ctx => _snakeDir = 1);
                    break;
                case SnakeType.Player1:
                    _layerName = "Player1";
                    methods.Add(_controls.Player01.Left, ctx => _snakeDir = -1);
                    methods.Add(_controls.Player01.Right, ctx => _snakeDir = 1);
                    break;
                default:
                    _layerName = "AI";
                    _agent = gameObject.AddComponent<NavMeshAgent>();
                    _agent.updateRotation = false;
                    _agent.updateUpAxis = false;
                    _agent.updatePosition = false;
                    break;
            }

            foreach (KeyValuePair<InputAction, Action<InputAction.CallbackContext>> kv in methods)
            {
                // kv.Key.started += kv.Value;
                kv.Key.performed += kv.Value;
                // kv.Key.canceled += kv.Value;
            }

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
                var block = _parts.GetPart(startParts[i], 0, -1, position);
                block.layer = GetLayerMaskInt();
                block.name = "SnakePart" + i;
                _snakeParts.Add(new BlockContents()
                {
                    block = block,
                    directionOut = 0,
                    directionIn = i == startParts.Length - 1 ? -1 : 0,
                    forward = Vector3.up,
                    lastForward = Vector3.up
                });
            }

            StartCoroutine(nameof(OnSnakeTick));
        }

        private void OnNewPowerup(GameObject powerup)
        {
            _target = powerup.transform;
        }

        private void OnGameOver()
        {
            StopCoroutine(nameof(OnSnakeTick));
        }

        IEnumerator OnSnakeTick()
        {
            while (true)
            {
                yield return new WaitForSeconds(_snakeInterval * 0.5f);
                CheckCollisions();
                yield return new WaitForSeconds(_snakeInterval * 0.5f);
                Move();
                _snakeDir = 0;
            }
        }

        private void CheckCollisions()
        {
            for (int i = 0; i < _snakeParts.Count; i++)
            {
                var snakePartTransform = _snakeParts[i].block.transform;
                // var layerMask = ~(1 << LayerMask.NameToLayer(_layerName));
                var colliders = Physics.OverlapSphere(snakePartTransform.position, 0.2f);
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Gear"))
                    {
                        _events.OnGearCaptured?.Invoke(1);
                        _addBody = true;
                        DestroyImmediate(collider.gameObject);
                    }
                    else if (collider.CompareTag("Ram"))
                    {
                        _events.OnRamsCaptured?.Invoke(1);
                        DestroyImmediate(collider.gameObject);
                    }
                    else if (collider.name != snakePartTransform.name)
                    {
                        if (_rams > 0)
                        {
                            _events.OnRamsCaptured?.Invoke(-1);
                        }
                        else
                        {
                            _events.OnGameOver?.Invoke();
                        }
                    }
                }
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

            if (_addBody)
            {
                _addBody = false;
                if (i == 0)
                {
                    _snakeParts[i].block.transform.position =
                        _parts.ModPosition(_snakeParts[i].block.transform.position + _snakeParts[i].forward.normalized);
                    i++;
                }

                var position = _snakeParts[0].block.transform.position - _headForward;
                var out_ = _snakeParts[0].directionIn;
                var in_ = _snakeParts[i].directionOut;
                var block = _parts.GetPart(SnakeParts.BodyType.BodyFilled, out_, in_, position);
                block.layer = GetLayerMaskInt();
                block.name = "SnakePart" + i;
                _snakeParts.Insert(1, new BlockContents()
                {
                    block = block,
                    directionOut = out_,
                    directionIn = in_,
                    forward = _headForward,
                    lastForward = _snakeParts[0].forward
                });
            }
            else
            {
                for (; i < _snakeParts.Count; i++)
                {
                    if (i > 0)
                    {
                        var prevDirOut = _snakeParts[i - 1].directionOut;
                        var currentDirOut = _snakeParts[i].directionOut;
                        var prevDirIn = _snakeParts[i - 1].directionIn;
                        var position = _snakeParts[i].block.transform.position + _snakeParts[i].forward.normalized;
                        var fwd = prevDirIn != currentDirOut
                            ? _snakeParts[i - 1].isTurn ? _snakeParts[i - 1].lastForward : _snakeParts[i - 1].forward
                            : _snakeParts[i].forward;
                        if (i == _snakeParts.Count - 1)
                        {
                            var newBlock = _parts.GetPart(SnakeParts.BodyType.Tail, prevDirIn, -1, position);
                            newBlock.layer = GetLayerMaskInt();
                            newBlock.name = "Tail" + i;
                            ReplaceBlock(i, new BlockContents()
                            {
                                block = newBlock,
                                directionOut = prevDirIn,
                                directionIn = -1,
                                forward = fwd,
                                lastForward = _snakeParts[i].forward
                            });
                            continue;
                        }

                        if (_snakeParts[i].isTurn)
                        {
                            if (currentDirOut != prevDirIn)
                            {
                                var newBlock = _parts.GetPart(SnakeParts.BodyType.Turn, currentDirOut, prevDirIn,
                                    position);
                                newBlock.layer = GetLayerMaskInt();
                                newBlock.name = "WasTurnAndStillTurn" + i;
                                ReplaceBlock(i, new BlockContents()
                                {
                                    block = newBlock,
                                    directionOut = prevDirIn,
                                    directionIn = currentDirOut,
                                    isTurn = true,
                                    forward = fwd,
                                    lastForward = _snakeParts[i].forward
                                });
                            }
                            else
                            {
                                var newBlock = _parts.GetPart(SnakeParts.BodyType.Body, currentDirOut, -1, position);
                                newBlock.layer = GetLayerMaskInt();
                                newBlock.name = "WasTurnAndNotTurn" + i;
                                ReplaceBlock(i, new BlockContents()
                                {
                                    block = newBlock,
                                    directionOut = currentDirOut,
                                    directionIn = currentDirOut,
                                    forward = fwd,
                                    lastForward = _snakeParts[i].forward
                                });
                            }

                            continue;
                        }

                        if (currentDirOut != prevDirIn)
                        {
                            var newBlock = _parts.GetPart(SnakeParts.BodyType.Turn, currentDirOut, prevDirIn, position);
                            newBlock.layer = GetLayerMaskInt();
                            newBlock.name = "IsNowTurn" + i;
                            ReplaceBlock(i, new BlockContents()
                            {
                                block = newBlock,
                                directionOut = prevDirIn,
                                directionIn = currentDirOut,
                                isTurn = true,
                                forward = fwd,
                                lastForward = _snakeParts[i].forward
                            });
                            continue;
                        }
                    }

                    _snakeParts[i].block.transform.position =
                        _parts.ModPosition(_snakeParts[i].block.transform.position + _snakeParts[i].forward.normalized);
                }
            }
        }

        private int GetLayerMaskInt()
        {
            return LayerMask.NameToLayer(_layerName);
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
            newBlock.layer = GetLayerMaskInt();
            newBlock.name = "Head";
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
            DestroyImmediate(_snakeParts[index].block);
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
            if (_snakeType == SnakeType.AI)
            {
                var pos = _target.position;
                _agent.SetDestination(pos);
                if (_agent.hasPath)
                {
                    var head = _snakeParts[0].block.transform.position;
                    var direction = (_agent.nextPosition - head).normalized;
                    direction = new Vector3(
                        Mathf.Round(direction.x),
                        Mathf.Round(direction.y),
                        0
                    );
                    var directionInt = -1;
                    if (direction == Vector3.up)
                    {
                        directionInt = 0;
                    }
                    else if (direction == Vector3.right)
                    {
                        directionInt = 1;
                    }
                    else if (direction == Vector3.down)
                    {
                        directionInt = 2;
                    }
                    else if (direction == Vector3.left)
                    {
                        directionInt = 3;
                    }

                    Debug.Log($"{directionInt}, {_snakeParts[0].directionOut}");

                    Debug.DrawLine(head, head + direction * 5, Color.red);
                }
            }
        }
    }
}