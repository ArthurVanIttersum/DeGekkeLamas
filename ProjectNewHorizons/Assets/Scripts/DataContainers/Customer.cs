using System.Collections;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public Order thisCustomersOrder;
    public Vector3 walkingAwayDirectionAndSpeed;
    bool walkingAway = false;
    public float walkingTime;
    [HideInInspector] public int index;

    private void Start()
    {
        this.transform.eulerAngles = Camera.main.transform.eulerAngles;
        StartCoroutine(WalkIntoSceneAnimation());
    }

    public IEnumerator WalkingAwayAnimation()
    {
        print("startWalkingAway");
        walkingAway = true;
        yield return new WaitForSeconds(walkingTime);
        DishManager.instance.DespawnAndRespawnCustomer();
    }
    public IEnumerator WalkIntoSceneAnimation()
    {
        Vector3 oriPos = transform.position;
        transform.position += walkingAwayDirectionAndSpeed * 100;

        for (int i = 0; i < 100; i++)
        {
            transform.position -= walkingAwayDirectionAndSpeed;
            yield return null;
        }

        yield return new();
    }

    private void Update()
    {
        if (walkingAway)
        {
            transform.position = transform.position + walkingAwayDirectionAndSpeed;
        }
    }
}
