using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    Rigidbody rigidbody;

    [SerializeField]
    private GameObject projectile;

    private PlayerController playerController;
    private GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }



    public void Launch(Vector3 direction, float force)
    {
        rigidbody.AddForce(direction * force);

        Invoke("SelfDestruct", 10.0f);
    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.collider.gameObject.tag == "Player")
        {
            playerController.Loss();
        }
        print("hit");
        Destroy(projectile);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
