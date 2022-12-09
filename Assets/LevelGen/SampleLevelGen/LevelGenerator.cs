using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    private LevelManager levelManager;
    public GameObject levelManagerObj;

    [Range(0, 5)]
    public int maxStrikes = 0;

    private int workingFloor = 0;

    void Awake()
    {
        levelManager = levelManagerObj.GetComponent<LevelManager>();
    }

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
        Grid = new GameObject[levelManager.levelX, levelManager.levelY];

        for (int y = 0; y < levelManager.levelY; y++)
        {
            for (int x = 0; x < levelManager.levelX; x++)
            {
                Grid[x, y] = Instantiate(emptyTile, new Vector3(x * 2, workingFloor * 2.8f, (levelManager.levelY - y) * 2), new Quaternion(0, 0, 0, 0));
                Grid[x, y].name = string.Concat("(", x.ToString(), ", ", y.ToString(), ")");
            }
        }

        for (int y = 0; y < levelManager.levelY; y++)
        {
            for (int x = 0; x < levelManager.levelX; x++)
            {
                if (x == 0 || x == levelManager.levelX - 1 || y == 0 || y == levelManager.levelY - 1)
                {
                    if (!((x == 0 && y == 0) || (x == levelManager.levelX - 1 && y == levelManager.levelY - 1) || (x == 0 && y == levelManager.levelY - 1) || (y == 0 && x == levelManager.levelX - 1)))
                    {
                        int rotation = x == 0 ? 2 : y == 0 ? 3 : y == levelManager.levelY - 1 ? 1 : 0;
                        PossibilitySpace possibilitySpace = Grid[x, y].GetComponent<PossibilitySpace>();

                        for (int i = possibilitySpace.Entropy.Count - 1; i > -1; i--)
                        {
                            TilePrototype tilePrototype = possibilitySpace.Entropy[i].GetComponent<TilePrototype>();
                            if (tilePrototype.rotation != rotation || tilePrototype.invariable) possibilitySpace.Entropy.RemoveAt(i);
                        }

                        PropagateEntropy(x, y);
                    }
                }
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
    public void ConvertTile(int x, int y, GameObject prefab)
    {
        TilePrototype proto = prefab.GetComponent<TilePrototype>();
        if (proto == null)
        {
            Debug.Log("GEN: Attempted to covert tile to a non-tile gameObject :(");
        }
        if (x < -1 || x > levelManager.levelX - 1 || y < -1 || y > levelManager.levelY - 1) return;
        if (Grid[x, y] != null)
        {
            Destroy(Grid[x, y]);
            Quaternion quaternion = new Quaternion();
            quaternion.eulerAngles = new Vector3(0, proto.rotation * 90, 0);
            Grid[x, y] = Instantiate(prefab, new Vector3(x * 2, workingFloor * 2.8f, (levelManager.levelY - y) * 2), quaternion);
            Grid[x, y].name = "(" + x.ToString() + ", " + y.ToString() + ")";
        }
        else
        {
            Debug.Log(string.Concat("GEN: Failed to convert tile at ", x.ToString(), ", ", y.ToString()));
        }
    }

    /// <summary>
    /// Propagates a change in entropy to the adjacent tiles 🌱
    /// </summary>
    /// <param name="x">the x position in the grid of the tile to propagate from</param>
    /// <param name="y">the y position in the grid of the tile to propagate from</param>
    void PropagateEntropy(int x, int y)
    {
        // check to make sure x and y are in range of the grid array
        if (x < 0 || x > levelManager.levelX || y < 0 || y > levelManager.levelY) return;

        GameObject tile = Grid[x, y];
        NeighborsList tilePrototype;

        List<Vector2Int> nextGeneration = new List<Vector2Int>();

        // If the tile that the changes are being propagated from is an undecided blank tile, create a new TilePrototype including all of the possibilities for that face from each tile in the entropy. This is because the PossibilitySpace component doesn't contain a Neighbors list, only an Entropy. 😵‍💫
        if (Grid[x, y].CompareTag("Undecided"))
        {
            PossibilitySpace blankTile = tile.GetComponent<PossibilitySpace>();
            if (blankTile.Entropy.Count == blankTile.DefaultEntropy.Count || blankTile.previouslyPropagated)
            {
                Debug.Log("GEN: Stopped propagation of blank or previously propagated tile in superposition");
                return;
            }

            tilePrototype = (NeighborsList)ScriptableObject.CreateInstance("NeighborsList");
            if (tilePrototype == null)
            {
                Debug.Log("GEN: Failed to create NeighborsList Instance");
                return;
            }
            blankTile.previouslyPropagated = true;

            foreach (GameObject possibility in blankTile.Entropy)
            {
                var possibilityPrototype = possibility.GetComponent<TilePrototype>();
                // Left Side possibilities ⬅️
                foreach (GameObject leftPossibility in possibilityPrototype.neighborsList.Left.neighbors)
                {
                    // Debug.Log("GEN: Added " + leftPossibility.name + " to left side of blank tile at: " + x.ToString() + ", " + y.ToString());
                    if (!tilePrototype.Left.neighbors.Contains(leftPossibility)) tilePrototype.Left.neighbors.Add(leftPossibility);
                }
                // Front Side possibilities ⬆️
                foreach (GameObject frontPossibility in possibilityPrototype.neighborsList.Front.neighbors)
                {
                    // Debug.Log("GEN: Added " + frontPossibility.name + " to front side of blank tile at: " + x.ToString() + y.ToString());
                    if (!tilePrototype.Front.neighbors.Contains(frontPossibility)) tilePrototype.Front.neighbors.Add(frontPossibility);
                }
                // Right Side possibilities ➡️
                foreach (GameObject rightPossibility in possibilityPrototype.neighborsList.Right.neighbors)
                {
                    // Debug.Log("GEN: Added " + rightPossibility.name + " to right side of blank tile at: " + x.ToString() + y.ToString());
                    if (!tilePrototype.Right.neighbors.Contains(rightPossibility)) tilePrototype.Right.neighbors.Add(rightPossibility);
                }
                // Back Side possibilities ⬇️
                foreach (GameObject backPossibility in possibilityPrototype.neighborsList.Back.neighbors)
                {
                    // Debug.Log("GEN: Added " + backPossibility.name + " to left back of blank tile at: " + x.ToString() + y.ToString());
                    if (!tilePrototype.Back.neighbors.Contains(backPossibility)) tilePrototype.Back.neighbors.Add(backPossibility);
                }
            }
        }
        else
        {
            // Attempt to retrieve the TilePrototype component from the tile
            tilePrototype = tile.GetComponent<TilePrototype>().neighborsList;
            // Send a debug message if it is null 😥
            if (tilePrototype == null)
            {
                Debug.Log(string.Concat("Failed to get TilePrototype component at ", x.ToString(), ", ", y.ToString()));
                return;
            }
        }

        // Distribute/propagate the entropy changes to the 4 adjacent tile
        // Left Side
        if (x - 1 > -1)
        {
            Vector2Int target = new Vector2Int(x - 1, y);
            if (PropagateEntropyOfFace(tilePrototype, target, Directions.Left)) nextGeneration.Add(target);
        }
        // Front Side
        if (y + 1 < levelManager.levelY)
        {
            Vector2Int target = new Vector2Int(x, y + 1);
            if (PropagateEntropyOfFace(tilePrototype, target, Directions.Front)) nextGeneration.Add(target);
        }
        // Right Side
        if  (x + 1 < levelManager.levelX)
        {
            Vector2Int target = new Vector2Int(x + 1, y);
            if (PropagateEntropyOfFace(tilePrototype, target, Directions.Right)) nextGeneration.Add(target);
        }
        // Back Side
        if  (y - 1 > -1)
        {
            Vector2Int target = new Vector2Int(x, y - 1);
            if (PropagateEntropyOfFace(tilePrototype, target, Directions.Back)) nextGeneration.Add(target);
        }

        foreach (Vector2Int coord in nextGeneration)
        {
            PropagateEntropy(coord.x, coord.y);
        }

        if (tile.CompareTag("Undecided"))
        {
            Destroy(tilePrototype);
            tile.GetComponent<PossibilitySpace>().previouslyPropagated = false;
        }
    }

    private enum Directions { Left, Front, Right, Back }
    bool PropagateEntropyOfFace(NeighborsList neighborsList, Vector2Int target, Directions direction)
    {
        // only propogate the changes in entopy to an undecided blank prefab, not a decided tile 🚫
        if (Grid[target.x, target.y].CompareTag("Undecided"))
        {
            bool wasChange = false;
            // the tile on the left side
            PossibilitySpace blankTile = Grid[target.x, target.y].GetComponent<PossibilitySpace>();
            List<GameObject> neighbors =
                direction == Directions.Left ? neighborsList.Left.neighbors :
                direction == Directions.Front ? neighborsList.Front.neighbors :
                direction == Directions.Right ? neighborsList.Right.neighbors :
                direction == Directions.Back ? neighborsList.Back.neighbors : null;

            // Remove the possibility of the blank tile if it is not in the valid neighbor list of the tile currently propogating
            for (int i = blankTile.Entropy.Count - 1; i > -1; i--)
            {
                if (!neighbors.Contains(blankTile.Entropy[i]))
                {
                    blankTile.Entropy.RemoveAt(i);
                    wasChange = true;
                }
            }

            // If a change was made to the front side tile's entropy, propogate it to it's adjacent tiles.
            return wasChange;
        }
        else return false;
    }

    GameObject ChoosePrefabFromEntropy(int x, int y)
    {
        if (x < 0 || x > levelManager.levelX || y < 0 || y > levelManager.levelY || !Grid[x, y].CompareTag("Undecided")) return null;

        List<GameObject> entropy = Grid[x, y].GetComponent<PossibilitySpace>().Entropy;
        List<TilePrototype> tiles = new List<TilePrototype>();
        foreach(GameObject option in entropy)
        {
            tiles.Add(option.GetComponent<TilePrototype>());
        }

        int percentageSum = 0;
        foreach (TilePrototype option in tiles) percentageSum += option.probability;

        int rand = Random.Range(0, percentageSum);
        foreach (TilePrototype option in tiles)
        {
            rand -= option.probability;
            if (rand <= 0) return option.neighborsList.Self;
        }

        return null;
    }

    /// <summary>
    /// Determines the blank tile with the lowest entropy from the adjacent tiles of the tile at (x, y)
    /// </summary>
    /// <param name="lastX">the x coordinate in the grid of the tile to check around</param>
    /// <param name="lastY">the y coordinate in the grid of the tile to check around</param>
    /// <returns>the x/y coordinate of the next tile in a vector</returns>
    Vector2Int GetNextTileCoords(int lastX, int lastY)
    {
        List<PossibilitySpace> options = new List<PossibilitySpace>();
        List<Vector2Int> optionCoordinates = new List<Vector2Int>();

        // Fill the list of options with the four adjacent tiles around the tile at (x, y)
        for (int i = 0; i < 4; i++)
        {
            int xOffset = i == 0 ? -1 : i == 2 ? 1 : 0;
            int yOffset = i == 1 ? 1 : i == 3 ? -1 : 0;

            if (lastX + xOffset > -1 && lastX + xOffset < levelManager.levelX && lastY + yOffset > -1 && lastY + yOffset < levelManager.levelY)
            {
                if (Grid[lastX + xOffset, lastY + yOffset] != null)
                {
                    if (Grid[lastX + xOffset, lastY + yOffset].CompareTag("Undecided"))
                    {
                        PossibilitySpace option = Grid[lastX + xOffset, lastY + yOffset].GetComponent<PossibilitySpace>();
                        if (option.Entropy.Count > 0)
                        {
                            options.Add(option);
                            optionCoordinates.Add(new Vector2Int(lastX + xOffset, lastY + yOffset));
                        }
                    }
                }
            }    
        }

        if (options.Count == 0)
        {
            Vector2Int lowestEntropyCoords = new Vector2Int();
            int lowestEntropy = int.MaxValue;
            bool foundTile = false;
            for (int y = 0; y < levelManager.levelY; y++)
            {
                for (int x = 0; x < levelManager.levelX; x++)
                {
                    if (Grid[x, y].CompareTag("Undecided"))
                    {
                        PossibilitySpace possibilitySpace = Grid[x, y].GetComponent<PossibilitySpace>();
                        if (possibilitySpace.Entropy.Count < lowestEntropy)
                        {
                            foundTile = true;
                            lowestEntropy = possibilitySpace.Entropy.Count;
                            lowestEntropyCoords = new Vector2Int(x, y);
                        }
                    }
                }
            }
            if (!foundTile)
            {
                Debug.Log("GEN: Attempted to find next tile in a completed level");
            }
            return lowestEntropyCoords;
        }

        // Remove options until only the shortest entropy tiles remain
        for (int i = options.Count - 1; i > -1; i--)
        {
            for (int j = options.Count - 1; j > -1; j--)
            {
                if (i > options.Count || i < 0 || options.Count < 2 || i == j) break;
                if (options[j].Entropy.Count < options[i].Entropy.Count && options[j].Entropy.Count - 1 > 0)
                {
                    options.RemoveAt(j);
                    optionCoordinates.RemoveAt(j);
                }
                if (options.Count < 2) break;
            }
        }

        // If there is more than one option, choose one at random, otherwise return the only option
        if (options.Count > 1)
        {
            return optionCoordinates[Random.Range(0, optionCoordinates.Count)];
        }
        else if (options.Count < 1)
        {
            Debug.Log("GEN: Failed to retrieve next tile for tile at: " + new Vector2Int(lastX, lastY).ToString());
            return new Vector2Int(lastX - 1, lastY);
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
        int randX = Random.Range(0, levelManager.levelX);
        int randY = Random.Range(0, levelManager.levelY);

        PossibilitySpace randomTile = Grid[randX, randY].GetComponent<PossibilitySpace>();
        ConvertTile(randX, randY, randomTile.Entropy[Random.Range(0, randomTile.Entropy.Count)]);
        PropagateEntropy(randX, randY);
        LastTile = new Vector2Int(randX, randY);
    }

    void StartGenerationWithSeed(Room seed)
    {
        List<Vector2Int> seedCoords = (List<Vector2Int>)Enumerable.Concat(seed.tileCoords, seed.sharedTileCoords);
		List<TilePrototype> seedTiles = (List<TilePrototype>)Enumerable.Concat(seed.tiles, seed.sharedTiles);

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

        if (seedCoords.Count != seedTiles.Count)
        {
            Debug.Log("GEN: failed to start generation with seed");
            return;
        }

        List<Vector2Int> seededPossibilitySpaces = new();
        foreach (Vector2Int coord in seedCoords)
        {
            if (coord.x > -1 && coord.x < levelManager.levelX && coord.y > -1 && coord.y < levelManager.levelY)
            {
                if (Grid[coord.x, coord.y].CompareTag("Undecided"))
                {
                    PossibilitySpace possibilitySpace = Grid[coord.x, coord.y].GetComponent<PossibilitySpace>();
                    possibilitySpace.Entropy.RemoveAll(x => x != seedTiles[seedCoords.IndexOf(coord)].neighborsList.Self);
                    seededPossibilitySpaces.Add(coord);
                }
				else 
				{
					Debug.Log("GEN: failed to start generation with seed");
            		return;
				}
            }
        }
        foreach (Vector2Int coord in seededPossibilitySpaces)
        {
            PropagateEntropy(coord.x, coord.y);
        }
        LastTile = seededPossibilitySpaces[seededPossibilitySpaces.Count - 1];
    }

    private int strikes = 0;

    [InspectorButton("Step")]
    public bool step = false;
    /// <summary>
    /// One 'step' in the algorithm
    /// </summary>
    void Step()
    {
        Vector2Int nextTileCoords = GetNextTileCoords(LastTile.x, LastTile.y);

        if (!Grid[nextTileCoords.x, nextTileCoords.y].CompareTag("Undecided"))
        {
            Debug.Log("GEN: Tile selected by the GetNextTileCoords was not undecided");
            return;
        }

        GameObject prefab = ChoosePrefabFromEntropy(nextTileCoords.x, nextTileCoords.y);
        if (prefab == null)
        {
            Debug.Log("GEN: Failed to retrieve tile from choices");
            return;
        }
        ConvertTile(nextTileCoords.x, nextTileCoords.y, prefab);
        PropagateEntropy(nextTileCoords.x, nextTileCoords.y);
        LastTile = new Vector2Int(nextTileCoords.x, nextTileCoords.y);

        for (int y = 0; y < levelManager.levelY; y++)
        {
            for (int x = 0; x < levelManager.levelX; x++)
            {
                if (!Grid[x, y].CompareTag("Undecided")) continue;
                PossibilitySpace possibilitySpace = Grid[x, y].GetComponent<PossibilitySpace>();
                if (possibilitySpace.Entropy.Count == 1) ConvertTile(x, y, possibilitySpace.Entropy[0]);
                else if (possibilitySpace.Entropy.Count == 0)
                {
                    if (strikes < maxStrikes)
                    {
                        strikes++;
                        CreatePatch(levelManager.levelX * levelManager.levelY > 144 ? 2 : 1, new Vector2Int(x, y));
                        Debug.Log("GEN: Generation Error Strike: " + strikes.ToString() + "/" + maxStrikes.ToString() + ", Generated Patch, Continuing...");
                    }
                    else
                    {
                        strikes = 0;
                        DestroyGrid();
                        CreateGrid();
                        StartGeneration();
                    }
                }
            }
        }
    }

    [InspectorButton("Generate")]
    public bool generate = false;
    public void Generate(int floor, Room seed = null)
    {
        workingFloor = floor;

        CreateGrid();
		
		if (seed != null) StartGenerationWithSeed(seed);
        else StartGeneration();

        bool complete = false;
        while (!complete)
        {
            Step();
            complete = true;
            for (int y = 0; y < levelManager.levelY; y++)
            {
                for (int x = 0; x < levelManager.levelX; x++)
                {
                    if (Grid[x, y].CompareTag("Undecided"))
                    {
                        complete = false;
                        break;
                    }
                }
                if (!complete) break;
            }
        }
        bool valid = true;
        if (complete)
        {
            NeighborsList[,] validationGrid = new NeighborsList[levelManager.levelX, levelManager.levelY];
            for (int y = 0; y < levelManager.levelY; y++)
            {
                for (int x = 0; x < levelManager.levelX; x++)
                {
                    validationGrid[x, y] = Grid[x, y].GetComponent<TilePrototype>().neighborsList;
                }
            }

            for (int y = 0; y < levelManager.levelY; y++)
            {
                for (int x = 0; x < levelManager.levelX; x++)
                {
                    GameObject self = validationGrid[x, y].Self;
                    if (x - 1 > -1)
                    {
                        if (!validationGrid[x - 1, y].Right.neighbors.Contains(self)) valid = false;
                    }
                    if (y + 1 < levelManager.levelY)
                    {
                        if (!validationGrid[x, y + 1].Back.neighbors.Contains(self)) valid = false;
                    }
                    if (x + 1 < levelManager.levelX)
                    {
                        if (!validationGrid[x + 1, y].Left.neighbors.Contains(self)) valid = false;
                    }
                    if (y - 1 > -1)
                    {
                        if (!validationGrid[x, y - 1].Front.neighbors.Contains(self)) valid = false;
                    }
                    if (!valid) break;
                }
                if (!valid) break;
            }
        }
        if (valid)
        {
            levelManager.Mansion.Add(Grid);
            Debug.Log("GEN: Generation Success! Took: " + Time.deltaTime.ToString() + " seconds.");
        }
        else
        {
            DestroyGrid();
            Generate(floor);
        }
    }


    void CreatePatch(int size, Vector2Int center)
    {
        if (center.x < 0 || center.x > levelManager.levelX - 1 || center.y < 0 || center.y > levelManager.levelY - 1) return;
        // replace tiles around center with blank tiles
        for (int y = center.y - size; y < center.y + size + 1; y++)
        {
            for (int x = center.x - size; x < center.x + size + 1; x++)
            {
                if (x < 0 || x > levelManager.levelX - 1 || y < 0 || y > levelManager.levelY - 1) continue;
                Destroy(Grid[x, y]);
                Grid[x, y] = Instantiate(emptyTile, new Vector3(x * 2, workingFloor * 2.8f, (levelManager.levelY - y) * 2), new Quaternion(0, 0, 0, 0));
            }
        }
        // propagates edges of the patch into it.
        for (int y = center.y - (size + 1); y < center.y + size + 2; y++)
        {
            for (int x = center.x - (size + 1); x < center.x + size + 2; x++)
            {
                if (x < 0 || x > levelManager.levelX - 1 || y < 0 || y > levelManager.levelY - 1) continue;
                if (!((x == 0 && y == 0) || (x == center.x + size + 1 && y == center.y + size + 1) || (x == 0 && y == center.y + size + 1) || (y == 0 && x == center.x + size + 1)))
                {
                    PropagateEntropy(x, y);
                }
            }
        }
    }
}
