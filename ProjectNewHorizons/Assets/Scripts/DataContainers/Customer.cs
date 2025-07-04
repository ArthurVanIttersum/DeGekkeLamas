using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public Order thisCustomersOrder;
    public Vector3 walkingAwayDirectionAndSpeed;
    bool walkingAway = false;
    public float walkingTime;
    [HideInInspector] public int index;
    [HideInInspector] public int positionIndex;
    private GameObject popup;
    [HideInInspector] public bool isWalkingAway = false;

    private void Start()
    {
        this.transform.eulerAngles = Camera.main.transform.eulerAngles;
        StartCoroutine(WalkIntoSceneAnimation());
    }

    public IEnumerator WalkingAwayAnimation()
    {
        DishManager.instance.dishesDone++;
        DishManager.instance.UpdateDishText();
        CustomerGenerator.instance.onOrderGiven.Invoke();

        if (DishManager.instance.dishesDone == DishManager.instance.dishesRequired) DishManager.instance.WinGame();
        walkingAway = true;
        popup.transform.GetChild(1).GetComponent<Image>().sprite = CustomerGenerator.instance.satisfiedSprite;
        MatchGridSystem.instance.ingredientLisText.text = string.Empty;
        for (int i = MatchGridSystem.instance.iconsSpawned.Count; i > 0; i--)
        {
            Destroy(MatchGridSystem.instance.iconsSpawned[i-1]);
            MatchGridSystem.instance.iconsSpawned.RemoveAt(i-1);
        }
        yield return new WaitForSeconds(walkingTime);
        CustomerGenerator.instance.possiblePositions.Add(positionIndex);
        DishManager.instance.DespawnAndRespawnCustomer(this.gameObject, index);
    }
    public IEnumerator WalkIntoSceneAnimation()
    {
        Vector3 oriPos = transform.position;
        transform.position += walkingAwayDirectionAndSpeed * 100;

        for (int i = 0; i < 100; i++)
        {
            transform.position -= walkingAwayDirectionAndSpeed;
            yield return new WaitForFixedUpdate();
        }
        popup = CreatePopUp();
        SetPopupToDefault();
        yield return new();
        
    }

    private void FixedUpdate()
    {
        if (walkingAway)
        {
            transform.position = transform.position + walkingAwayDirectionAndSpeed;
        }
    }

    public GameObject CreatePopUp()
    {
        CustomerGenerator CG = CustomerGenerator.instance;
        Quaternion rotation = transform.rotation;
        GameObject newPopup = Instantiate(CG.speechbubblePrefab, transform.position + Vector3.up, transform.rotation, transform);
        
        return newPopup;
    }

    public IEnumerator SetPopupToSpeachAndBack()
    {
        CustomerGenerator.instance.onOrderTaken.Invoke();
        popup.transform.GetChild(0).GetComponent<Image>().sprite = CustomerGenerator.instance.speechBubbleSprite;
        yield return new WaitForSeconds(5);
        popup.transform.GetChild(0).GetComponent<Image>().sprite = CustomerGenerator.instance.thoughtBubbleSprite;
        popup.transform.GetChild(1).GetComponent<Image>().sprite = CustomerGenerator.instance.waitingSprite;
    }

    public void SetPopupToDefault()
    {
        popup.transform.GetChild(0).GetComponent<Image>().sprite = CustomerGenerator.instance.thoughtBubbleSprite;
        popup.transform.GetChild(1).GetComponent<Image>().sprite = thisCustomersOrder.dishes.First().dishType.spriteForPopup;
    }

}
