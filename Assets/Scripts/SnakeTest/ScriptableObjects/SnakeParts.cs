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

        public GameObject GetPart(BodyType bodyType, int direction, int directionOut, Vector3 position)
        {
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
                    int turnDirection = turnMap[$"{directionOut},{direction}"];
                    return Clone(TurnPrefabs[turnDirection], position);
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