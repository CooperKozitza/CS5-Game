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
            public List<Tuple<GameObject, int>> Neighbors = new List<Tuple<GameObject, int>>();
        }
        
        public Face Left { get; set; }
        public Face Front { get; set; }
        public Face Right { get; set; } 
        public Face Back { get; set; }

    }
}

