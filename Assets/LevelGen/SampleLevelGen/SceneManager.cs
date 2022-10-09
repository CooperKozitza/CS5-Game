using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class SceneManager : MonoBehaviour
{
    public GameObject prefab;
    public int gridX = 0;
    public int gridY = 0;

    void CreateGrid()
    {
        #if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            for (int i = 0; i < this.gridY; i++)
            {
                for (int j = 0; j < this.gridX; j++)
                {
                    Instantiate<GameObject>(this.prefab, new Vector3(j * 2, 0, i * 2), new Quaternion(0, 0, 0, 0));
                }
            }
        }
        else
        {
            return;
        }
        #endif
    }


    [InspectorButton("CreateGrid")]
    public bool createGrid = false;
}
