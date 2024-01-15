using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SnakeTest.Misc;
using SnakeTest.Objects;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace SnakeTest.Player
{
    public class SnakeController : MonoBehaviour
    {
        public enum SnakeType
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

        private int _snakeTurn;

        private Vector3 _headForward = Vector3.up;
        private float _snakeInterval = 0.2666f;
        private float _gearMultiplier = 0.95f;

        private Quaternion _defaultRotation = Quaternion.Euler(0, 0, 90);

        struct BlockContents
        {
            public GameObject block;
            public bool isTurn;
            public bool isFilled;
            public IndexDirection directionOut;
            public IndexDirection directionIn;
            public Vector3 forward;
            public Vector3 lastForward;
        }

        private List<BlockContents> _snakeParts;
        private NavMeshAgent _agent;
        private Transform _target;
        private int _gears = 0;
        private int _rams = 0;
        private bool _addBody;
        private bool _waitAddBody;
        private List<int> _indexFilled;
        private int _colorIndex;
        private NavMeshPath _path;
        private IndexDirection _snakeDirection;

        private static List<SnakeController> _snakes = new();
        private float _time;
        private NavMeshObstacle _navMeshObstacle;
        private string uid;

        private void Awake()
        {
            uid = Guid.NewGuid().ToString();
            _events.OnGameOver += OnGameOver;
            _events.OnNewPowerup += OnNewPowerup;
            _events.OnGearChange += OnGearChange;
            _events.OnRamsChange += OnRamsChange;
            _snakes.Add(this);
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
        }

        private void OnDestroy()
        {
            _events.OnGameOver -= OnGameOver;
            _events.OnNewPowerup -= OnNewPowerup;
            _events.OnGearChange -= OnGearChange;
            _events.OnRamsChange -= OnRamsChange;
            _snakes.Remove(this);
        }

        private void OnRamsChange(int rams, string s)
        {
            if (!uid.Equals(s))
                return;
            var before = _rams;
            _rams = rams;
            if (before == 0 && _rams > 0)
            {
                ChangeHead(SnakeParts.BodyType.HeadRam);
            }
            else if (before > 0 && _rams == 0)
            {
                ChangeHead(SnakeParts.BodyType.Head);
            }
        }

        private void ChangeHead(SnakeParts.BodyType headType)
        {
            var part = _snakeParts[0];
            var newBlock = _parts.GetPart(headType, part.directionOut, IndexDirection.Right,
                part.block.transform.position, _colorIndex);
            ReplaceBlock(0, new BlockContents
            {
                block = newBlock,
                directionOut = part.directionOut,
                directionIn = part.directionIn,
                forward = part.forward,
                lastForward = part.lastForward
            });
        }

        private void OnGearChange(int gears, string s)
        {
            if (!uid.Equals(s))
                return;
            _gears = gears;
        }

        void Start()
        {
            _events.OnSnakeStart?.Invoke(uid, _snakeType);
            if (_events.currentPowerUp != null)
                _target = _events.currentPowerUp.transform;
            _indexFilled = new List<int>();
            var methods = new Dictionary<InputAction, Action<InputAction.CallbackContext>>();
            switch (_snakeType)
            {
                case SnakeType.Player1:
                    _layerName = "Player1";
                    methods.Add(_controls.Player01.Left, ctx => _snakeTurn = -1);
                    methods.Add(_controls.Player01.Right, ctx => _snakeTurn = 1);
                    _colorIndex = 0;
                    break;
                case SnakeType.Player2:
                    _layerName = "Player2";
                    methods.Add(_controls.Player02.Left, ctx => _snakeTurn = -1);
                    methods.Add(_controls.Player02.Right, ctx => _snakeTurn = 1);
                    _colorIndex = 1;
                    break;
                default:
                    _layerName = "AI";
                    _agent = gameObject.AddComponent<NavMeshAgent>();
                    _agent.updateRotation = false;
                    _agent.updateUpAxis = false;
                    _agent.updatePosition = true;
                    _path = new NavMeshPath();
                    _colorIndex = 2;
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
                var position = transform.position + new Vector3(0, -i, 0);
                var block = _parts.GetPart(startParts[i], 0, IndexDirection.Left, position, _colorIndex);
                block.layer = GetLayerMaskInt();
                block.name = "SnakePart" + i;
                _snakeParts.Add(new BlockContents
                {
                    block = block,
                    directionOut = 0,
                    directionIn = i == startParts.Length - 1 ? IndexDirection.Down : IndexDirection.Up,
                    forward = Vector3.up,
                    lastForward = Vector3.up
                });
            }
            
            _time = 0;
        }

        private void OnNewPowerup(GameObject powerup)
        {
            _target = powerup.transform;
        }

        private void OnGameOver(int playerIndex)
        {
            Destroy(this);
        }

        private bool CheckCollisions(Vector3 position, bool dryRun = false)
        {
            foreach (var snakeController in _snakes)
            {
                var bounds = snakeController.GetBounds();
                if (!bounds.Contains(position))
                    continue;
                var otherSnakeParts = snakeController._snakeParts;
                foreach (var otherSnakePart in otherSnakeParts)
                {
                    if (otherSnakePart.block == _snakeParts[0].block)
                        continue;
                    var distance = Vector3.Distance(position, otherSnakePart.block.transform.position);
                    if (distance < 0.5f)
                    {
                        if (_rams > 0)
                        {
                            if (!dryRun)
                            {
                                _events.OnRamsCaptured?.Invoke(-1, uid);
                                _events.OnExplosion?.Invoke(position);
                            }
                            return true;
                        }
                        return false;
                    }
                }
            }
            
            if (_target == null)
                return !dryRun;

            var powerupDistance = Vector3.Distance(position, _target.position);
            if (powerupDistance < 0.5f)
            {
                var isGear = _target.CompareTag("Gear");
                var pos = _target.position;
                DestroyImmediate(_target.gameObject);
                _events.OnExplosion?.Invoke(pos);
                if (isGear)
                {
                    _events.OnGearCaptured?.Invoke(1, uid);
                    _snakeInterval *= _gearMultiplier;
                }
                else
                {
                    _events.OnRamsCaptured?.Invoke(1, uid);
                }

                _addBody = true;
                _waitAddBody = true;
            }

            return true;
        }

        private void Explode()
        {
            var partsPositions = _snakeParts.Select(x => x.block.transform.position).ToList();
            foreach (var partsPosition in partsPositions)
            {
                _events.OnExplosion?.Invoke(partsPosition);
            }

            foreach (var part in _snakeParts)
            {
                DestroyImmediate(part.block);
            }
            
            _snakeParts.Clear();

            DestroyImmediate(gameObject);
        }

        private Bounds GetBounds()
        {
            return _snakeParts.Select(x => new Bounds(x.block.transform.position, Vector3.one*2)).Aggregate((x, y) =>
            {
                x.Encapsulate(y);
                return x;
            });
        }

        private void Move()
        {
            _indexFilled = _indexFilled.Select(i => i + 1).Where(i => i < _snakeParts.Count).ToList();
            int i = 0;
            switch (_snakeTurn)
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

            if (_addBody) // just ate a gear
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
                var currentDirOut = _snakeParts[i].directionOut;
                var prevDirIn = _snakeParts[i - 1].directionIn;
                var turned = in_ != out_;
                var bodyType = turned ? SnakeParts.BodyType.TurnFilled : SnakeParts.BodyType.BodyFilled;
                var block = _parts.GetPart(bodyType, currentDirOut, prevDirIn, position, _colorIndex);
                block.layer = GetLayerMaskInt();
                block.name = "SnakePartFilled" + i;
                _indexFilled.Add(i);
                _snakeParts.Insert(1, new BlockContents
                {
                    block = block,
                    directionOut = out_,
                    directionIn = in_,
                    isTurn = turned,
                    forward = _headForward,
                    lastForward = _snakeParts[0].forward
                });
            }
            else
            {
                for (; i < _snakeParts.Count; i++)
                {
                    var filled = _indexFilled.Contains(i);
                    var wasFilled = _indexFilled.Contains(i + 1);

                    if (i > 0)
                    {
                        var currentDirOut = _snakeParts[i].directionOut;
                        var prevDirIn = _snakeParts[i - 1].directionIn;

                        var isTurn = currentDirOut != prevDirIn;

                        var position = _parts.ModPosition(_snakeParts[i].block.transform.position + _snakeParts[i].forward.normalized);
                        var fwd = prevDirIn != currentDirOut
                            ? _snakeParts[i - 1].isTurn ? _snakeParts[i - 1].lastForward : _snakeParts[i - 1].forward
                            : _snakeParts[i].forward;
                        if (i == _snakeParts.Count - 1) // tail
                        {
                            var newBlock = _parts.GetPart(SnakeParts.BodyType.Tail, prevDirIn, IndexDirection.Right,
                                position,
                                _colorIndex);
                            newBlock.layer = GetLayerMaskInt();
                            newBlock.name = "Tail" + i;
                            ReplaceBlock(i, new BlockContents
                            {
                                block = newBlock,
                                directionOut = prevDirIn,
                                directionIn = IndexDirection.Down,
                                forward = fwd,
                                lastForward = _snakeParts[i].forward
                            });
                            continue; // no need to check anything else
                        }

                        if (_snakeParts[i].isTurn) // current is turn, so we possibly need to switch blocks
                        {
                            var bodyType = isTurn ? filled ? SnakeParts.BodyType.TurnFilled : SnakeParts.BodyType.Turn :
                                filled ? SnakeParts.BodyType.BodyFilled : SnakeParts.BodyType.Body;
                            prevDirIn = isTurn ? prevDirIn : IndexDirection.Down;
                            var newBlock = _parts.GetPart(bodyType, currentDirOut, prevDirIn, position, _colorIndex);
                            newBlock.layer = GetLayerMaskInt();
                            if (isTurn)
                                newBlock.name = "WasTurnAndStillTurn" + i;
                            else
                                newBlock.name = "WasTurnAndNotTurn" + i;
                            ReplaceBlock(i, new BlockContents
                            {
                                block = newBlock,
                                directionOut = isTurn ? prevDirIn : currentDirOut,
                                directionIn = currentDirOut,
                                isTurn = isTurn,
                                forward = fwd,
                                lastForward = _snakeParts[i].forward
                            });

                            continue;
                        }

                        if (filled || wasFilled) // check if it was filled or will be filled
                        {
                            var bodyType = isTurn ? filled ? SnakeParts.BodyType.TurnFilled : SnakeParts.BodyType.Turn :
                                filled ? SnakeParts.BodyType.BodyFilled : SnakeParts.BodyType.Body;
                            prevDirIn = isTurn ? prevDirIn : IndexDirection.Up;
                            var newBlock = _parts.GetPart(bodyType, currentDirOut, prevDirIn, position, _colorIndex);
                            newBlock.layer = GetLayerMaskInt();
                            newBlock.name = filled ? "NowFilled" + i : "WasFilled" + i;
                            ReplaceBlock(i, new BlockContents
                            {
                                block = newBlock,
                                directionOut = isTurn ? prevDirIn : currentDirOut,
                                directionIn = currentDirOut,
                                isTurn = isTurn,
                                forward = fwd,
                                lastForward = _snakeParts[i].forward
                            });
                            continue;
                        }

                        if (isTurn) // new turn
                        {
                            var bodyType = filled
                                ? SnakeParts.BodyType.TurnFilled
                                : SnakeParts.BodyType.Turn;
                            var newBlock = _parts.GetPart(bodyType, currentDirOut, prevDirIn, position, _colorIndex);
                            newBlock.layer = GetLayerMaskInt();
                            newBlock.name = "IsNowTurn" + i;
                            ReplaceBlock(i, new BlockContents
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
            if (right)
            {
                _headForward = Quaternion.Inverse(_defaultRotation) * _headForward;
                _snakeDirection = (IndexDirection)(((int)_snakeParts[0].directionOut + 1) % 4);
            }
            else
            {
                _headForward = _defaultRotation * _headForward;
                _snakeDirection = (IndexDirection)(((int)_snakeParts[0].directionOut + 3) % 4);
            }

            var position = _snakeParts[0].block.transform.position + _headForward;
            var newBlock = _parts.GetPart(_rams > 0 ? SnakeParts.BodyType.HeadRam : SnakeParts.BodyType.Head,
                _snakeDirection, _snakeDirection, position, _colorIndex);
            newBlock.layer = GetLayerMaskInt();
            newBlock.name = "Head";
            ReplaceBlock(0, new BlockContents
            {
                block = newBlock,
                directionOut = _snakeDirection,
                directionIn = _snakeDirection,
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

        private void Update()
        {
            if (_snakeParts == null || _snakeParts.Count == 0 || _snakeParts[0].block == null)
                return;
            _time += Time.deltaTime;
            if (_time >= _snakeInterval)
            {
                if (_waitAddBody)
                {
                    _time -= _snakeInterval * 0.5f;
                    _waitAddBody = false;
                }
                else
                {
                    if (_snakeType == SnakeType.AI)
                        AIThinking();
                    Move();
                    var headPos = _snakeParts[0].block.transform.position;
                    if (!CheckCollisions(headPos))
                    {
                        Explode();
                        int playerWonIndex;
                        switch (_snakeType)
                        {
                            case SnakeType.AI:
                                _events.OnAIDied?.Invoke();
                                return;
                            case SnakeType.Player1:
                                playerWonIndex = 1; // player 2 won
                                break;
                            default:
                                playerWonIndex = 0; // player 1 won
                                break;
                        }
                        _events.OnGameOver?.Invoke(playerWonIndex);
                    }
                    _snakeTurn = 0;
                    _time %= _snakeInterval;
                }
            }
        }

        void AIThinking()
        {
            var headPosition = _snakeParts[0].block.transform.position;
            _agent.transform.position = headPosition;
            if (_target == null)
                return;
            var targetPosition = _target.position;
            Vector3 direction = Vector3.up;
            var distance = Vector3.Distance(headPosition, targetPosition);
            if (distance < 4)
            {
                direction = (targetPosition - headPosition);
            }
            else if (_agent.CalculatePath(targetPosition, _path))
            {
                var corners = _path.corners;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    var a = corners[i];
                    var b = corners[i + 1];
                    Debug.DrawLine(a, b, Color.red, _snakeInterval);
                }

                var index = 0;

                direction = Vector3.zero;

                while (direction.magnitude < 3f && index < corners.Length)
                {
                    direction = (corners[index] - headPosition);
                    index++;
                }
            }

            Debug.DrawLine(headPosition, headPosition + direction * 3, Color.green, _snakeInterval);

            var directionInt = GetDirectionFromVector(direction);
            int tries = 0;
            Vector3 vectorDirection = Constants.vectorMap[directionInt];
            while (!CheckCollisions(headPosition + vectorDirection, true) && tries < 4)
            {
                directionInt = (IndexDirection)(((int)directionInt + 1) % 4);
                vectorDirection = Constants.vectorMap[directionInt];
                tries++;
            }

            var right = (IndexDirection)(((int)_snakeParts[0].directionOut + 1) % 4);
            _snakeTurn = directionInt == _snakeDirection ? 0 : directionInt != right ? -1 : 1;
        }

        IndexDirection GetDirectionFromVector(Vector3 direction)
        {
            IndexDirection directionInt = IndexDirection.Up;

            if (direction.y > 0)
            {
                if (direction.x > 0)
                {
                    directionInt = Mathf.Abs(direction.y) > Mathf.Abs(direction.x)
                        ? IndexDirection.Up
                        : IndexDirection.Right;
                }
                else
                {
                    directionInt = Mathf.Abs(direction.y) > Mathf.Abs(direction.x)
                        ? IndexDirection.Up
                        : IndexDirection.Left;
                }
            }
            else
            {
                if (direction.x > 0)
                {
                    directionInt = Mathf.Abs(direction.y) > Mathf.Abs(direction.x)
                        ? IndexDirection.Down
                        : IndexDirection.Right;
                }
                else
                {
                    directionInt = Mathf.Abs(direction.y) > Mathf.Abs(direction.x)
                        ? IndexDirection.Down
                        : IndexDirection.Left;
                }
            }

            return directionInt;
        }
    }

    public enum IndexDirection
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }
}