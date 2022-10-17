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
        [Serializable]
        public class Face
        {
            [Serializable]
            public struct Pair
            {
                public GameObject gameObject;
                [Range(0, 3)]
                public int rotation;
            }
            public List<Pair> Neighbors = new List<Pair>();
        }


        public Face Left = new Face();
        public Face Front = new Face();
        public Face Right = new Face(); 
        public Face Back = new Face();
    }
}

