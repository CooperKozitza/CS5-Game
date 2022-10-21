using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tile;

public class LevelGen : MonoBehaviour
{
    public int levelX = 0;
    public int levelY = 0;

    [Tooltip("The prefab used as a blank tile")]
    public GameObject emptyTile;

    /// <summary>
    /// The grid of tiles
    /// </summary>
    public GameObject[,] Grid { get; set; }
    
    [InspectorButton("CreateGrid")]
    public bool createGrid = false;
    /// <summary>
    /// creates a grid of blank tiles in a super-position 🦸‍♂️
    /// </summary>
    void CreateGrid()
    {
        Grid = new GameObject[levelX, levelY];
        for (int x = 0; x < levelX; x++)
        {
            for (int y = 0; y < levelY; y++)
            {
                Grid[x, y] = Instantiate(emptyTile, new Vector3(x * 2, 1, y * 2), new Quaternion(0, 0, 0, 0));
                Grid[x, y].name = string.Concat("(", x.ToString(), ", ", y.ToString(), ")");
            }
        }
    }

    [InspectorButton("DestroyGrid")]
    public bool destroyGrid = false;
    /// <summary>
    /// Destroys an existing grid if any 🔥
    /// </summary>
    void DestroyGrid()
    {
        if (Grid != null)
        {
            foreach (GameObject tile in Grid)
            {
                if (tile != null) Destroy(tile);
            }
        }
    }

    /// <summary>
    /// Coverts tile in grid at (x, y) to the specified prefab 🪄
    /// </summary>
    /// <param name="x">x position in the grid</param>
    /// <param name="y">y position in the grid</param>
    /// <param name="prefab">the prefab to convert to</param>
    void CovertTile(int x, int y, GameObject prefab)
    {
        if (x < -1 || x > levelX - 1 || y < -1 || y > levelY - 1) return;
        if (Grid[x, y] != null)
        {
            Destroy(Grid[x, y]);
            Grid[x, y] = Instantiate(prefab, new Vector3(x * 2, 1, y * 2), new Quaternion(0, 0, 0, 0));
        }
        else
        {
            Debug.Log(string.Concat("Failed to convert tile at ", x.ToString(), ", ", y.ToString()));
        }
    }

