
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MatchingDetection : MonoBehaviour
{
    public Dish currentDish;
    public Camera mainCamera; // Assign the main camera
    public MatchGridSystem grid;
    private Vector2Int gridStartPos = Vector2Int.left;
    private Vector3 startScreenPos;
    private Vector3 endScreenPos;
    private bool swiping = false;
    private Vector2Int[] alldirections = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

    private Vector3 swipeDifference;

    private HashSet<Vector2Int> allBlocksToBeReplaced = new();
    private HashSet<Vector2Int> allPositionsToCheck = new();
    private Dictionary<int, List<Vector2Int>> columnsOfBlocksToBeReplaced = new();
    private List<GameObject> theObjectsThatGotMovedUp = new();

    private List<Ingredient> foundIngredientTypes = new();
    private List<Ingredient> sentIngredientTypes = new();
    

    private bool swipingAnimationPlaying = false;

    public UnityEvent onMatchMade;

    [Header("Time durations")]
    private int numberOfCoroutinesRunning = 0;//for animations
    public float timeBeforeSwitchingBack = 0.5f;//before blocks switch back
    public float timeWhileBlocksVisible = 0.75f;//time that the blocks can be visible as being 3 in a row
    public float timeBeforeBlocksStartFalling = 0.25f;//time before the blocks start falling
    public float timeBetweenBlocksFalling = 0.15f;//time between blocks faling one space
    

    void LateUpdate()
    {
        if (!GridActivator.isPlayingMatch3) return;

        if (swipingAnimationPlaying) return;//so the player doesn't interact with the grid as it's moving.
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 50f);
            
            if (Physics.Raycast(ray, out RaycastHit info))
            {
                if (info.transform.TryGetComponent(out GridPosition gridPosition))
                {
                    Vector2 gridData = gridPosition.index;

                    gridStartPos.x = (int)gridData.x;
                    gridStartPos.y = (int)gridData.y;
                    startScreenPos = Input.mousePosition;
                    swiping = true;
                }
            }
        }
        else if (swiping && Input.GetMouseButtonUp(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.blue, 50f);
            if (Physics.Raycast(ray, out _))
            {
                endScreenPos = Input.mousePosition;

                swiping = false;

                swipeDifference = endScreenPos - startScreenPos;

                if (swipeDifference.magnitude > 0.1f) // Threshold to ensure it's a valid swipe
                {
                    startScreenPos = Vector3.left;
                    endScreenPos = Vector3.left;
                    swipingAnimationPlaying = true;
                    StartCoroutine(SwipeDetected());
                    return;
                }
                else
                {
                    gridStartPos = Vector2Int.left;
                    startScreenPos = Vector3.left;
                    endScreenPos = Vector3.left;
                }
            }
        }
    }

    /// <summary>
    /// this is the main method. here you see all the steps in the algorithm. 
    /// each step might use helper methods, but it will always return to this methed before moving on to another major step
    /// </summary>
    /// public IEnumerator SetPopupToSpeachAndBack()
    
    private IEnumerator SwipeDetected()
    {
        Vector2Int swipeDirection = CalculateSwipeDirection();
        Vector2Int swipeStartPosition = gridStartPos;
        gridStartPos = Vector2Int.left;
        Vector2Int swipeDestination = swipeStartPosition + swipeDirection;

        if (TestIfOutOfBounds(swipeStartPosition))
        {
            swipingAnimationPlaying = false;
            yield break;
            
        }
        if (TestIfOutOfBounds(swipeDestination))
        {
            swipingAnimationPlaying = false;
            yield break;
        }
        yield return StartCoroutine(SwitchBlocks(swipeStartPosition, swipeDestination));
        allPositionsToCheck.Add(swipeStartPosition);
        allPositionsToCheck.Add(swipeDestination);
        if (!CheckAllPositions())
        {
            yield return StartCoroutine(SwitchBlocks(swipeStartPosition, swipeDestination));
            allPositionsToCheck.Clear();
            yield return new WaitForSeconds(timeBeforeSwitchingBack);//time before the blocks switch back
            swipingAnimationPlaying = false;
            yield break;
        }
        for (int i = 0; i < 10; i++)//10 is a limiter, so that if it keeps looping it won't loop forever breaking the game.
        {
            if (!CheckAllPositions())
            {
                allPositionsToCheck.Clear();
                FinalizeIngredients();
                swipingAnimationPlaying = false;
                yield break;
            }
            CollectDataOfAllPositions();
            
            Vector2Int[] toReplace = allBlocksToBeReplaced.ToArray();
            CalculateIngredients(toReplace);
            columnsOfBlocksToBeReplaced = allBlocksToBeReplaced.GroupBy(point => point.x).ToDictionary(group => group.Key, group => group.ToList());
            yield return new WaitForSeconds(timeWhileBlocksVisible);//time that the blocks can be visible as being 3 in a row
            HideBlocks(toReplace);
            onMatchMade.Invoke();
            yield return new WaitForSeconds(timeBeforeBlocksStartFalling);//time before the blocks start falling
            for (int j = 0; j < toReplace.Length; j++)
            {
                Ingredient ingredientFound = SampleGrid(toReplace[j]);
                GameObject theCubeObject = ingredientFound.cubeForDisplay.gameObject;
                theObjectsThatGotMovedUp.Add(theCubeObject);
            }
            yield return StartCoroutine(MoveEverythingUp());
            
            for (int j = 0; j < theObjectsThatGotMovedUp.Count; j++)
            {
                Vector2 position = theObjectsThatGotMovedUp[j].GetComponent<GridPosition>().index;
                Vector2Int gridPosition = new Vector2Int((int)position.x, (int)position.y);
                ReplaceBlock(gridPosition);
            }
            
            theObjectsThatGotMovedUp.Clear();
            columnsOfBlocksToBeReplaced.Clear();
            allBlocksToBeReplaced.Clear();
        }
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

    public IEnumerator SwitchBlocks(Vector2Int gridPos1, Vector2Int gridPos2)
    {
        //collectingData
        Ingredient ingredient1 = SampleGrid(gridPos1);
        Ingredient ingredient2 = SampleGrid(gridPos2);

        GameObject gameobject1 = ingredient1.cubeForDisplay;
        GameObject gameobject2 = ingredient2.cubeForDisplay;

        Vector2 indexData1 = gameobject1.GetComponent<GridPosition>().index;
        Vector2 indexData2 = gameobject2.GetComponent<GridPosition>().index;

        Vector3 pos31 = GetWorldPosOfBlock(gridPos1);
        Vector3 pos32 = GetWorldPosOfBlock(gridPos2);

        //switching ingredients
        grid.currentGrid[gridPos1.y, gridPos1.x] = ingredient2;
        grid.currentGrid[gridPos2.y, gridPos2.x] = ingredient1;

        //switching indexes
        gameobject1.GetComponent<GridPosition>().index = indexData2;
        gameobject2.GetComponent<GridPosition>().index = indexData1;

        //switching positions
        gameobject1.GetComponent<GridPosition>().destinationPosition = pos32;
        gameobject2.GetComponent<GridPosition>().destinationPosition = pos31;

        Vector3 difference1 = pos32 - pos31;
        Vector3 difference2 = pos31 - pos32;

        for (int i = 0; i < 4; i++)
        {
            yield return null;
            gameobject1.transform.position += difference1 / 4;
            gameobject2.transform.position += difference2 / 4;
        }


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
                allBlocksToBeReplaced.Add(testPosition);
                allBlocksToBeReplaced.Add(testPosition + alldirections[i]);
                allBlocksToBeReplaced.Add(testPosition + alldirections[i] * 2);
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
            allBlocksToBeReplaced.Add(testPosition);
            allBlocksToBeReplaced.Add(testPosition + alldirections[0]);
            allBlocksToBeReplaced.Add(testPosition + alldirections[1]);
        }

        //vertical
        testpos = testPosition + alldirections[2];
        found1 = SampleAndTestGrid(testpos, ingredientToMatch);
        testpos = testPosition + alldirections[3];
        found2 = SampleAndTestGrid(testpos, ingredientToMatch);
        if (found1 && found2)
        {
            allBlocksToBeReplaced.Add(testPosition);
            allBlocksToBeReplaced.Add(testPosition + alldirections[2]);
            allBlocksToBeReplaced.Add(testPosition + alldirections[3]);
        }
    }

    public void ReplaceBlock(Vector2Int gridPosition)
    {
        List<Ingredient> posibleIngredients = new();
        for (int i = 0; i < grid.ingredientsUsed.Count; i++)
        {
            ReplaceBlockTo(gridPosition, grid.ingredientsUsed[i]);
            if (!TestMatch3AtPosition(gridPosition))
            {
                posibleIngredients.Add(grid.ingredientsUsed[i]);
            }
        }
        int randomOption = Random.Range(0,posibleIngredients.Count);
        GameObject newSpawnedBlock = ReplaceBlockTo(gridPosition, posibleIngredients[randomOption]);
        StartCoroutine(AnimateSpawned(newSpawnedBlock));
    }

    public GameObject ReplaceBlockTo(Vector2Int gridPosition, Ingredient changeTo)
    {
        Ingredient toReplace = SampleGrid(gridPosition);
        GameObject blockToReplace = toReplace.cubeForDisplay.gameObject;
        Vector3 replacePosition = GetWorldPosOfBlock(gridPosition);
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
        var spawned = Instantiate(grid.gridQuad, replacePosition, Quaternion.identity, grid.gridContainer);
        spawned.sharedMaterial = grid.currentGrid[gridPosition.y, gridPosition.x].material;
        spawned.gameObject.name = $"{gridPosition.x}, {gridPosition.y}, type = {grid.currentGrid[gridPosition.y, gridPosition.x].index}";
        spawned.gameObject.GetOrAddComponent<GridPosition>().index = new(gridPosition.x, gridPosition.y);
        spawned.gameObject.GetOrAddComponent<GridPosition>().destinationPosition = replacePosition;
        grid.currentGrid[gridPosition.y, gridPosition.x].cubeForDisplay = spawned.gameObject;
        
        Destroy(blockToReplace);
        return spawned.gameObject;
    }

    private IEnumerator AnimateSpawned(GameObject toAnimate)
    {
        for (int i = 1; i < 5; i++)
        {
            toAnimate.transform.localScale = Vector3.one * ((float)i / 4);
            yield return null;
        }
    }

    private IEnumerator MoveEverythingUp()
    {
        numberOfCoroutinesRunning = 0;
        for (int i = 0; i < grid.gridDimensions.y; i++)//for every column
        {
            if (columnsOfBlocksToBeReplaced.TryGetValue(i, out List<Vector2Int> columnList))
            {
                numberOfCoroutinesRunning++;
                StartCoroutine(MoveColumnUp(columnList, i));
            }
        }
        yield return new WaitUntil(() => numberOfCoroutinesRunning == 0);
    }

    private IEnumerator MoveColumnUp(List<Vector2Int> blocksInColumn, int column)
    {
        int numberOfBlocks = blocksInColumn.Count;
        for (int i = 0; i < numberOfBlocks; i++)
        {
            Vector2Int highest = FindHighestInColumn(column, blocksInColumn.ToArray());
            MoveOneBlockAllTheWayUp(highest);
            blocksInColumn.Remove(highest);
            if (!TestIfOutOfBounds(highest + Vector2Int.up))
            {
                yield return new WaitForSeconds(timeBetweenBlocksFalling);//time between blocks falling one space
            }
        }
        numberOfCoroutinesRunning--;
    }

    private void MoveOneBlockAllTheWayUp(Vector2Int gridPosition)
    {
        for (int i = 0; i < grid.gridDimensions.x; i++)
        {
            allPositionsToCheck.Add(gridPosition);
            if (TestIfOutOfBounds(gridPosition + Vector2Int.up))
            {
                return;
            }
            else
            {
                StartCoroutine(SwitchBlocks(gridPosition, gridPosition + Vector2Int.up));
            }
            gridPosition.y++;
        }
    }

    private Vector2Int FindHighestInColumn(int column, Vector2Int[] blocks)
    {
        int highest = 0;
        for (int j = 0; j < grid.gridDimensions.y; j++)
        {
            if (blocks.Contains(new Vector2Int(column, j)))
            {
                highest = j;
            }
        }
        return new Vector2Int(column, highest);
    }

    private bool CheckAllPositions()
    {
        Vector2Int[] allPositions = allPositionsToCheck.ToArray();
        for (int i = 0; i < allPositions.Length; i++)
        {
            if (TestMatch3AtPosition(allPositions[i]))
            {
                return true;
            }
        }
        return false;
    }

    private void CollectDataOfAllPositions()
    {
        Vector2Int[] allPositions = allPositionsToCheck.ToArray();
        for (int i = 0; i < allPositions.Length; i++)
        {
            CollectMatchData3AtPosition(allPositions[i]);
        }
    }

    private void CalculateIngredients(Vector2Int[] foundBlocks)
    {
        int scoreToAdd = foundBlocks.Length;
        for (int i = 0; i < foundBlocks.Length; i++)
        {
            Ingredient theIngredientFromTheGrid = SampleGrid(foundBlocks[i]);
            if (!Ingredient.ContainsIndex(foundIngredientTypes.ToArray(), theIngredientFromTheGrid))
            {
                foundIngredientTypes.Add(theIngredientFromTheGrid);
            }
        }
        for (int i = 0; i < foundIngredientTypes.Count; i++)
        {
            if (!Ingredient.ContainsIndex(sentIngredientTypes.ToArray(), foundIngredientTypes[i]))
            {
                currentDish.AddIngredient(foundIngredientTypes[i]);
                sentIngredientTypes.Add(foundIngredientTypes[i]);
                scoreToAdd -= 3;
            }
        }
        ScoreManager.instance.IncreaseScore(ScoreManager.instance.scoreIngredientCorrect * scoreToAdd);
    }

    private void FinalizeIngredients()
    {
        DishManager.instance.TestFinished();
        sentIngredientTypes.Clear();
        foundIngredientTypes.Clear();
    }

    private void HideBlocks(Vector2Int[] blocksToHide)
    {
        for (int i = 0; i < blocksToHide.Length; i++)
        {
            StartCoroutine(ShrinkBlock(SampleGrid(blocksToHide[i]).cubeForDisplay.gameObject));
        }
    }

    private IEnumerator ShrinkBlock(GameObject toAnimate)
    {
        for (int i = 4; i > 0; i--)
        {
            toAnimate.transform.localScale = Vector3.one * ((float)i / 4);
            yield return null;
        }
        toAnimate.GetComponent<MeshRenderer>().enabled = false;
    }

    private Vector3 GetWorldPosOfBlock(Vector2Int gridPos)
    {
        return SampleGrid(gridPos).cubeForDisplay.GetComponent<GridPosition>().destinationPosition;
    }

}