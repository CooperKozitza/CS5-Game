using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour
{

    public List<NavMeshSurface> surfaces;

    [InspectorButton("BuildNavMesh")]
    public bool buildNavmesh = false;
    void BuildNavMesh()
    {
        for (int i = 0; i < surfaces.Count; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }

}
