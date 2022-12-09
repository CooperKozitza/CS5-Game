using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject levelGeneratorObj;
    public GameObject roomManagerObj;

    private LevelGenerator levelGenerator;
    private RoomManager roomManager;

    public int levelX, levelY;
    [Range(0,5)]
    public int floors;

    public List<GameObject[,]> Mansion { get; set; }

    private Room seed;

    void Start()
    {
        Mansion = new List<GameObject[,]>();
        levelGenerator = levelGeneratorObj.GetComponent<LevelGenerator>();
        roomManager = roomManagerObj.GetComponent<RoomManager>();
    }

    void CreateNewLevel(int floor)
    {
        if (levelGenerator != null)
        {
            levelGenerator.Generate(floor, seed != null ? seed : null);
        }
        if (roomManager != null)
        {
            roomManager.SetRooms(floor);
            seed = roomManager.StairRoom;
        }
    }

    [InspectorButton("CreateMansion")]
    public bool createMansion = false;
    void CreateMansion()
    {
        for (int i = 0; i < floors; i++)
        {
            CreateNewLevel(i);
        }
    }
}
