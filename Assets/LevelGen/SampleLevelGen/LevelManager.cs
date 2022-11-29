using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject levelGenerator;
    public GameObject roomManager;
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
        if (levelGenerator != null)
        {
            LevelGenerator levelGen = levelGenerator.GetComponent<LevelGenerator>();
            levelGen.Generate();
        }
        if (roomManager != null)
        {
            roomManager.GetComponent<RoomManager>().SetRooms();
        }
    }
}
