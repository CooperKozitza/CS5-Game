using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    LevelManager levelManager;
    Room FindRoom(int floor)
    {
        Room room = new Room();
        GameObject[,] grid = levelManager.Mansion[floor];

        int randX = Random.Range(0, levelManager.levelX);
        int randY = Random.Range(0, levelManager.levelY);
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
