using System;
using System.Collections.Generic;
using System.Linq;
using SnakeTest.Misc;
using SnakeTest.Player;
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
            Turn,
            TurnFilled
        }

        public GameObject[] HeadPrefabs;
        public GameObject[] HeadRamPrefabs;
        public GameObject[] BodyPrefabs;
        public GameObject[] BodyFilledPrefabs;
        public GameObject[] TailPrefabs;
        public GameObject[] TurnPrefabs;
        public GameObject[] TurnFilledPrefabs;


        private static readonly int Color1 = Shader.PropertyToID("_Color");

        public Vector3 ModPosition(Vector3 position)
        {
            var snap = new Vector3(14, 8);
            position = (position + snap * 3);
            position = new Vector3(position.x % (snap.x * 2), position.y % (snap.y * 2)) - snap;
            return position;
        }

        public GameObject GetPart(BodyType bodyType, IndexDirection direction, IndexDirection directionOut,
            Vector3 position, int c = 0)
        {
            Color color = Color.green;
            switch (c)
            {
                case 1:
                    color = new Color(1f, 0.5f, 0);
                    break;
                case 2:
                    color = Color.yellow;
                    break;
            }

            string key = $"{directionOut},{direction}";
            position = ModPosition(position);
            switch (bodyType)
            {
                case BodyType.Head:
                    return Clone(HeadPrefabs[(int)direction], position, color);
                case BodyType.HeadRam:
                    return Clone(HeadRamPrefabs[(int)direction], position, color);
                case BodyType.Body:
                    return Clone(BodyPrefabs[(int)direction % 2], position, color);
                case BodyType.BodyFilled:
                    return Clone(BodyFilledPrefabs[(int)direction % 2], position, color);
                case BodyType.TurnFilled:
                    if (Constants.turnMap.ContainsKey(key))
                    {
                        int turnIndex = Constants.turnMap[key];
                        return Clone(TurnFilledPrefabs[turnIndex], position, color);
                    }

                    throw new Exception($"Turn direction not found: {key}");
                case BodyType.Tail:
                    return Clone(TailPrefabs[(int)direction], position, color);
                case BodyType.Turn:
                    if (Constants.turnMap.ContainsKey(key))
                    {
                        int turnIndex = Constants.turnMap[key];
                        return Clone(TurnPrefabs[turnIndex], position, color);
                    }

                    throw new Exception($"Turn direction not found: {key}");
                default:
                    return null;
            }
        }

        private GameObject Clone(GameObject prefab, Vector3 position, Color color)
        {
            var instance = Instantiate(prefab, position, Quaternion.identity);
            instance.GetComponent<MeshRenderer>().material.SetColor(Color1, color);
            return instance;
        }
    }
}