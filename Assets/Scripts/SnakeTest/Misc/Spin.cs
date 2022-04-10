using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeTest.Misc
{
    public class Spin : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * 100);
        }
    }
}