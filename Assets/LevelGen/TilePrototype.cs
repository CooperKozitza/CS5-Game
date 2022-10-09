using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using System;

public class TilePrototype : MonoBehaviour
{
    public List<TilePrototype> Entropy = new List<TilePrototype>();
    public bool Collapsed = false;

    [Serializable]
    public class Face
    {
        public int connectorId;
        [Range(0, 3)]
        public int rotation = 0;
        public bool flipped = false;
        public bool invariable = false;
        public Dictionary<TilePrototype, int> ProbabilityOverrides = new Dictionary<TilePrototype, int>();
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