    /// <summary>
    /// Propogates a change in entropy to the adjacent tiles 🌱
    /// </summary>
    /// <param name="x">the x position in the grid of the tile to propogate from</param>
    /// <param name="y">the y position in the grid of the tile to propogate from</param>
    void PropogateEntropy(int x, int y)
    {
        // check to make sure x and y are in range of the grid array
        if (x < 0 || x > levelX || y < 0 || y > levelY) return;

        Debug.Log("Began propogation of tile at: (" + x.ToString() + ", " + y.ToString() + ")");

        GameObject tile = Grid[x, y];
        TilePrototype tilePrototype;

        List<Vector2Int> nextGeneration = new List<Vector2Int>();

        // If the tile that the changes are being propogated from is an undecided blank tile, create a new TilePrototype including all of the possibilities for that face from each tile in the entropy. This is because the PossibilitySpace component doesn't contain a Neighbors list, only an Entropy. 😵‍💫
        if (Grid[x, y].CompareTag("Undecided"))
        {
            PossibilitySpace blankTile = tile.GetComponent<PossibilitySpace>();
            if (blankTile.Entropy.Count == blankTile.DefaultEntropy.Count || blankTile.previouslyPropogated)
            {
                Debug.Log("Stopped propogation of blank ore previouly propogated tile in superposition");
                return;
            }
            
            tilePrototype = tile.AddComponent<TilePrototype>();
            blankTile.previouslyPropogated = true;

            foreach (GameObject possibility in blankTile.Entropy)
            {
                var possibilityPrototype = possibility.GetComponent<TilePrototype>();
                // Left Side possibilities ⬅️
                foreach (GameObject leftPossibility in possibilityPrototype.Left.Neighbors)
                {
                    // Debug.Log("Added " + leftPossibility.name + " to left side of blank tile at: " + x.ToString() + ", " + y.ToString());
                    if (!tilePrototype.Left.Neighbors.Contains(leftPossibility)) tilePrototype.Left.Neighbors.Add(leftPossibility);
                }
                // Front Side possibilities ⬆️
                foreach (GameObject frontPossibility in possibilityPrototype.Front.Neighbors)
                {
                    // Debug.Log("Added " + frontPossibility.name + " to front side of blank tile at: " + x.ToString() + y.ToString());
                    if (!tilePrototype.Front.Neighbors.Contains(frontPossibility)) tilePrototype.Front.Neighbors.Add(frontPossibility);
                }
                // Right Side possibilities ➡️
                foreach (GameObject rightPossibility in possibilityPrototype.Right.Neighbors)
                {
                    // Debug.Log("Added " + rightPossibility.name + " to right side of blank tile at: " + x.ToString() + y.ToString());
                    if (!tilePrototype.Right.Neighbors.Contains(rightPossibility)) tilePrototype.Right.Neighbors.Add(rightPossibility);
                }
                // Back Side possibilities ⬇️
                foreach (GameObject backPossibility in possibilityPrototype.Back.Neighbors)
                {
                    // Debug.Log("Added " + backPossibility.name + " to left back of blank tile at: " + x.ToString() + y.ToString());
                    if (!tilePrototype.Back.Neighbors.Contains(backPossibility)) tilePrototype.Back.Neighbors.Add(backPossibility);
                }
            }
        }
        else
        {
            // Attempt to retrive the TilePrototype component from the tile
            tilePrototype = tile.GetComponent<TilePrototype>();
            // Send a debug message if it is null 😥
            if (tilePrototype == null)
            {
                Debug.Log(string.Concat("Failed to get TilePrototype component at ", x.ToString(), ", ", y.ToString()));
                return;
            }
        }

        // Distribute/propogate the entropy changes to the 4 adjacent tile
        // Left Side
        if (x - 1 > -1)
        {
            // only propogate the changes in entopy to an undecided blank prefab, not a decided tile 🚫
            if (Grid[x - 1, y].CompareTag("Undecided"))
            {
                bool wasChange = false;
                // the tile on the left side
                PossibilitySpace blankTile = Grid[x - 1, y].GetComponent<PossibilitySpace>();

                // Remove the possibility of the blank tile if it is not in the valid neighbor list of the tile currently propogating
                for (int i = blankTile.Entropy.Count - 1; i > -1; i--)
                {
                    if (!tilePrototype.Left.Neighbors.Contains(blankTile.Entropy[i]))
                    {
                        blankTile.Entropy.RemoveAt(i);
                        wasChange = true;
                    }
                }

                // If a change was made to the left side tile's entropy, propogate it to it's adjacent tiles.
                if (wasChange) nextGeneration.Add(new Vector2Int(x - 1, y));
            }
        }
        // Front Side
        if (y + 1 < levelY)
        {
            // only propogate the changes in entopy to an undecided blank prefab, not a decided tile 🚫
            if (Grid[x, y + 1].CompareTag("Undecided"))
            {
                bool wasChange = false;
                // the tile on the left side
                PossibilitySpace blankTile = Grid[x, y + 1].GetComponent<PossibilitySpace>();

                // Remove the possibility of the blank tile if it is not in the valid neighbor list of the tile currently propogating
                for (int i = blankTile.Entropy.Count - 1; i > -1; i--)
                {
                    if (!tilePrototype.Front.Neighbors.Contains(blankTile.Entropy[i]))
                    {
                        blankTile.Entropy.RemoveAt(i);
                        wasChange = true;
                    }
                }

                // If a change was made to the front side tile's entropy, propogate it to it's adjacent tiles.
                if (wasChange) nextGeneration.Add(new Vector2Int(x, y + 1));
            }
        }
        // Right Side
        if  (x + 1 < levelX)
        {
            // only propogate the changes in entopy to an undecided blank prefab, not a decided tile 🚫
            if (Grid[x + 1, y].CompareTag("Undecided"))
            {
                bool wasChange = false;
                // the tile on the left side
                PossibilitySpace blankTile = Grid[x + 1, y].GetComponent<PossibilitySpace>();

                // Remove the possibility of the blank tile if it is not in the valid neighbor list of the tile currently propogating
                for (int i = blankTile.Entropy.Count - 1; i > -1; i--)
                {
                    if (!tilePrototype.Right.Neighbors.Contains(blankTile.Entropy[i]))
                    {
                        blankTile.Entropy.RemoveAt(i);
                        wasChange = true;
                    }
                }

                // If a change was made to the right side tile's entropy, propogate it to it's adjacent tiles.
                if (wasChange) nextGeneration.Add(new Vector2Int(x + 1, y));
            }
        }
        // Back Side
        if  (y - 1 > -1)
        {
            // only propogate the changes in entopy to an undecided blank prefab, not a decided tile 🚫
            if (Grid[x, y - 1].CompareTag("Undecided"))
            {
                bool wasChange = false;
                // the tile on the left side
                PossibilitySpace blankTile = Grid[x, y - 1].GetComponent<PossibilitySpace>();

                // Remove the possibility of the blank tile if it is not in the valid neighbor list of the tile currently propogating
                for (int i = blankTile.Entropy.Count - 1; i > -1; i--)
                {
                    if (!tilePrototype.Back.Neighbors.Contains(blankTile.Entropy[i]))
                    {
                        blankTile.Entropy.RemoveAt(i);
                        wasChange = true;
                    }
                }

                // If a change was made to the back side tile's entropy, propogate it to it's adjacent tiles.
                if (wasChange) nextGeneration.Add(new Vector2Int(x, y - 1));
            }
        }

        foreach (Vector2Int coord in nextGeneration)
        {
            PropogateEntropy(coord.x, coord.y);
        }

        if (tile.CompareTag("Undecided"))
        {
            Destroy(tilePrototype);
            tile.GetComponent<PossibilitySpace>().previouslyPropogated = false;
        }
    }

