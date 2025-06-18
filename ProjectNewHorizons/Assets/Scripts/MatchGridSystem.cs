using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MatchGridSystem : MonoBehaviour
{
    public Ingredient[] ingredientTypes;
    public Vector2Int gridDimensions;
    public Ingredient[,] currentGrid;
    public int seed;

    public bool autoGenerate;

    [Header("Requirements")]
    public Ingredient[] requiredIngredients;
    public int totalIngredientQTY;
    public bool autoResize;

    private System.Random rand;
    [SerializeField, HideInInspector] Transform gridContainer;

    [Header("Debug stuff")]
    public MeshRenderer debugCube;
    private void OnValidate()
    {
        for (int i = 0; i < ingredientTypes.Length; i++) ingredientTypes[i].index = i;
        for(int i = 0;i < requiredIngredients.Length; i++)
        {
            requiredIngredients[i].index = ingredientTypes[ 
                Ingredient.FindByName(ingredientTypes, requiredIngredients[i].name) 
                ].index;
        }

        totalIngredientQTY = Mathf.Max(totalIngredientQTY, requiredIngredients.Length);

        // Grid must always be big enough for connections to be possible
        gridDimensions = new(Mathf.Max(3, gridDimensions.x), Mathf.Max(3, gridDimensions.y));

        if (autoGenerate)
            UnityEditor.EditorApplication.delayCall += () => _onValidate();
    }
    /// <summary>
    /// this feels like a crappy solution but i cant call destroy in OnValidate
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
        if (autoResize)
        {
            gridDimensions = new(totalIngredientQTY+1, totalIngredientQTY + 1);
            gridDimensions = new(Mathf.Max(3, gridDimensions.x), Mathf.Max(3, gridDimensions.y));
        }

        Initialize();
        DestroyOldGrid();
        // Generate grid, regenerate if no possible connections
        GenerateGrid();
        while (!SolubilityCheck(out int[] ingredientsGenerated) || !EnoughIngredientsPresent(requiredIngredients, ingredientsGenerated))
        {
            Debug.Log("Invalid grid, regenerated with random seed");
            seed = Random.Range(int.MinValue, int.MaxValue);
            rand = new System.Random(seed);
            GenerateGrid();
        }
        DestroyOldGrid();
        if (debugCube != null) GenerateDisplay();
    }

    /// <summary>
    /// Destroys old grid display
    /// </summary>
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
        int[] quantities = new int[ingredientTypes.Length];
        foreach(var ingredient in requiredIngredients)
        {
            quantities[ingredient.index]++;
        }
        List<float> weights = new(); // Weights is used for making ingredients in the required list have a higher ratio in the generated grid
        int total = MathTools.ArrayTotal(quantities);
        for(int i = 0; i < ingredientTypes.Length; i++)
        {
            weights.Add((quantities[i]+1) / (float)total * 100);
        }
        Debug.Log(weights.Count);

        for (int y = 0; y < currentGrid.GetLength(0); y++)
        {
            for(int x = 0; x < currentGrid.GetLength(1); x++)
            {
                List<Ingredient> possibleIngredients = new(ingredientTypes);

                // Do not spawn type if this would create 3 in a row
                List<int> indexesToRemove = new();
                if (y >= 2 && currentGrid[y-1, x].IndexEquals(currentGrid[y-2, x]))
                {
                    indexesToRemove.Add(currentGrid[y - 1, x].index);
                }
                if (x >= 2 && currentGrid[y, x-1].IndexEquals(currentGrid[y, x-2]))
                {
                    indexesToRemove.Add(currentGrid[y, x - 1].index);
                }
                // Remove objects that shouldnt spawn, in correct order to avoid deleting the order one if indexes shifted
                List<float> localWeights = new(weights);
                indexesToRemove.Sort();
                indexesToRemove.Reverse();
                for(int i = 0; i < indexesToRemove.Count; i++)
                {
                    int index = indexesToRemove[i];
                    possibleIngredients.RemoveAt(index);
                    localWeights.RemoveAt(index);
                }
                //currentGrid[y, x] = possibleIngredients[rand.Next(0, possibleIngredients.Count)];
                localWeights = ReadjustWeights(localWeights.ToArray(), 100).ToList();
                currentGrid[y, x] = WeightedRandomOption(possibleIngredients.ToArray(), localWeights.ToArray());
            }
        }
        Debug.Log("Generated grid!");
    }
    /// <summary>
    /// Gets a random from a list, the values of all weights in the weight list should add up to 100
    /// </summary>
    T WeightedRandomOption<T>(T[] possibilities, float[] weights)
    {
        if (possibilities.Length != weights.Length) Debug.LogWarning("arrays are different lengths");

        int value = rand.Next(0, 100);
        float currentweight = 0;
        for ( int i = 0; i < possibilities.Length; i++)
        {
            currentweight += weights[i];
            if (currentweight > value) return possibilities[i];
        }
        Debug.Log("It failed for mysterious reasons");
        return default;
    }

    /// <summary>
    /// Checks if there are possible matches
    /// </summary>
    bool SolubilityCheck(out int[] ingredientsGenerated)
    {
        int height = currentGrid.GetLength(0);
        int length = currentGrid.GetLength(1);
        ingredientsGenerated = new int[ingredientTypes.Length];
        int possibleMoves = 0;
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
                
                if (Mathf.Max(valueQTYs) >= 3) // 3 neighbours means possible connection
                {
                    DebugExtension.DebugWireSphere(new(x, y, -1), Color.magenta, .25f, 10);
                    //return true;
                    possibleMoves++;
                    continue;
                }

                // Possible connections from side
                if (Neighbours2InARow(x, y, out int axis, out int value))
                {
                    if (axis != 0 && x > 0 && currentGrid[y, x-1].index == value) // xMin
                    {
                        DebugExtension.DebugWireSphere(new(x, y, -1), ingredientTypes[value].material.color, .25f, 10);
                        //return true;
                        ingredientsGenerated[value]++;
                        possibleMoves++;
                        continue;
                    }
                    if (axis != 1 && x < length-2 && currentGrid[y, x+1].index == value) // xPlus
                    {
                        DebugExtension.DebugWireSphere(new(x, y, -1), ingredientTypes[value].material.color, .25f, 10);
                        //return true;
                        ingredientsGenerated[value]++;
                        possibleMoves++;
                        continue;
                    }
                    if (axis != 2 && y > 0 && currentGrid[y-1, x].index == value) // yMin
                    {
                        DebugExtension.DebugWireSphere(new(x, y, -1), ingredientTypes[value].material.color, .25f, 10);
                        //return true;
                        ingredientsGenerated[value]++;
                        possibleMoves++;
                        continue;
                    }
                    if (axis != 3 && y < height-2 && currentGrid[y+1, x].index == value) // yPlus
                    {
                        DebugExtension.DebugWireSphere(new(x, y, -1), ingredientTypes[value].material.color, .25f, 10);
                        //return true;
                        ingredientsGenerated[value]++;
                        possibleMoves++;
                        continue;
                    }
                }
            }
        }
        Debug.Log(possibleMoves);
        return possibleMoves > 0;
        //return false;
    }

    /// <summary>
    /// Checks if all required ingredients are present in the int[], using the ingredients invisible index
    /// </summary>
    bool EnoughIngredientsPresent(Ingredient[] required, int[] quantities)
    {
        foreach (Ingredient ingredient in required)
        {
            if (quantities[ingredient.index] > 0)
                quantities[ingredient.index]--;
            else return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if there is a connection of 2 in a row at this nodes neighbours
    /// Axis 0 means x-, 1 means x+, 2 means y-, 3 means y+
    /// </summary>
    bool Neighbours2InARow(int x, int y, out int axis, out int ingredientValue)
    {
        axis = 0;
        ingredientValue = 0;

        if (x >= 2 && currentGrid[y, x - 1].IndexEquals(currentGrid[y, x - 2])) // xMin
        {
            axis = 0;
            ingredientValue = currentGrid[y, x-1].index;
            return true;
        }
        if (x <= currentGrid.GetLength(1)-3 && currentGrid[y, x + 1].IndexEquals(currentGrid[y, x + 2])) // xPlus
        {
            axis = 1;
            ingredientValue = currentGrid[y, x+1].index;
            return true;
        }
        if (y >= 2 && currentGrid[y - 1, x].IndexEquals(currentGrid[y - 2, x])) // yMin
        {
            axis = 2;
            ingredientValue = currentGrid[y-1, x].index;
            return true;
        }
        if (y <= currentGrid.GetLength(0)-3 && currentGrid[y + 1, x].IndexEquals(currentGrid[y + 2, x])) // yMax
        {
            axis = 3;
            ingredientValue = currentGrid[y+1, x].index;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Very basic display for the grid, using materials on cubes
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
                spawned.gameObject.name = $"{x}, {y}, type = {currentGrid[y, x].index}";
            }
        }
        Debug.Log("Generated grid display!");
    }

    /// <summary>
    /// Readjust weights to have a different total, while keeping their ratio
    /// </summary>
    static float[] ReadjustWeights(float[] original, float newTotal)
    {
        float oldTotal = MathTools.ArrayTotal(original);
        float[] result = original.ToArray();
        for (int i = 0; i < original.Length; i++)
        {
            result[i] = result[i] / oldTotal * newTotal;
        }

        return result;
    }
}
