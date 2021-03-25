using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class grappleGun : MonoBehaviour
{
    //private LineRenderer lr;
    public bool grappling = false;
    private int grappleRange = 15;
    private float grappleDelay = 0.15f;
    
    [SerializeField]
    private GameObject gunTip;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Rigidbody playerRB;
    [SerializeField]
    private Camera cam;
    private SpringJoint spring;

    [SerializeField]
    private Text grapplingText;

    private Rigidbody grapplePoint;

    //distance formula variables
    private float deltaX;
    private float deltaY;
    private float deltaZ;
    //distance formula result
    private float deviation;
    //equilibrium point for grappling hook's spring.
    private float equiDistance;
    //force applied by 'spring' to player
    private float springForce;
    private Vector3 springDirection;

    void Awake()
    {
        //lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //toggle grapple
        if(Input.GetAxisRaw("Fire3") != 0)
        {
            if (grappling)
            {
                //require some delay to ungrapple so spamming the button doesn't ruin the grapple attempt
                if (grappleDelay < 0)
                {
                    StopGrapple();
                }
            }
            else
            {
                StartGrapple();
            }
        }

        if (grappling)
        {
            if (grappleDelay > -.5f)
            {
                grappleDelay -= Time.deltaTime;
            }

            deltaX = player.transform.position.x - grapplePoint.transform.position.x;
            deltaY = player.transform.position.y - grapplePoint.transform.position.y;
            deltaZ = player.transform.position.z - grapplePoint.transform.position.z;
            //calculate distance between player and grappling point
            deviation = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
            if (deviation - equiDistance > 0 )
            {
                springForce = 9.81f;
                springDirection = grapplePoint.transform.position - player.transform.position;
                Vector3.Normalize(springDirection);
                springDirection *= springForce;
                playerRB.AddForce(springDirection);
                print(grappleDelay);
            }            
        }
    }
    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit , grappleRange,LayerMask.GetMask("grapple")))
        {
            print("raycast hit");
            grappling = true;
            grapplingText.text = "Grappling: " + grappling.ToString();
            grappleDelay = 0.5f;
            /*
            player.AddComponent<SpringJoint>();
            spring = player.GetComponent<SpringJoint>();
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedAnchor = gunTip.transform.position;
            spring.connectedBody = hit.rigidbody;
            spring.maxDistance = 5;
            spring.minDistance = 3;
            */

            grapplePoint = hit.rigidbody;
            deltaX = player.transform.position.x - grapplePoint.transform.position.x;
            deltaY = player.transform.position.y - grapplePoint.transform.position.y;
            deltaZ = player.transform.position.z - grapplePoint.transform.position.z;
            //calculate distance between player and grappling point for equilibrium point
            equiDistance = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }
    }
    void StopGrapple()
    {
        grappling = false;
        grapplingText.text = "Grappling: " + grappling.ToString();
        grapplePoint = null;
        springForce = 0;
        springDirection = Vector3.zero;
    }
}
