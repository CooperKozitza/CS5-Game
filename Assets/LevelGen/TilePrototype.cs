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
    public List<Room> rooms = new();
    public NeighborsList neighborsList;

    public GameObject floor;

    public void ChangeFloorMaterial(Material material)
    {
        if (floor != null)
        {
            floor.GetComponent<MeshRenderer>().material = material;
        }
    }
}


