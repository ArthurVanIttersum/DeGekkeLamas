using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchGridSystem : MonoBehaviour
{
    public Ingredient[] ingredientTypes;
    public Vector2Int gridDimensions;
    Ingredient[,] currentGrid;
    public int seed;

    public bool autoGenerate;

    private System.Random rand;
    [SerializeField, HideInInspector] Transform gridContainer;

    [Header("Debug stuff")]
    public MeshRenderer debugCube;
    private void OnValidate()
    {
        for (int i = 0; i < ingredientTypes.Length; i++) ingredientTypes[i].index = i;

        // Grid must always be big enough for connections to be possible
        gridDimensions = new(Mathf.Max(3, gridDimensions.x), Mathf.Max(3, gridDimensions.y));

        if (autoGenerate)
            UnityEditor.EditorApplication.delayCall += () => _onValidate();
    }
    /// <summary>
    /// this feels like a crappy solution but i cant call destroy in onvalidate
    /// </summary>
    void _onValidate()
    {
        Generate();
    }
    void Start()
    {
        Generate();
    }

    [NaughtyAttributes.Button]
    void Generate()
    {
        Initialize();
        DestroyOldGrid();
        // Generate grid, regenerate if no possible connections
        GenerateGrid();
        GenerateDisplay();
        while (!SolubilityCheck())
        {
            Debug.Log("Invalid grid, regenerated with random seed");
            seed = Random.Range(int.MinValue, int.MaxValue);
            rand = new System.Random(seed);
            DestroyOldGrid();
            GenerateGrid();
            GenerateDisplay();
        }
    }

    void DestroyOldGrid()
    {
        if (gridContainer != null)
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                DestroyImmediate(gridContainer.gameObject);
            }
            else
            {
                Destroy(gridContainer.gameObject);
            }
        }
    }

    /// <summary>
    /// Initialize values
    /// </summary>
    void Initialize()
    {
        currentGrid = new Ingredient[gridDimensions.y, gridDimensions.x];
        if (seed == 0) seed = Random.Range(int.MinValue, int.MaxValue);
        rand = new System.Random(seed);
    }

    /// <summary>
    /// Randomize grid for connect 3
    /// </summary>
    void GenerateGrid()
    {
        for (int y = 0; y < currentGrid.GetLength(0); y++)
        {
            for(int x = 0; x < currentGrid.GetLength(1); x++)
            {
                List<Ingredient> possibleIngredients = new(ingredientTypes);

                // Do not spawn type if this would create 3 in a row
                if (y >= 2 && currentGrid[y-1, x].Equals(currentGrid[y-2, x]))
                {
                    possibleIngredients.Remove(currentGrid[y-1, x]);
                }
                if (x >= 2 && currentGrid[y, x-1].Equals(currentGrid[y, x-2]))
                {
                    possibleIngredients.Remove(currentGrid[y, x-1]);
                }

                currentGrid[y, x] = possibleIngredients[rand.Next(0, possibleIngredients.Count)];
            }
        }
        Debug.Log("Generated grid!");
    }

    /// <summary>
    /// Checks if there are possible matches
    /// </summary>
    bool SolubilityCheck()
    {
        int height = currentGrid.GetLength(0);
        int length = currentGrid.GetLength(1);
        //int possibleMoves = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < length; x++)
            {
                // Possible connections in middle
                int[] valueQTYs = new int[ingredientTypes.Length];

                if (y > 0) valueQTYs[currentGrid[y - 1, x].index]++;
                if (y < height-1) valueQTYs[currentGrid[y+1, x].index]++;
                if (x > 0) valueQTYs[currentGrid[y, x-1].index]++;
                if (x < length-1) valueQTYs[currentGrid[y, x+1].index]++;
                
                if (Mathf.Max(valueQTYs) >= 3)
                {
                    DebugExtension.DebugWireSphere(new(x, y, -1), Color.magenta, .25f, 10);
                    return true;
                    //possibleMoves++;
                    //continue;
                }

                // Possible connections from side
                if (Neighbours2InARow(x, y, out int axis, out int value))
                {
                    if (axis != 0 && x > 0 && currentGrid[y, x-1].index == value) // xMin
                    {
                        DebugExtension.DebugWireSphere(new(x, y, -1), ingredientTypes[value].material.color, .25f, 10);
                        return true;
                        //possibleMoves++;
                        //continue;
                    }
                    if (axis != 1 && x < length-2 && currentGrid[y, x+1].index == value) // xPlus
                    {
                        DebugExtension.DebugWireSphere(new(x, y, -1), ingredientTypes[value].material.color, .25f, 10);
                        return true;
                        //possibleMoves++;
                        //continue;
                    }
                    if (axis != 2 && y > 0 && currentGrid[y-1, x].index == value) // yMin
                    {
                        DebugExtension.DebugWireSphere(new(x, y, -1), ingredientTypes[value].material.color, .25f, 10);
                        return true;
                        //possibleMoves++;
                        //continue;
                    }
                    if (axis != 3 && y < height-2 && currentGrid[y+1, x].index == value) // yPlus
                    {
                        DebugExtension.DebugWireSphere(new(x, y, -1), ingredientTypes[value].material.color, .25f, 10);
                        return true;
                        //possibleMoves++;
                        //continue;
                    }
                }
            }
        }
        //Debug.Log(possibleMoves);
        //return possibleMoves > 0;
        return false;
    }

    /// <summary>
    /// Checks if there is a connection of 2 in a row at this nodes neighbours
    /// Axis 0 means x-, 1 means x+, 2 means y-, 3 means y+
    /// </summary>
    bool Neighbours2InARow(int x, int y, out int axis, out int ingredientValue)
    {
        axis = 0;
        ingredientValue = 0;

        if (x >= 2 && currentGrid[y, x - 1].Equals(currentGrid[y, x - 2])) // xMin
        {
            axis = 0;
            ingredientValue = currentGrid[y, x-1].index;
            return true;
        }
        if (x <= currentGrid.GetLength(1)-3 && currentGrid[y, x + 1].Equals(currentGrid[y, x + 2])) // xPlus
        {
            axis = 1;
            ingredientValue = currentGrid[y, x+1].index;
            return true;
        }
        if (y >= 2 && currentGrid[y - 1, x].Equals(currentGrid[y - 2, x])) // yMin
        {
            axis = 2;
            ingredientValue = currentGrid[y-1, x].index;
            return true;
        }
        if (y <= currentGrid.GetLength(0)-3 && currentGrid[y + 1, x].Equals(currentGrid[y + 2, x])) // yMax
        {
            axis = 3;
            ingredientValue = currentGrid[y+1, x].index;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Very basic display for the grid
    /// </summary>
    void GenerateDisplay()
    {
        gridContainer = new GameObject("GridContainer").transform;

        for (int y = 0; y < currentGrid.GetLength(0); y++)
        {
            for (int x = 0; x < currentGrid.GetLength(1); x++)
            {
                var spawned = Instantiate(debugCube, new(x, y), Quaternion.identity, gridContainer.transform);
                spawned.sharedMaterial = currentGrid[y, x].material;
                spawned.gameObject.name = $"{x}, {y}";
            }
        }
        Debug.Log("Generated grid display!");
    }
}
