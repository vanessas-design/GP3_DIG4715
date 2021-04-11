using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlocks : MonoBehaviour
{

    [SerializeField]
    public float speed;

    [SerializeField]
    Transform startPoint, endPoint;

    [SerializeField]
    float changeDirectionDelay;


    private Transform destinationTarget, departTarget;

    private float startTime;

    private float journeyLength;

    public bool isWaiting;

    private Rigidbody rb;

    public Vector3 direction;



    void Start()
    {
        departTarget = startPoint;
        destinationTarget = endPoint;

        startTime = Time.time;
        journeyLength = Vector3.Distance(departTarget.position, destinationTarget.position);
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {


        if (!isWaiting)
        {
            if (Vector3.Distance(transform.position, destinationTarget.position) > 0.01f)
            {
                float distCovered = (Time.time - startTime) * speed;

                float fractionOfJourney = distCovered / journeyLength;

                direction = new Vector3(destinationTarget.position.x - transform.position.x, destinationTarget.position.y - transform.position.y, destinationTarget.position.z - transform.position.z);
                direction.Normalize();
                rb.MovePosition(rb.position + direction * Time.deltaTime * speed);
            }
            else
            {
                isWaiting = true;
                StartCoroutine(changeDelay());
            }
        }


    }

    void ChangeDestination()
    {

        if (departTarget == endPoint && destinationTarget == startPoint)
        {
            departTarget = startPoint;
            destinationTarget = endPoint;
        }
        else
        {
            departTarget = endPoint;
            destinationTarget = startPoint;
        }

    }
    IEnumerator changeDelay()
    {
        direction = Vector3.zero;
        yield return new WaitForSeconds(changeDirectionDelay);
        ChangeDestination();
        startTime = Time.time;
        journeyLength = Vector3.Distance(departTarget.position, destinationTarget.position);
        isWaiting = false;
    }
}