using System.Collections.Generic;

using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class MatchingDetection : MonoBehaviour
{
    public Dish currentDish;
    public Camera mainCamera; // Assign the main camera
    public MatchGridSystem grid;
    private Vector2Int GridStartPos;
    private Vector3 startScreenPos;
    private Vector3 endScreenPos;
    private bool swiping = false;
    private Vector2Int[] alldirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private List <Vector2Int> foundMatch3Here = new();
    void LateUpdate()
    {
        if (!GridActivator.isPlayingMatch3) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 50f);
            //Debug.Log(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit info))
            {
                Vector2 gridData = info.transform.GetComponent<GridPosition>().index;

                GridStartPos.x = (int)gridData.x;
                GridStartPos.y = (int)gridData.y;
                startScreenPos = Input.mousePosition;
            }
            swiping = true;
        }
        else if (swiping && Input.GetMouseButtonUp(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.blue, 50f);
            if (Physics.Raycast(ray, out RaycastHit info))
            {
                endScreenPos = Input.mousePosition;
            }
            swiping = false;
            SwipeDetected();
        }
    }

    void SwipeDetected()
    {
        Vector2 direction = endScreenPos - startScreenPos;
        Vector2Int directionVector;


        if (direction.magnitude > 0.05f) // Threshold to ensure it�s a valid swipe
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                {
                    directionVector = Vector2Int.right;
                }
                else
                {
                    directionVector = Vector2Int.left;
                }

            }
            else
            {
                if (direction.y > 0)
                {
                    directionVector = Vector2Int.up;
                }
                else
                {
                    directionVector = Vector2Int.down;
                }
            }

            Vector2Int startingPos = GridStartPos;

            bool foundAMatch = false;

            if (TestMatch3(startingPos, directionVector))
            {
                foundAMatch = true;
            }
            if (TestMatch3(startingPos + directionVector, -directionVector))
            {
                foundAMatch = true;

            }
            if (foundAMatch)
            {
                SwitchBlocks(startingPos, startingPos + directionVector);//switching

                //horizontal or vertical
                for (int i = 0; i < foundMatch3Here.Count / 3; i++)
                {
                    Vector2Int[] positions = new Vector2Int[3];
                    positions[0] = foundMatch3Here[i * 3];
                    positions[1] = foundMatch3Here[i * 3 + 1];
                    positions[2] = foundMatch3Here[i * 3 + 2];
                    
                    Vector2Int difference = positions[2] - positions[0];

                    if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
                    {
                        BlocksFallingHorizontal(positions);
                        for (int j = 0; j < 3; j++)
                        {
                            foundMatch3Here.Remove(positions[i]);
                        }
                    }
                    else
                    {
                        BlocksFallingVertical(positions);
                        for (int j = 0; j < 3; j++)
                        {
                            foundMatch3Here.Remove(positions[i]);
                        }
                    }
                }
                foundMatch3Here.Clear();
            }
        }
        GridStartPos = new();
        startScreenPos = new();
        endScreenPos = new();
    }


    public bool TestMatch3(Vector2Int fromGridPos, Vector2Int direction)
    {
        print(fromGridPos);

        // Stop execution if out of bounds
        if (TestIfOutOfBounds(fromGridPos)) return false;

        Ingredient ingredientToMatch = grid.currentGrid[fromGridPos.y, fromGridPos.x];
        Vector2Int newPosition = fromGridPos + direction;
        Vector2Int opositeDirection = Vector2Int.zero - direction;
        List<Vector2Int> directionsToTest = alldirections.ToList();
        directionsToTest.Remove(opositeDirection);
        Vector2Int testpos;
        bool foundAMatch = false;
        for (int i = 0; i < 3; i++)
        {
            testpos = newPosition + directionsToTest[i];
            bool found1 = SampleGrid(testpos, ingredientToMatch);
            testpos = newPosition + directionsToTest[i] * 2;
            bool found2 = SampleGrid(testpos, ingredientToMatch);
            if (found1 && found2)
            {


                ScoreManager.instance.IncreaseScore(TestOverkill(fromGridPos, directionsToTest[i], ingredientToMatch));//match
                grid.CollectIngredient(ingredientToMatch);
                currentDish.AddIngredient(ingredientToMatch);
                foundAMatch = true;
                //save positions here
                foundMatch3Here.Add(newPosition);
                foundMatch3Here.Add(newPosition + directionsToTest[i]);
                foundMatch3Here.Add(newPosition + directionsToTest[i] * 2);
            }
        }
        directionsToTest.Remove(direction);
        bool found = false;
        for (int i = 0; i < 2; i++)
        {
            testpos = newPosition + directionsToTest[i];
            if (SampleGrid(testpos, ingredientToMatch))
            {
                if (!found)
                {
                    found = true;
                }
                else
                {
                    found = false;
                    ScoreManager.instance.IncreaseScore(TestOverkill(fromGridPos, directionsToTest[i], ingredientToMatch));//match
                    grid.CollectIngredient(ingredientToMatch);
                    currentDish.AddIngredient(ingredientToMatch);//match
                    
                    foundAMatch = true;
                }
            }
        }
        return foundAMatch;
    }

    private int TestOverkill(Vector2Int fromGridPos, Vector2Int direction, Ingredient ingredientToMatch)
    {
        Vector2Int opositeDirection = Vector2Int.zero - direction;
        Vector2Int testPos = fromGridPos + opositeDirection;
        int bonuspoints = 0;
        if (SampleGrid(testPos, ingredientToMatch))
        {
            bonuspoints += ScoreManager.instance.score4InARow;
        }
        if (SampleGrid(testPos, ingredientToMatch))
        {
            bonuspoints += ScoreManager.instance.score5InARow;
        }
        return bonuspoints;
    }

    private bool SampleGrid(Vector2Int position, Ingredient typeToTest)
    {
        if (TestIfOutOfBounds(position))
        {
            return false;
        }
        
        return grid.currentGrid[position.y, position.x].IndexEquals(typeToTest);
    }

    public void BlocksFallingHorizontal(Vector2Int[] gridPositions)
    {
        for(int i = 0; i < 11; i++)
        {
            bool foundEnd = false;
            for (int j = 0; j < gridPositions.Length; j++)
            {
                if (TestIfOutOfBounds(gridPositions[j] + Vector2Int.up))
                {
                    
                    print(gridPositions);
                    ReplaceBlock(gridPositions[j]);

                    foundEnd = true;
                }
                else
                {
                    SwitchBlocks(gridPositions[j], gridPositions[j] + Vector2Int.up);
                }
                gridPositions[j].y++;
            }
            if (foundEnd)
            {
                break;
            }
        }
        

        
    }
    public void BlocksFallingVertical(Vector2Int[] gridPositions)
    {// work in progress
        SwitchBlocks(gridPositions[0], gridPositions[0] + Vector2Int.up);
        SwitchBlocks(gridPositions[1], gridPositions[1] + Vector2Int.up);
        SwitchBlocks(gridPositions[2], gridPositions[2] + Vector2Int.up);
    }

    public void SwitchBlocks(Vector2Int gridPos1, Vector2Int gridPos2)
    {
        Ingredient selected = grid.currentGrid[gridPos1.y, gridPos1.x];
        Ingredient sideEffect = grid.currentGrid[gridPos2.y, gridPos2.x];
        grid.currentGrid[gridPos1.y, gridPos1.x] = sideEffect;
        grid.currentGrid[gridPos2.y, gridPos2.x] = selected;

        GameObject selectedObject = selected.cubeForDisplay;
        GameObject sideEffectObject = sideEffect.cubeForDisplay;

        Vector3 selectedPos3;
        Vector3 sideEffectPos3;

        selectedPos3 = selectedObject.transform.position;

        sideEffectPos3 = sideEffectObject.transform.position;

        selectedObject.transform.position = sideEffectPos3;
        sideEffectObject.transform.position = selectedPos3;

        Vector2 selectedIndexData = selectedObject.GetComponent<GridPosition>().index;
        Vector2 SideEffectIndexData = sideEffectObject.GetComponent<GridPosition>().index;

        selectedObject.GetComponent<GridPosition>().index = SideEffectIndexData;
        sideEffectObject.GetComponent<GridPosition>().index = selectedIndexData;
    }

    public bool TestIfOutOfBounds(Vector2Int positionToTest)
    {
        if (positionToTest.x < 0)
        {
            return true;
        }
        if (positionToTest.x > grid.gridDimensions.x - 1)
        {
            return true;
        }
        if (positionToTest.y < 0)
        {
            return true;
        }
        if (positionToTest.y > grid.gridDimensions.y - 1)
        {
            return true;
        }
        return false;
    }

    public void ReplaceBlock(Vector2Int gridPosition)
    {
        //remove object in worldspace
        GameObject blockToReplace = grid.currentGrid[gridPosition.y, gridPosition.x].cubeForDisplay;
        Vector3 replacePosition = blockToReplace.transform.position;

        //make a new ingredient spawn at the top
        List<Ingredient> ingredientsThatCouldSpawn = grid.requiredIngredients.ToList();
        // Do not spawn type if this would create 3 in a row
        List<int> indexesToRemove = new();
        for (int i = 0; i < ingredientsThatCouldSpawn.Count; i++)
        {
            if (SampleGrid(gridPosition + Vector2Int.left, ingredientsThatCouldSpawn[i]) && SampleGrid(gridPosition + Vector2Int.left * 2, ingredientsThatCouldSpawn[i]))
            {
                indexesToRemove.Add(i);
            }
            else if (SampleGrid(gridPosition + Vector2Int.right, ingredientsThatCouldSpawn[i]) && SampleGrid(gridPosition + Vector2Int.right * 2, ingredientsThatCouldSpawn[i]))
            {
                indexesToRemove.Add(i);
            }
            else if (SampleGrid(gridPosition + Vector2Int.down, ingredientsThatCouldSpawn[i]) && SampleGrid(gridPosition + Vector2Int.down * 2, ingredientsThatCouldSpawn[i]))
            {
                indexesToRemove.Add(i);
            }
        }

        // Remove objects that shouldnt spawn, in correct order to avoid deleting the order one if indexes shifted
        indexesToRemove.Sort();
        indexesToRemove.Reverse();
        for (int k = 0; k < indexesToRemove.Count; k++)
        {
            int index = indexesToRemove[k];
            ingredientsThatCouldSpawn.RemoveAt(index);
        }
        Ingredient ingredientToSpawn = ingredientsThatCouldSpawn[Random.Range(0, ingredientsThatCouldSpawn.Count)];
        grid.currentGrid[gridPosition.y, gridPosition.x] = ingredientToSpawn;
        print("this is getting activated");
        //spawn cube


        var spawned = Instantiate(grid.debugCube, replacePosition, Quaternion.identity);//blockToReplace.GetComponentInParent<Transform>()
        spawned.sharedMaterial = grid.currentGrid[gridPosition.y, gridPosition.x].material;
        spawned.gameObject.name = $"{gridPosition.x}, {gridPosition.y}, type = {grid.currentGrid[gridPosition.y, gridPosition.x].index}";
        spawned.gameObject.GetOrAddComponent<GridPosition>().index = new(gridPosition.x, gridPosition.y);
        grid.currentGrid[gridPosition.y, gridPosition.x].cubeForDisplay = spawned.gameObject;
        print(spawned);
        print(spawned.transform.position);
        Destroy(blockToReplace);
    }
}
