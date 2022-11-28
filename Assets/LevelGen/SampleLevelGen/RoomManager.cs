using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private LevelManager levelManager;
    public GameObject levelManagerObj;

    public GameObject seedTile;
    public int minTwoDoorSize = 20;
    public List<GameObject> preDoors = new List<GameObject>();
    public List<GameObject> postDoors = new List<GameObject>();

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
            if (room.tiles.Count == 0) break;
            else
            {
                foreach (TilePrototype tile in room.tiles) tile.rooms.Add(room);
                foreach (TilePrototype tile in room.sharedTiles) tile.rooms.Add(room);
                rooms.Add(room);
            }
        } 

        foreach (Room room in rooms)
        {
            foreach (Vector2Int coord in room.tileCoords)
            {
                levelManager.Mansion[0][coord.x, coord.y].name = rooms.IndexOf(room).ToString() + ": " + levelManager.Mansion[0][coord.x, coord.y].name;
            }

            foreach (TilePrototype tile in room.sharedTiles)
            {
                foreach (Room tileRoom in tile.rooms)
                {
                    if (!room.adjacentRooms.Contains(tileRoom)) room.adjacentRooms.Add(tileRoom);
                }
            }

            if (room.adjacentRooms.Count >= (int)Room.Type.Hallway) room.roomType = Room.Type.Hallway; 
            //AddDoorToRoom(room);
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
                if (protoGrid[x, y].rooms.Count == 0)
                {
                    break;
                }
                if (x == levelManager.levelX - 1 && y == levelManager.levelY) return null;
            }
        }

        bool foundTile = false;
        for (int y = 0; y < levelManager.levelY; y++)
        {
            for (int x = 0; x < levelManager.levelX; x++)
            {
                if (protoGrid[x, y].rooms.Count == 0 && protoGrid[x, y].GetComponent<TilePrototype>().neighborsList.Self == seedTile)
                {
                    Debug.Log("Found Roomless Tile, Starting...");
                    nextGeneration.Add(new Vector2Int(x, y));
                    foundTile = true;
                }
                if (foundTile) break;
            }
            if (foundTile) break;
            if (y == levelManager.levelY - 1) return null;
        }

        void spread(Vector2Int tile, int xOffset, int yOffset)
        {
            if (tile.x + xOffset < levelManager.levelX && tile.x + xOffset > -1 && tile.y + yOffset < levelManager.levelY && tile.y + yOffset > -1)
            {
                TilePrototype target = protoGrid[tile.x + xOffset, tile.y + yOffset];

                if (room.tiles.Contains(target)) return;
                NeighborsList.Face face = xOffset == 1 ? target.neighborsList.Left :
                    xOffset == -1 ? target.neighborsList.Right :
                    yOffset == 1 ? target.neighborsList.Back :
                    yOffset == -1 ? target.neighborsList.Front : null;

                if (!room.tiles.Contains(target) || !room.sharedTiles.Contains(target))
                {
                    if (face.permeable)
                    {
                        Debug.Log("Spread");
                        room.tiles.Add(target);
                        room.tileCoords.Add(new Vector2Int(tile.x + xOffset, tile.y + yOffset));
                        nextGeneration.Add(new Vector2Int(tile.x + xOffset, tile.y + yOffset));
                    }
                    else
                    {
                        room.sharedTiles.Add(target);
                        room.sharedTileCoords.Add(new Vector2Int(tile.x + xOffset, tile.y + yOffset));
                    }
                }
            }
        }

        while (true)
        {
            for (int i = nextGeneration.Count - 1; i > -1; i--)
            {
                if (protoGrid[nextGeneration[i].x, nextGeneration[i].y].neighborsList.Left.permeable) spread(nextGeneration[i], -1, 0);
                if (protoGrid[nextGeneration[i].x, nextGeneration[i].y].neighborsList.Front.permeable) spread(nextGeneration[i], 0, 1);
                if (protoGrid[nextGeneration[i].x, nextGeneration[i].y].neighborsList.Right.permeable) spread(nextGeneration[i], 1, 0);
                if (protoGrid[nextGeneration[i].x, nextGeneration[i].y].neighborsList.Back.permeable) spread(nextGeneration[i], 0, -1);
                nextGeneration.RemoveAt(i);
            }
            Debug.Log("Spread Complete for generation, next gen count = " + nextGeneration.Count.ToString() + ", room tile count = " + room.tiles.Count.ToString());
            if (nextGeneration.Count == 0) break;
        }
        return room;
    }

    void AddDoorToRoom(Room room)
    {
        switch (room.roomType)
        {
            case Room.Type.Hallway:
            {
                foreach (Room adjacentRoom in room.adjacentRooms)
                {
                    List<TilePrototype> possibleTiles = adjacentRoom.tiles.FindAll(x => x.rooms.Contains(room));
                    possibleTiles[Random.Range(0, possibleTiles.Count)]
                }
                break;
            }
            default:
            {

                break;
            }
        }
    }

    void GenerateDoorsOnFloor(int floor)
    {

    }
}

public class Room
{
    public List<TilePrototype> tiles = new();
    public List<TilePrototype> sharedTiles = new();

    public List<Vector2Int> tileCoords = new();
    public List<Vector2Int> sharedTileCoords = new();

    public List<Room> adjacentRooms = new();

    public enum Type { Hallway = 5, Bedroom = 1, Library = 2 }
    public Type roomType { get; set; }
}
