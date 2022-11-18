using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject generator;
    public int levelX, levelY;
    public List<GameObject[,]> Mansion { get; set; }

    void Start()
    {
        Mansion = new List<GameObject[,]>();
    }

    [InspectorButton("CreateNewLevel")]
    public bool createNewLevel = false;
    void CreateNewLevel()
    {
        if (generator != null)
        {
            LevelGenerator levelGen = generator.GetComponent<LevelGenerator>();
            levelGen.Generate();

            //Update NavMesh
            
        }
    }
}
