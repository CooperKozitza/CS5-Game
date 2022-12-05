using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public void SetRooms(int floor)
    {
        while (true)
        {
            Room room = FindRoomOnFloor(floor);
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
                levelManager.Mansion[floor][coord.x, coord.y].name = rooms.IndexOf(room).ToString() + ": " + levelManager.Mansion[floor][coord.x, coord.y].name;
            }

            foreach (TilePrototype tile in room.sharedTiles)
            {
                foreach (Room tileRoom in tile.rooms)
                {
                    if (!room.adjacentRooms.Contains(tileRoom) && tileRoom != room) room.adjacentRooms.Add(tileRoom);
                }
            }

            if (room.adjacentRooms.Count >= (int)Room.Type.Hallway) 
            {
                room.roomType = Room.Type.Hallway;
                Debug.Log("RM: " + rooms.IndexOf(room).ToString() + ": is a hallway");
            }
        }

        List<Room> hallways = rooms.FindAll(x => x.roomType == Room.Type.Hallway);
        foreach (Room room in hallways)
        {
            AddDoorToRoom(room, floor);
        }

        List<Room> doorlessRooms = rooms.FindAll(x => x.doorCount < 1);
        foreach (Room room in doorlessRooms) AddDoorToRoom(room, floor);
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
                    Debug.Log("RM: Found Roomless Tile, Starting...");
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
                        room.tiles.Add(target);
                        room.tileCoords.Add(new Vector2Int(tile.x + xOffset, tile.y + yOffset));
                        nextGeneration.Add(new Vector2Int(tile.x + xOffset, tile.y + yOffset));
                        if (preDoors.Contains(target.neighborsList.Self) && !room.sharedTiles.Contains(target))
                        {
                            room.sharedTiles.Add(target);
                            room.sharedTileCoords.Add(new Vector2Int(tile.x + xOffset, tile.y + yOffset));
                        }
                    }
                    else
                    {
                        if (preDoors.Contains(target.neighborsList.Self) && !room.sharedTiles.Contains(target))
                        {
                            room.sharedTiles.Add(target);
                            room.sharedTileCoords.Add(new Vector2Int(tile.x + xOffset, tile.y + yOffset));
                        }
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
            Debug.Log("RM: Spread Complete for generation, next gen count = " + nextGeneration.Count.ToString() + ", room tile count = " + room.tiles.Count.ToString());
            if (nextGeneration.Count == 0) break;
        }
        return room;
    }

    void AddDoorToRoom(Room room, int floor)
    {
        switch (room.roomType)
        {
            case Room.Type.Hallway:
            {
                foreach (Room adjacentRoom in room.adjacentRooms)
                {
                    List<TilePrototype> possibleTiles = adjacentRoom.sharedTiles.FindAll(x => x.rooms.Contains(room) && preDoors.Contains(x.neighborsList.Self));
                    List<Vector2Int> possibleTileCoords = new();
                    foreach (TilePrototype possibleTile in possibleTiles) possibleTileCoords.Add(adjacentRoom.sharedTileCoords[adjacentRoom.sharedTiles.IndexOf(possibleTile)]);
                    if (possibleTiles.Count > 0 && (adjacentRoom.roomType == Room.Type.Hallway || adjacentRoom.doorCount < 2))
                    {
                        Vector2Int target = possibleTileCoords[possibleTileCoords.Count / 2];
                        ConvertToDoor(target, floor);

                        adjacentRoom.doorCount++;
                        room.doorCount++;
                    }
                }
                break;
            }
            default:
            {
                Vector2Int target = room.sharedTileCoords[room.doorCount < 1 ? 0 : room.sharedTileCoords.Count];
                if (target.x > 0 && target.x < levelManager.levelX + 1 && target.y > 0 && target.y < levelManager.levelY + 1) ConvertToDoor(target, floor);
                room.doorCount++;
                break;
            }
        }
    }

    void ConvertToDoor(Vector2Int pos, int floor)
    {
        if (pos.x < 1 || pos.x > levelManager.levelX - 2 || pos.y < 1 || pos.y > levelManager.levelY - 2) return;

        TilePrototype preDoor = levelManager.Mansion[floor][pos.x, pos.y].GetComponent<TilePrototype>();
        try
        {
            GameObject postDoor = postDoors[preDoors.IndexOf(preDoor.neighborsList.Self)];

            Destroy(levelManager.Mansion[floor][pos.x, pos.y]);
        
            Quaternion quaternion = new Quaternion();
            quaternion.eulerAngles = new Vector3(0, postDoor.GetComponent<TilePrototype>().rotation * 90, 0);
            levelManager.Mansion[floor][pos.x, pos.y] = Instantiate(postDoor, new Vector3(pos.x * 2, floor * 2.8f, (levelManager.levelY - pos.y) * 2), quaternion);
            levelManager.Mansion[floor][pos.x, pos.y].name = "(" + pos.x.ToString() + ", " + pos.y.ToString() + ")";
        }
        catch (Exception e)
        {
            Debug.Log("RM: Failed to find postDoor from preDoor. Exception: " + e.Message);
        }
    }
}

public class Room
{
    public List<TilePrototype> tiles = new();
    public List<TilePrototype> sharedTiles = new();

    public List<Vector2Int> tileCoords = new();
    public List<Vector2Int> sharedTileCoords = new();

    public List<Room> adjacentRooms = new();

    public int doorCount = 0;

    public enum Type { Hallway = 6, Bedroom = 1, Library = 2 }
    public Type roomType { get; set; }
}
