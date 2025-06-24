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
    private Vector2Int[] alldirections = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

    private Vector3 swipeDifference;

    private HashSet<Vector2Int> allBlocksToBeDeleted = new();

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

            swipeDifference = endScreenPos - startScreenPos;

            if (swipeDifference.magnitude > 0.1f) // Threshold to ensure it's a valid swipe
            {
                SwipeDetected();
            }
        }
    }
    //this is the main method. here you see all the steps in the algorithm.
    //each step might use helper methods, but it will always return to this methed before moving on to another major step


    private void SwipeDetected()//this is the main method. here you see all the steps in the algorithm. each step might use helper methods, but it will always return to thi methed before moving on to another maor step
    {
        Vector2Int swipeDirection = CalculateSwipeDirection();
        Vector2Int swipeStartPosition = GridStartPos;
        Vector2Int swipeDestination = swipeStartPosition + swipeDirection;

        if (TestIfOutOfBounds(swipeStartPosition))
        {
            return;
        }
        if (TestIfOutOfBounds(swipeDestination))
        {
            return;
        }
        SwitchBlocks(swipeStartPosition, swipeDestination);
        bool testAtStart = TestMatch3AtPosition(swipeStartPosition);
        bool testAtEnd = TestMatch3AtPosition(swipeDestination);
        if (!(testAtStart || testAtEnd))
        {
            SwitchBlocks(swipeStartPosition, swipeDestination);//switching the blocks back from where they came since no match was found
            return;
        }
        CollectMatchData3AtPosition(swipeStartPosition);
        CollectMatchData3AtPosition(swipeDestination);
        //just for testing
        Vector2Int[] toDelete = allBlocksToBeDeleted.ToArray();
        for (int i = 0; i < toDelete.Length; i++)
        {
            ReplaceBlock(toDelete[i]);
        }
        allBlocksToBeDeleted.Clear();
    }

    private Vector2Int CalculateSwipeDirection()
    {
        Vector2Int directionVector;

        if (Mathf.Abs(swipeDifference.x) > Mathf.Abs(swipeDifference.y))
        {
            if (swipeDifference.x > 0)
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
            if (swipeDifference.y > 0)
            {
                directionVector = Vector2Int.up;
            }
            else
            {
                directionVector = Vector2Int.down;
            }
        }
        return directionVector;
    }



    private bool SampleGrid(Vector2Int position, Ingredient typeToTest)
    {
        if (TestIfOutOfBounds(position))
        {
            return false;
        }

        return grid.currentGrid[position.y, position.x].IndexEquals(typeToTest);
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

    public void SwitchBlocks(Vector2Int gridPos1, Vector2Int gridPos2)
    {
        //switching ingredients
        Ingredient selected = SampleGrid(gridPos1);
        Ingredient sideEffect = SampleGrid(gridPos2);

        grid.currentGrid[gridPos1.y, gridPos1.x] = sideEffect;
        grid.currentGrid[gridPos2.y, gridPos2.x] = selected;

        //switching positions
        GameObject selectedObject = selected.cubeForDisplay;
        GameObject sideEffectObject = sideEffect.cubeForDisplay;

        Vector3 selectedPos3 = selectedObject.transform.position;
        Vector3 sideEffectPos3 = sideEffectObject.transform.position;

        selectedObject.transform.position = sideEffectPos3;
        sideEffectObject.transform.position = selectedPos3;

        //switching indexes
        Vector2 selectedIndexData = selectedObject.GetComponent<GridPosition>().index;
        Vector2 SideEffectIndexData = sideEffectObject.GetComponent<GridPosition>().index;

        selectedObject.GetComponent<GridPosition>().index = SideEffectIndexData;
        sideEffectObject.GetComponent<GridPosition>().index = selectedIndexData;
    }

    public bool TestMatch3AtPosition(Vector2Int testPosition)
    {
        //basic setup
        Ingredient ingredientToMatch = SampleGrid(testPosition);
        Vector2Int testpos;
        bool foundAMatch = false;
        bool found1, found2;
        //test for a match 3 assuming the testposition is on the edge of the 3
        for (int i = 0; i < 4; i++)
        {
            testpos = testPosition + alldirections[i];
            found1 = SampleAndTestGrid(testpos, ingredientToMatch);
            testpos = testPosition + alldirections[i] * 2;
            found2 = SampleAndTestGrid(testpos, ingredientToMatch);
            if (found1 && found2)
            {
                foundAMatch = true;
            }
        }

        //test for a match 3 assuming the testposition is in the middle of the 3
        //horizontal
        testpos = testPosition + alldirections[0];
        found1 = SampleAndTestGrid(testpos, ingredientToMatch);
        testpos = testPosition + alldirections[1];
        found2 = SampleAndTestGrid(testpos, ingredientToMatch);
        if (found1 && found2) { foundAMatch = true; }

        //vertical
        testpos = testPosition + alldirections[2];
        found1 = SampleAndTestGrid(testpos, ingredientToMatch);
        testpos = testPosition + alldirections[3];
        found2 = SampleAndTestGrid(testpos, ingredientToMatch);
        if (found1 && found2) { foundAMatch = true; }

        return foundAMatch;
    }
    private bool SampleAndTestGrid(Vector2Int position, Ingredient typeToTest)
    {
        if (TestIfOutOfBounds(position))
        {
            return false;
        }

        return grid.currentGrid[position.y, position.x].IndexEquals(typeToTest);
    }

    private Ingredient SampleGrid(Vector2Int position)
    {
        if (TestIfOutOfBounds(position))
        {
            return new Ingredient();
        }

        return grid.currentGrid[position.y, position.x];
    }

    public void CollectMatchData3AtPosition(Vector2Int testPosition)
    {
        //basic setup
        Ingredient ingredientToMatch = SampleGrid(testPosition);
        Vector2Int testpos;
        bool found1, found2;
        //test for a match 3 assuming the testposition is on the edge of the 3
        for (int i = 0; i < 4; i++)
        {
            testpos = testPosition + alldirections[i];
            found1 = SampleAndTestGrid(testpos, ingredientToMatch);
            testpos = testPosition + alldirections[i] * 2;
            found2 = SampleAndTestGrid(testpos, ingredientToMatch);
            if (found1 && found2)
            {
                allBlocksToBeDeleted.Add(testPosition);
                allBlocksToBeDeleted.Add(testPosition + alldirections[i]);
                allBlocksToBeDeleted.Add(testPosition + alldirections[i] * 2);
            }
        }

        //test for a match 3 assuming the testposition is in the middle of the 3
        //horizontal
        testpos = testPosition + alldirections[0];
        found1 = SampleAndTestGrid(testpos, ingredientToMatch);
        testpos = testPosition + alldirections[1];
        found2 = SampleAndTestGrid(testpos, ingredientToMatch);
        if (found1 && found2)
        {
            allBlocksToBeDeleted.Add(testPosition);
            allBlocksToBeDeleted.Add(testPosition + alldirections[0]);
            allBlocksToBeDeleted.Add(testPosition + alldirections[1]);
        }

        //vertical
        testpos = testPosition + alldirections[2];
        found1 = SampleAndTestGrid(testpos, ingredientToMatch);
        testpos = testPosition + alldirections[3];
        found2 = SampleAndTestGrid(testpos, ingredientToMatch);
        if (found1 && found2)
        {
            allBlocksToBeDeleted.Add(testPosition);
            allBlocksToBeDeleted.Add(testPosition + alldirections[2]);
            allBlocksToBeDeleted.Add(testPosition + alldirections[3]);
        }
    }

    public void ReplaceBlock(Vector2Int gridPosition)
    {
        for (int i = 0; i < grid.ingredientTypes.Length; i++)
        {
            ReplaceBlockTo(gridPosition, grid.ingredientTypes[i]);
            if (!TestMatch3AtPosition(gridPosition))
            {
                print("found a good option");
                return;
            }
        }
        print("didn't find a good option");
    }

    public void ReplaceBlockTo(Vector2Int gridPosition, Ingredient changeTo)
    {
        Ingredient toReplace = SampleGrid(gridPosition);
        GameObject blockToReplace = toReplace.cubeForDisplay.gameObject;
        Vector3 replacePosition = blockToReplace.transform.position;
        Ingredient ingredientToSpawn = grid.ingredientTypes.First();//first one by default
        for (int i = 0; i < grid.ingredientTypes.Length; i++)
        {
            if (grid.ingredientTypes[i].index == changeTo.index)
            {
                ingredientToSpawn = grid.ingredientTypes[i];
            }
        }
        grid.currentGrid[gridPosition.y, gridPosition.x] = ingredientToSpawn;

        //spawn cube
        var spawned = Instantiate(grid.debugCube, replacePosition, Quaternion.identity, grid.gridContainer);
        spawned.sharedMaterial = grid.currentGrid[gridPosition.y, gridPosition.x].material;
        spawned.gameObject.name = $"{gridPosition.x}, {gridPosition.y}, type = {grid.currentGrid[gridPosition.y, gridPosition.x].index}";
        spawned.gameObject.GetOrAddComponent<GridPosition>().index = new(gridPosition.x, gridPosition.y);
        grid.currentGrid[gridPosition.y, gridPosition.x].cubeForDisplay = spawned.gameObject;
        
        Destroy(blockToReplace);
    }

}