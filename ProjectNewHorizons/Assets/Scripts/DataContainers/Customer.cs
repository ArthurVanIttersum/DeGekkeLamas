using System.Collections;
using System.Linq;
using UnityEngine;
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

    private void Start()
    {
        this.transform.eulerAngles = Camera.main.transform.eulerAngles;
        StartCoroutine(WalkIntoSceneAnimation());
    }

    public IEnumerator WalkingAwayAnimation()
    {
        walkingAway = true;
        popup.transform.GetChild(1).GetComponent<Image>().sprite = CustomerGenerator.instance.satisfiedSprite;
        MatchGridSystem.instance.ingredientList.text = string.Empty;
        yield return new WaitForSeconds(walkingTime);
        CustomerGenerator.instance.possiblePositions.Add(positionIndex);
        if (DishManager.instance.dishesDone == DishManager.instance.dishesRequired) DishManager.instance.WinGame();
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
        GameObject newPopup = Instantiate(CG.speechbubblePrefab, transform.position + Vector3.up, transform.rotation, transform);
        
        newPopup.transform.GetChild(0).GetComponent<Image>().sprite = CustomerGenerator.instance.thoughtBubbleSprite;
        newPopup.transform.GetChild(1).GetComponent<Image>().sprite = thisCustomersOrder.dishes.First().dishType.spriteForPopup;

        return newPopup;
    }

    public IEnumerator SetPopupToSpeachAndBack()
    {
        print("switch to speach");
        popup.transform.GetChild(0).GetComponent<Image>().sprite = CustomerGenerator.instance.speechBubbleSprite;
        yield return new WaitForSeconds(1);
        popup.transform.GetChild(0).GetComponent<Image>().sprite = CustomerGenerator.instance.thoughtBubbleSprite;
        popup.transform.GetChild(1).GetComponent<Image>().sprite = CustomerGenerator.instance.waitingSprite;
        print("switch back to thought");
    }
}
