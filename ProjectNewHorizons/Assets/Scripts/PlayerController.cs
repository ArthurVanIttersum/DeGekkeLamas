using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    public MatchGridSystem matchGridSystem;
    public float interactDistance = 6;

    /// <summary>
    /// Use this variable to lock player moment and interactions when in match3 minigame
    /// </summary>
    [HideInInspector] public bool movementLocked;
    private void OnValidate()
    {
        DebugExtension.DebugWireSphere(this.transform.position, Color.red, interactDistance, 5);
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (movementLocked || GridActivator.isPlayingMatch3) return;

        // Touchscreen is also considered mouse button 0
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from camera position to mouse pos
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out RaycastHit info, 100, ~0, QueryTriggerInteraction.Ignore))
            {
                // Interact instead of setting destination if interactions
                if (info.collider.gameObject.CompareTag("Interactible") 
                    && Vector3.Distance(transform.position, info.transform.position) < interactDistance)
                {
                    // Get order from customer
                    if (info.collider.gameObject.TryGetComponent(out Customer customer))
                    {
                        if (customer.thisCustomersOrder.orderComplete)
                        {
                            print("startWalkingAway Animation Script");
                            StartCoroutine(customer.WalkingAwayAnimation());
                        }
                        else
                        {
                            StartCoroutine(customer.SetPopupToSpeachAndBack());
                            List<Dish> dishes = customer.thisCustomersOrder.dishes;
                            DishManager.instance.SetDish(dishes[0], (info.collider.gameObject, customer.index));
                            Debug.Log("Received order from customer");
                        }
                    }
                    // Open oven for match3 minigame
                    else if (info.collider.gameObject.TryGetComponent(out GridActivator activator) 
                        && activator.stationType == DishManager.instance.currentDish.dishType.dishType)
                    {
                        activator.ToggleGame();
                        print("Clicked on grid activator");
                    }
                    // Old unused UI version of grid
                    else if (info.collider.gameObject.TryGetComponent(out GridUIRenderer renderer))
                    {
                        print("Clicked on grid UI generator");
                        renderer.GenerateUI();
                    }
                }
                else agent.SetDestination(info.point);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (movementLocked || GridActivator.isPlayingMatch3) return;

        print($"Player collided with {other.name}");
        // Open oven for match3 minigame
        if (other.gameObject.TryGetComponent(out GridActivator activator)
            && activator.stationType == DishManager.instance.currentDish.dishType.dishType)
        {
            activator.ToggleGame();
            print("Clicked on grid activator");
        }
    }
}
