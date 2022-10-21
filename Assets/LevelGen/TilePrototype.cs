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
            public List<GameObject> Neighbors = new List<GameObject>();
        }

        void Awake()
        {
            for (int i = 0; i < Left.Neighbors.Count; i++)
            {
            }
        }

        public Face Left = new Face();
        public Face Front = new Face();
        public Face Right = new Face(); 
        public Face Back = new Face();
    }
}

