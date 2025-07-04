using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    public MatchGridSystem matchGridSystem;
    public float interactDistance = 6;
    public bool isServingCustomer = false;
    public Customer previousCustomer;

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
                if (info.collider.gameObject.CompareTag("Interactible") )
                {
                    // Get order from customer
                    if (info.collider.gameObject.TryGetComponent(out Customer customer))
                    {
                        agent.SetDestination(info.point);
                        if (Vector3.Distance(transform.position, info.transform.position) < interactDistance)
                        {
                            // Complete order
                            if (customer.thisCustomersOrder.orderComplete)
                            {
                                if (!customer.isWalkingAway)
                                {
                                    customer.isWalkingAway = true;
                                    print("startWalkingAway Animation Script");
                                    StartCoroutine(customer.WalkingAwayAnimation());
                                    isServingCustomer = false;
                                }
                            }
                            else // take order
                            {
                                // refuse if already has an order
                                if (!isServingCustomer)
                                {
                                    CustomerGenerator.firstOrderReceived = true;
                                    CustomerGenerator.instance.RemoveHighLights();
                                    StationHighlighter.instance.RemoveHighlight();
                                    previousCustomer = customer;
                                    StartCoroutine(customer.SetPopupToSpeachAndBack());
                                    List<Dish> dishes = customer.thisCustomersOrder.dishes;
                                    DishManager.instance.SetDish(dishes[0], (info.collider.gameObject, customer.index));
                                    Debug.Log("Received order from customer");
                                    isServingCustomer = true;
                                }
                            }
                        }
                    }
                    if (Vector3.Distance(transform.position, info.transform.position) < interactDistance)
                    {
                    // Open oven for match3 minigame
                    if (info.collider.gameObject.TryGetComponent(out GridActivator activator)
                        && activator.stationType == DishManager.instance.currentDish.dishType.dishType)
                        {
                            activator.ToggleGame();
                            print("Clicked on grid activator");
                        }
                    }
                }
                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    agent.SetDestination(info.point);
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

    public void ToggleInteraction()
    {
        movementLocked = !movementLocked;
        MatchingDetection.isPaused = !MatchingDetection.isPaused;
    }
}
