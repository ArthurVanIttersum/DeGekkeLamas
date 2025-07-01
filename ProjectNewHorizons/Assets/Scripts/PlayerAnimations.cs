using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerAnimations : MonoBehaviour
{
    private NavMeshAgent agent;
    public Sprite[] walkforward;
    public Sprite[] walkbackward;
    public Sprite[] walkLeft;
    public Sprite[] walkRight;
    public Sprite standStill;
    private Image theImage;
    private Quaternion rotation;
    private void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        theImage = GetComponentInChildren<Image>();
        theImage.transform.eulerAngles = Camera.main.transform.eulerAngles;
        rotation = Camera.main.transform.rotation;
        rotation = Quaternion.Euler(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
    }
    // Update is called once per frame
    void Update()
    {
        int frame = Mathf.FloorToInt(Time.time * 10) % 4;
        transform.rotation = rotation;
        Vector3 velocity = agent.velocity;
        if (agent.velocity.magnitude > 0.1)
        {
            if (Mathf.Abs(velocity.z) > Mathf.Abs(velocity.x))
            {
                if (velocity.z > 0)
                {
                    //right
                    theImage.sprite = walkRight[frame];
                }
                else
                {
                    //left
                    theImage.sprite = walkLeft[frame];
                }
            }
            else
            {
                if (velocity.x > 0)
                {
                    //forward
                    theImage.sprite = walkforward[frame];
                }
                else
                {
                    //backward
                    theImage.sprite = walkbackward[frame];
                }
            }
        }
        else
        {
            //staning still
            theImage.sprite = standStill;
        }
    }
}
