using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grappleGun : MonoBehaviour
{
    private LineRenderer lr;
    private bool grappling = false;
    private int grappleRange = 50;
    
    [SerializeField]
    private GameObject gunTip;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Camera cam;
    private SpringJoint spring;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxisRaw("Fire3") != 0)
        {
            if (grappling)
            {
                StopGrapple();
            }
            else
            {
                StartGrapple();
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
            player.AddComponent<SpringJoint>();
            spring = player.GetComponent<SpringJoint>();
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedAnchor = gunTip.transform.position;
            spring.connectedBody = hit.rigidbody;
            spring.maxDistance = 10;
            spring.minDistance = 3;
        }
    }
    void StopGrapple()
    {
        grappling = false;
        Destroy(player.GetComponent<SpringJoint>());
    }
}
