using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSourceScript : MonoBehaviour
{
    private Vector3 raycastLine;

    private PlayerController playerController;
    private GameObject player;

    [SerializeField]
    private GameObject projectileObject;
    private ProjectileScript projectileScript;
    [SerializeField]
    private int projectileSpeed;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        deployToy();
    }

    public void deployToy()
    {

        raycastLine = transform.forward;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, raycastLine, out hit, 1.0f))
        {
            if (hit.collider.tag == "Player")
            {
                playerController.Loss();
            }
            else
            {
                ProjectileFire();
            }
        }

        //No terrain too close. Can place block.
        else
        {
            ProjectileFire();
        }
        Invoke("deployToy", 1.0f);
    }
    private void ProjectileFire()
    {
        raycastLine = transform.forward;

        GameObject _projectileObject = Instantiate(projectileObject, transform.position + raycastLine, Quaternion.identity); // local variables start w/ and underscore

        projectileScript = _projectileObject.GetComponent<ProjectileScript>();
        projectileScript.Launch(raycastLine, projectileSpeed);
    }
}
