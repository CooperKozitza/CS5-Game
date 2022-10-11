using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class LevelGen : MonoBehaviour
{
    public int levelX, levelY;
    public Dictionary<string, Object> Tileset { get; set; }

    public List<Object> Existing { get; set; }

    public void Start()
    {
        Tileset = new Dictionary<string, Object>()
        {
            {"Empty", Resources.Load("Tileset/Empty")},
            {"Floor", Resources.Load("Tileset/Floor")},
            {"Wall", Resources.Load("Tileset/Wall")},
            {"Door", Resources.Load("Tileset/Door")},
            {"Corner", Resources.Load("Tileset/Corner")},
            {"Stairs", Resources.Load("Tileset/Stairs")}
        };
    }

    [InspectorButton("Generate")]
    public bool generate = false;

    public void Generate()
    {
        if (this.Existing.Count > 0)
        {
            foreach (Object tile in this.Existing)
            {
                DestroyImmediate(tile);
            }
        }

        if (!EditorApplication.isPlaying)
        {
            this.Start();
        }

        string[] tiles = {"Empty", "Floor", "Wall", "Door", "Corner", "Stairs"};

        for (int y = 0; y < levelY; y++)
        {
            for (int x = 0; x < levelX; x++)
            {
                Existing.Add(Instantiate(Tileset[tiles[Random.Range(0, 6)]], new Vector3(x * 2, 1, y * 2), new Quaternion()));
            }
        }
    }
}
