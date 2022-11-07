using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using System;

public class TilePrototype : MonoBehaviour
{
    [Range(0, 100)]
    public int probability = 50;
    public bool invariable = false;
    [Range(0, 3)]
    public int rotation = 0;
    public Room room;
    public NeighborsList neighborsList;
}


