using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MatchGridSystem : MonoBehaviour
{
    public Ingredient[] ingredientTypes;
    public Vector2Int gridDimensions;
    Ingredient[,] currentGrid;
    public int seed;

    private System.Random rand;
    [SerializeField, HideInInspector] Transform gridContainer;

    [Header("Debug stuff")]
    public MeshRenderer debugCube;
    private void OnValidate()
    {
        EditorApplication.delayCall += () => _onValidate();
    }
    /// <summary>
    /// this feels like a crappy solution but i cant call destroy in onvalidate
    /// </summary>
    void _onValidate()
    {
        for (int i = 0; i < ingredientTypes.Length; i++) ingredientTypes[i].index = i;
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

        Initialize();
        GenerateGrid();
        GenerateDisplay();
    }
    private void Start()
    {
        Initialize();
        GenerateGrid();
        GenerateDisplay();
    }

    void Initialize()
    {
        currentGrid = new Ingredient[gridDimensions.y, gridDimensions.x];
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
            }
        }
        Debug.Log("Generated grid display!");
    }
}
