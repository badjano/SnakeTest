using System;
using System.Collections.Generic;
using UnityEngine;


namespace SnakeTest.Objects
{
    [CreateAssetMenu(fileName = "SnakeParts", menuName = "ScriptableObjects/SnakeParts", order = 1)]
    public class SnakeParts : ScriptableObject
    {
        public enum BodyType
        {
            Head,
            HeadRam,
            Body,
            BodyFilled,
            Tail,
            Turn
        }

        public GameObject[] HeadPrefabs;
        public GameObject[] HeadRamPrefabs;
        public GameObject[] BodyPrefabs;
        public GameObject[] BodyFilledPrefabs;
        public GameObject[] TailPrefabs;
        public GameObject[] TurnPrefabs;

        private Dictionary<string, int> turnMap = new Dictionary<string, int>
        {
            {"0,1", 3},
            {"0,3", 0},
            {"1,0", 1},
            {"1,2", 0},
            {"2,1", 2},
            {"2,3", 1},
            {"3,0", 2},
            {"3,2", 3}
        };

        public Vector3 ModPosition(Vector3 position)
        {
            var snap = new Vector3(14, 8);
            position = (position + snap * 3);
            position = new Vector3(position.x % (snap.x * 2), position.y % (snap.y * 2)) - snap;
            return position;
        }

        public GameObject GetPart(BodyType bodyType, int direction, int directionOut, Vector3 position)
        {
            position = ModPosition(position);
            switch (bodyType)
            {
                case BodyType.Head:
                    return Clone(HeadPrefabs[direction], position);
                case BodyType.HeadRam:
                    return Clone(HeadRamPrefabs[direction], position);
                case BodyType.Body:
                    return Clone(BodyPrefabs[direction % 2], position);
                case BodyType.BodyFilled:
                    return Clone(BodyFilledPrefabs[direction % 2], position);
                case BodyType.Tail:
                    return Clone(TailPrefabs[direction], position);
                case BodyType.Turn:
                    var key = $"{directionOut},{direction}";
                    if (turnMap.ContainsKey(key))
                    {
                        int turnDirection = turnMap[key];
                        return Clone(TurnPrefabs[turnDirection], position);
                    }
                    else
                    {
                        throw new Exception($"Turn direction not found: {key}");
                    }
                default:
                    return null;
            }
        }

        private GameObject Clone(GameObject prefab, Vector3 position)
        {
            return Instantiate(prefab, position, Quaternion.identity);
        }
    }
}