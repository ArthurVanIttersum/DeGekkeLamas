using System.Collections.Generic;
using System.Linq;
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
    private Vector2Int[] alldirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
    
    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction*1000f, Color.red, 50f);
            Debug.Log(Input.mousePosition);
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
        

        if (direction.magnitude > 0.05f) // Threshold to ensure it’s a valid swipe
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
                print("foundAMatch");
            }
            if (TestMatch3(startingPos + directionVector, Vector2Int.zero - directionVector))
            {
                foundAMatch= true;
                print("foundAMatch");
            }
            if (foundAMatch)
            {//switching
                print("switching");
                Vector2Int selectedPos = startingPos;
                Vector2Int sideEffectPos = startingPos + directionVector;

                print(selectedPos);
                print(sideEffectPos);

                Ingredient selected = grid.currentGrid[startingPos.y,startingPos.x];
                Ingredient sideEffect = grid.currentGrid[sideEffectPos.y, sideEffectPos.x];
                grid.currentGrid[startingPos.y, startingPos.x] = sideEffect;
                grid.currentGrid[sideEffectPos.y, sideEffectPos.x] = selected;

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
        }
    }


    public bool TestMatch3(Vector2Int fromGridPos, Vector2Int direction)
    {
        print(fromGridPos);
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
                print("found a match, type1");
                
                ScoreManager.instance.IncreaseScore(TestOverkill(fromGridPos, directionsToTest[i], ingredientToMatch));//match
                currentDish.AddIngredient(ingredientToMatch);
                foundAMatch = true;
                
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
                    print("found a match, type2");
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
            bonuspoints += 500;
        }
        if (SampleGrid(testPos, ingredientToMatch))
        {
            bonuspoints += 1000;
        }
        return bonuspoints;
    }

    private bool SampleGrid(Vector2Int position, Ingredient typeToTest)
    {
        if (position.x < 0)
        {
            return false;
        }
        if (position.x > grid.gridDimensions.x - 1)
        {
            return false;
        }
        if (position.y < 0)
        {
            return false;
        }
        if (position.y > grid.gridDimensions.y - 1)
        {
            return false;
        }
        

        return grid.currentGrid[position.y, position.x].IndexEquals(typeToTest);
    }
}
