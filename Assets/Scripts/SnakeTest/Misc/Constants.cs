using System.Collections.Generic;
using SnakeTest.Player;
using UnityEngine;

namespace SnakeTest.Misc
{
    public class Constants
    {
        public static Dictionary<string, int> turnMap = new()
        {
            { "Right,Up", 1 }, // this one is tricky to look at, you have to think of the direction you are going
            { "Up,Right", 3 }, // and coming and also the frame of the turn which visually are different
            { "Left,Down", 3 }, // so if you are going left and turn down, the index of the prefab is right down
            { "Down,Left", 1 },
            { "Down,Right", 2 },
            { "Right,Down", 0 },
            { "Up,Left", 0 },
            { "Left,Up", 2 },
        };
        public static Dictionary<IndexDirection, Vector3> vectorMap = new()
        {
            { IndexDirection.Up, Vector3.up },
            { IndexDirection.Down, Vector3.down },
            { IndexDirection.Left, Vector3.left },
            { IndexDirection.Right, Vector3.right },
        };
    }
}