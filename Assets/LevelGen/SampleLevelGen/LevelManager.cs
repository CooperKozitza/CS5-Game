using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject levelGenerator;
    public GameObject roomManager;
    public int levelX, levelY;
    [Range(0,5)]
    public int floors;
    public List<GameObject[,]> Mansion { get; set; }

    public int ghostCount;
    public GameObject ghostPrefab;
    public float tileSize = 2;

    void Start()
    {
        Mansion = new List<GameObject[,]>();
    }

    void CreateNewLevel(int floor)
    {
        if (levelGenerator != null)
        {
            LevelGenerator levelGen = levelGenerator.GetComponent<LevelGenerator>();
            levelGen.Generate(floor);
        }
        if (roomManager != null)
        {
            roomManager.GetComponent<RoomManager>().SetRooms(floor);
        }
    }

    [InspectorButton("CreateMansion")]
    public bool createMansion = false;
    void CreateMansion()
    {
        for (int i = 0; i < floors; i++)
        {
            CreateNewLevel(i);
            SpawnGhosts(ghostCount);
        }
    }

    void SpawnGhosts(int ghostCount) {
        for (int i = 0; i < ghostCount; i++)
        {
            Instantiate(ghostPrefab, new Vector3(Random.Range(0, levelX) * tileSize, 1, Random.Range(0, levelY) * tileSize), Quaternion.identity);
        }
    }

}