    /// <summary>
    /// Determines the blank tile with the lowest entropy from the adjacent tiles of the tile at (x, y)
    /// </summary>
    /// <param name="x">the x coordinate in the grid of the tile to check around</param>
    /// <param name="y">the y coordinate in the grid of the tile to check around</param>
    /// <returns>the x/y coordinate of the next tile in a vector</returns>
    Vector2Int GetNextTileCoords(int x, int y)
    {
        List<PossibilitySpace> options = new List<PossibilitySpace>();
        List<Vector2Int> optionCoordinates = new List<Vector2Int>();

        // Fill the list of options with the four adjacent tiles around the tile at (x, y)
        for (int i = 0; i < 4; i++)
        {
            int xOffset = i == 0 ? -1 : i == 2 ? 1 : 0;
            int yOffset = i == 1 ? 1 : i == 3 ? -1 : 0;

            Debug.Log(xOffset.ToString() + ", " + yOffset.ToString());

            if (x + xOffset > -1 && x + xOffset < levelX && y + yOffset > -1 && y + yOffset < levelY)
            {
                if (Grid[x + xOffset, y + yOffset] != null)
                {
                    if (Grid[x + xOffset, y + yOffset].CompareTag("Undecided"))
                    {
                        PossibilitySpace option = Grid[x + xOffset, y + yOffset].GetComponent<PossibilitySpace>();
                        if (option.Entropy.Count > 0)
                        {
                            options.Add(option);
                            optionCoordinates.Add(new Vector2Int(x + xOffset, y + yOffset));
                        }
                    }
                }
            }    
        }

        // Remove options until only the shortest entropy tiles remain
        for (int i = 0; i < options.Count; i++)
        { 
            for (int j = 0; j < options.Count; j++)
            {
                if (options[j].Entropy.Count < options[i].Entropy.Count && i != j)
                {
                    options.RemoveAt(j);
                    optionCoordinates.RemoveAt(j);
                }
            }
        }

        // If there is more than one option, choose one at random, otherwise return the only option
        if (options.Count > 1)
        {
            return optionCoordinates[Random.Range(0, optionCoordinates.Count + 1)];
        }
        else if (options.Count == 0)
        {
            Debug.Log("Failed to retrive next tile for tile at: " + new Vector2Int(x, y).ToString());
            return new Vector2Int(x - 1, y);
        }
        else
        {
            return optionCoordinates[0];
        }
    }

    // Generation:

    Vector2Int LastTile { get; set; }

    [InspectorButton("StartGeneration")]
    public bool startGeneration = false;
    /// <summary>
    /// Prepares the level for generation, and takes the first step 👶
    /// </summary>
    void StartGeneration()
    {
        // Reset Grid
        if (Grid == null) CreateGrid();
        foreach (GameObject tile in Grid)
        {
            if (!tile.CompareTag("Undecided"))
            {
                DestroyGrid();
                CreateGrid();
            }
        }

        // Choose a random tile to start with
        int randX = Random.Range(0, levelX);
        int randY = Random.Range(0, levelY);

        PossibilitySpace randomTile = Grid[randX, randY].GetComponent<PossibilitySpace>();
        CovertTile(randX, randY, randomTile.Entropy[Random.Range(0, randomTile.Entropy.Count + 1)]);
        PropogateEntropy(randX, randY);
        LastTile = new Vector2Int(randX, randY);
    }

    /// <summary>
    /// One 'step' in the algorithm
    /// </summary>
    /// <param name="lastX"></param>
    /// <param name="lastY"></param>
    void Step(int lastX, int lastY)
    {
        Vector2Int nextTileCoords = GetNextTileCoords(lastX, lastY);

        Debug.Log("Chose: " + nextTileCoords.ToString());

        if (!Grid[nextTileCoords.x, nextTileCoords.y].CompareTag("Undecided"))
        {
            Debug.Log("Tile selected by the GetNextTileCoords was not undecided");
            return;
        }

        PossibilitySpace nextTileOptions = Grid[nextTileCoords.x, nextTileCoords.y].GetComponent<PossibilitySpace>();

        CovertTile(nextTileCoords.x, nextTileCoords.y, nextTileOptions.Entropy[Random.Range(0, nextTileOptions.Entropy.Count + 1)]);
        PropogateEntropy(nextTileCoords.x, nextTileCoords.y);
        LastTile = new Vector2Int(nextTileCoords.x, nextTileCoords.y);
    }

    [InspectorButton("Generate")]
    public bool generate = false;
    void Generate()
    {
        bool complete = false;

        while (!complete)
        {
            Step(LastTile.x, LastTile.y);
            for (int i = 0; i < levelX; i++)
            {
                for (int j = 0; j < levelY; j++)
                {
                    if (Grid[i, j] != null && Grid[i, j].CompareTag("Undecided"))
                    {
                        complete = false;
                        break;
                    }
                    if (i == levelX - 1 && j == levelY - 1) complete = true;
                }
                if (!complete) break;
            }
        }
    }

}
