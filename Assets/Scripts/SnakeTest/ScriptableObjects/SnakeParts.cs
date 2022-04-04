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
            Tail
        }

        public GameObject[] HeadPrefabs;
        public GameObject[] HeadRamPrefabs;
        public GameObject[] BodyPrefabs;
        public GameObject[] BodyFilledPrefabs;
        public GameObject[] TailPrefabs;

        public GameObject GetPart(BodyType bodyType, int direction, Vector3 position)
        {
            switch (bodyType)
            {
                case BodyType.Head:
                    return Clone(HeadPrefabs[direction], position);
                case BodyType.HeadRam:
                    return Clone(HeadRamPrefabs[direction], position);
                case BodyType.Body:
                    return Clone(BodyPrefabs[direction], position);
                case BodyType.BodyFilled:
                    return Clone(BodyFilledPrefabs[direction], position);
                case BodyType.Tail:
                    return Clone(TailPrefabs[direction], position);
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