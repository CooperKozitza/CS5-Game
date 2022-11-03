using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using System;

public class TilePrototype : MonoBehaviour
{
    [Range(0, 100)]
    public int probability = 0;
    public bool invariable = false;
    [Range(0, 3)]
    public int rotation = 0;
    public NeighborsList neighborsList;
}


