using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NeighborsList", menuName = "Tileset/New Neighbors List", order = 1)]
public class NeighborsList : ScriptableObject
{
    public GameObject Self;

    [Serializable]
    public class Face
    {
        public bool permeable = false;
        public List<GameObject> neighbors = new List<GameObject>();
    }

    public Face Left = new Face();
    public Face Front = new Face();
    public Face Right = new Face();
    public Face Back = new Face();
}
