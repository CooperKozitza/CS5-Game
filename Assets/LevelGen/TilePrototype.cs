using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using System;

namespace Tile
{
    public class TilePrototype : MonoBehaviour
    {
        [Range(0, 1)]
        public float probability = 1.0f;
        public List<TilePrototype> Entropy = new List<TilePrototype>();
        public bool collapsed = false;

        [Serializable]
        public class Face
        {
            public string connectorId;
            [Range(0, 3), Tooltip("Clockwise")]
            public int rotation = 0;
            public bool invariable = false;
        }

        [Serializable]
        public class VerticalFace : Face
        {
            public bool symetrical;
        }

        public bool walkable = false;

        public VerticalFace Top;
        public VerticalFace Bottom;
        public Face Left;
        public Face Right;
        public Face Front;
        public Face Back;

    }
}

