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

<<<<<<< HEAD
    private Material material;

    void OnGizmosDraw()
    {
        Gizmos.color = new Color(rooms.FindAll(x => x.roomType == Room.Type.Hallway).Count > 0 ? 1 : 0, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(2, 0.5f, 2));
    }
}


