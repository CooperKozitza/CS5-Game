using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using Unity.AI.Navigation;
using System;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface NavMeshSurface;

    [InspectorButton("Bake")]
    public bool bake = false;
    void Bake()
    {
        NavMeshSurface.BuildNavMesh();
    }
}
