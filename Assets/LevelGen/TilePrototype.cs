using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using System;

public class TilePrototype : MonoBehaviour
{

    [Serializable]
    public class Side
    {
        public int connectorId;
        [Range(0, 3)]
        public int rotation = 0;
        public bool flipped = false;
    }

    public bool walkable = false;

    public Side Top;
    public Side Bottom;
    public Side Left;
    public Side Right;
    public Side Front;
    public Side Back;

}
