using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private LevelManager levelManager;
    public GameObject levelManagerObj;

    void Awake()
    {
        levelManager = levelManagerObj.GetComponent<LevelManager>();
    }

    List<Room> rooms = new List<Room>();

    [InspectorButton("SetRooms")]
    public bool setRooms = false;
    void SetRooms()
    {
        while (true)
        {
            Room room = FindRoomOnFloor(0);
            if (room == null) break;
            else
            {
                foreach (TilePrototype tile in room.tiles) tile.room = room;
                rooms.Add(room);
            }
        } 

        foreach (Room room in rooms)
        {
            Debug.Log(room.tileCoords);
        }
    }

    /// <summary>
    /// Creates a new room from a given floor plan. The room produced will be unique, and if no new rooms can be found, a null room will be returned
    /// </summary>
    /// <param name="floor">The floor in which to search for a room</param>
    /// <returns></returns>
    Room FindRoomOnFloor(int floor)
    {
        Room room = new Room();
        GameObject[,] grid = levelManager.Mansion[floor];
        TilePrototype[,] protoGrid = new TilePrototype[levelManager.levelX, levelManager.levelY];
        for (int y = 0; y < levelManager.levelY; y++)
        {
            for (int x = 0; x < levelManager.levelX; x++)
            {
                protoGrid[x, y] = grid[x, y].GetComponent<TilePrototype>();
            }
        }

        List<Vector2Int> nextGeneration = new List<Vector2Int>();
        for (int y = 0; y < levelManager.levelY; y++)
        {
            for (int x = 0; x < levelManager.levelX; x++)
            {
                if (protoGrid[x, y].room == null)
                {
                    break;
                }
                if (x == levelManager.levelX - 1 && y == levelManager.levelY) return null;
            }
        }

        while (true)
        {
            int randX = Random.Range(0, levelManager.levelX);
            int randY = Random.Range(0, levelManager.levelY);

            if (protoGrid[randX, randY].room == null)
            {
                Debug.Log("Found Roomless Tile, Starting...");
                nextGeneration.Add(new Vector2Int(randX, randY));
                break;
            }
        }

        void spread(Vector2Int tile, int xOffset, int yOffset)
        {
            if (tile.x + xOffset < levelManager.levelX && tile.x + xOffset > -1 && tile.y + yOffset < levelManager.levelY && tile.y + yOffset > -1)
            {
                if (room.tiles.Contains(protoGrid[tile.x + xOffset, tile.y + yOffset])) return;
                if (protoGrid[tile.x + xOffset, tile.y + yOffset].neighborsList.Right.permiable && !room.tiles.Contains(protoGrid[tile.x + xOffset, tile.y + yOffset]))
                {
                    Debug.Log("Spread");
                    room.tiles.Add(protoGrid[tile.x + xOffset, tile.y + yOffset]);
                    room.tileCoords.Add(new Vector2Int(tile.x + xOffset, tile.y + yOffset));
                    nextGeneration.Add(new Vector2Int(tile.x + xOffset, tile.y + yOffset));
                }
            }
        }

        while (true)
        {
            for (int i = nextGeneration.Count - 1; i > -1; i--)
            {
                if (protoGrid[nextGeneration[i].x, nextGeneration[i].y].neighborsList.Left.permiable)
                {
                    spread(nextGeneration[i], -1, 0);
                }
                if (protoGrid[nextGeneration[i].x, nextGeneration[i].y].neighborsList.Front.permiable)
                {
                    spread(nextGeneration[i], 0, 1);
                }
                if (protoGrid[nextGeneration[i].x, nextGeneration[i].y].neighborsList.Right.permiable)
                {
                    spread(nextGeneration[i], 1, 0);
                }
                if (protoGrid[nextGeneration[i].x, nextGeneration[i].y].neighborsList.Back.permiable)
                {
                    spread(nextGeneration[i], 0, -1);
                }
                nextGeneration.RemoveAt(i);
                Debug.Log("Spread Complete for generation, next gen count = " + nextGeneration.Count.ToString() + ", room tile count = " + room.tiles.Count.ToString());
                if (nextGeneration.Count == 0) break;
            }
            if (nextGeneration.Count == 0) break;
        }

        return room;
    }
}

public class Room
{
    public List<TilePrototype> tiles = new List<TilePrototype>();
    public List<Vector2Int> tileCoords = new List<Vector2Int>();
    public int adjacentRooms = 0;

    public enum Type { Hallway = 5, Bedroom = 1, Library = 2 }
    public Type roomType { get; set; }
}
