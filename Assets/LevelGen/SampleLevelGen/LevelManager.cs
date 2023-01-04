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

    public bool[] GenerationComplete { get; set; }

    private Room seed { get; set; }

    void Start()
    {
        Mansion = new List<GameObject[,]>();
        levelGenerator = levelGeneratorObj.GetComponent<LevelGenerator>();
        roomManager = roomManagerObj.GetComponent<RoomManager>();
    }

    [InspectorButton("CreateMansion")]
    public bool createMansion = false;
    void CreateMansion()
    {
        for (int i = 0; i < floors; i++)
        {
            Mansion.Add(new GameObject[levelX, levelY]);
        }
        GenerationComplete = new bool[floors];
        StartCoroutine(GenerateFloors());
    }

    public IEnumerator GenerateFloors()
    {
        GenerationComplete[0] = false;
        if (levelGenerator != null)
        {
            StartCoroutine(levelGenerator.Generate(0, seed != null ? seed : null));
        }

        while (!GenerationComplete[0])
        {
            yield return null;
        }

        if (roomManager != null)
        {
            StartCoroutine(roomManager.SetRooms(0));
            if (seed != null) seed = roomManager.StairRoom;
        }

        for (int i = 1; i < floors; i++)
        {
            StartCoroutine(GenerateFloor(i));
        }
    }

    private IEnumerator GenerateFloor(int floor = 0)
    {
        GenerationComplete[floor] = false;
        if (levelGenerator != null)
        {
            StartCoroutine(levelGenerator.Generate(floor, seed != null ? seed : null));
        }

        while (!GenerationComplete[floor])
        {
            yield return null;
        }

        if (roomManager != null)
        {
            StartCoroutine(roomManager.SetRooms(floor));
            if (seed != null) seed = roomManager.StairRoom;
        }
    }

    [InspectorButton("CleanUp")]
    public bool cleanUp = false;
    public void CleanUp()
    {
        for (int floor = 0; floor < floors; floor++)
        {
            List<Room> roomsOnFloor = new();
            foreach (GameObject tile in Mansion[floor])
            {
                List<Room> tileRooms = tile.GetComponent<TilePrototype>().rooms;
                foreach (Room room in tileRooms)
                {
                    if (!roomsOnFloor.Contains(room)) roomsOnFloor.Add(room);
                }
            }

            List<GameObject> roomParents = new();
            int roomIndex = 0;
            foreach (Room room in roomsOnFloor)
            {
                GameObject roomParent = new GameObject(roomIndex.ToString() + ": " + room.roomType.ToString());
                roomIndex++;
                roomParents.Add(roomParent);
                foreach (TilePrototype tile in room.tiles)
                {
                    tile.gameObject.transform.SetParent(roomParent.transform);
                }
            }

            GameObject floorParent = new GameObject("Floor: " + floor.ToString());
            foreach (GameObject roomParent in roomParents)
            {
                roomParent.transform.SetParent(floorParent.transform);
            }
        }
    }
}
