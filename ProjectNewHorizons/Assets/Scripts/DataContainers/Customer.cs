using System.Collections;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public Order thisCustomersOrder;
    public Vector3 walkingAwayDirectionAndSpeed;
    public bool walkingAway = false;
    public float walkingTime;
    [HideInInspector] public int index;

    private void Start()
    {
        this.transform.eulerAngles = Camera.main.transform.eulerAngles;
    }

    public IEnumerator WalkingAwayAnimation()
    {
        print("startWalkingAway");
        walkingAway = true;
        yield return new WaitForSeconds(walkingTime);
        DishManager.instance.DespawnAndRespawnCustomer();
    }

    private void Update()
    {
        if (walkingAway)
        {
            transform.position = transform.position + walkingAwayDirectionAndSpeed;
        }
    }
}
