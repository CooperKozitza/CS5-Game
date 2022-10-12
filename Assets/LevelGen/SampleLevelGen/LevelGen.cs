using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tile;

public class LevelGen : MonoBehaviour
{
    public int levelX, levelY;

    // removable
    private List<Object> Existing = new List<Object>();
    public List<GameObject> Tileset = new List<GameObject>();
    private List<string> connectorSet { get; set; }

    private string[,] entropyMap { get; set; }
    private GameObject[,] level { get; set; }

    void Start()
    {
        entropyMap = new string[levelX, levelY];
        level = new GameObject[levelX, levelY];
        
        for (int i = 0; i < Tileset.Count; i++)
        {
            Tileset[i].SetActive(true);
        }

        foreach (GameObject tile in Tileset)
        {
            connectorSet.Add(tile.GetComponent<TilePrototype>().horizontalConnectors);
        }
    }

    [InspectorButton("Generate")]
    public bool generate = false;

    public void Generate()
    {
        if (!UnityEngine.Application.isPlaying) Start();
        // removaable
        // Remove existing tiles in scene previosly generated
        if (this.Existing.Count > 0)
        {
            foreach (Object tile in this.Existing)
            {
                DestroyImmediate(tile);
            }
        }

        // Start by setting a random tile to a random mesh
        int randX = Random.Range(0, levelX);
        int randY = Random.Range(0, levelY);

        GameObject seedTile = Instantiate(Tileset[Random.Range(0, Tileset.Count)], new Vector3(randX * 2, 1, randY * 2), new Quaternion());
        seedTile.SetActive(true);
        seedTile.gameObject.GetComponent<TilePrototype>().collapsed = true;
        level[randX, randY] = seedTile;

        Distribute(randX, randY);

        // generate the rest of the level
        bool collapsed = false;
        int x = randX, y = randY;

        while (!collapsed)
        {
            int deltaX = Random.Range(-1, 2);
            int deltaY = deltaX == 0 ? Random.Range(-1 , 2) : 0;

            // pick a random tile from four adjacent
            x += deltaX;
            y += deltaY;

            // find valid tile
            foreach (string horizontalConnectors in connectorSet)
            {
                // left tile
                if (deltaX == -1 && deltaY == 0)
                {
                    if (entropyMap[x, y][0] != '-')
                    {
                        if (horizontalConnectors.Contains(entropyMap[x, y][0]))
                        {
                            // valid tile
                        }
                        else continue;
                    }
                }
            }
        }
        
        foreach (GameObject tile in level)
        {
            Existing.Add(tile);
        }
    }

    private enum entropyOffsets { Left = 0, Right = 1, Front = 2, Back = 3 }
    public void Distribute(int x, int y)
    {
        if (entropyMap[x, y] == "----") return;
        if (x > levelX || y > levelY) return;

        // get connectors of collapsed tile
        string collapsedTileConnectors = 
            level[x, y] != null ? level[x, y].GetComponent<TilePrototype>().horizontalConnectors
            : entropyMap[x, y];
        entropyMap[x, y] = collapsedTileConnectors;

        //distribute to four adj. tiles:
        // set the right side connector of the left tile to the left side connector of the original tile
        if (entropyMap[x - 1, y] != "")
        {
            entropyMap[x - 1, y].Remove((int)entropyOffsets.Right);
            entropyMap[x - 1, y].Insert((int)entropyOffsets.Right, collapsedTileConnectors[(int)entropyOffsets.Left].ToString());
        }
        else 
        {
            entropyMap[x - 1, y] = string.Concat("-", collapsedTileConnectors[(int)entropyOffsets.Left].ToString(), "--");
        }
        Distribute(x - 1, y);

        // set the left side connector of the right tile to the right side connector of the original tile
        if (entropyMap[x + 1, y] != "")
        {
            entropyMap[x + 1, y].Remove((int)entropyOffsets.Left);
            entropyMap[x + 1, y].Insert((int)entropyOffsets.Left, collapsedTileConnectors[(int)entropyOffsets.Right].ToString());
        }
        else 
        {
            entropyMap[x + 1, y] = string.Concat(collapsedTileConnectors[(int)entropyOffsets.Right].ToString(), "---");
        }
        Distribute(x + 1, y);

        // set the back side connector of the front tile to the front side connector of the original tile
        if (entropyMap[x, y + 1] != "")
        {
            entropyMap[x, y + 1].Remove((int)entropyOffsets.Back);
            entropyMap[x, y + 1].Insert((int)entropyOffsets.Back, collapsedTileConnectors[(int)entropyOffsets.Front].ToString());
        }
        else 
        {
            entropyMap[x, y + 1] = string.Concat("---", collapsedTileConnectors[(int)entropyOffsets.Front].ToString());
        }
        Distribute(x, y + 1);

        // set the front side connector of the back tile to the back side connector of the original tile
        if (entropyMap[x, y - 1] != "")
        {
            entropyMap[x, y - 1].Remove((int)entropyOffsets.Front);
            entropyMap[x, y - 1].Insert((int)entropyOffsets.Front, collapsedTileConnectors[(int)entropyOffsets.Back].ToString());
        }
        else 
        {
            entropyMap[x, y - 1] = string.Concat("---", collapsedTileConnectors[(int)entropyOffsets.Back].ToString());
        }
        Distribute(x, y - 1);

        Debug.Log("(" + x + ", " + y + "): " + entropyMap[x, y]);
    }
}