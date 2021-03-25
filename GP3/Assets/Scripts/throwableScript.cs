using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwableScript : MonoBehaviour
{
    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }



    public void Launch(Vector3 direction, float force)
    {
        rigidbody.AddForce(direction * force);
    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.collider.gameObject.tag == "Ground")
        {
            Destroy(GetComponent<Rigidbody>());
        }
        else if(col.collider.gameObject.layer == 8)
        {
            Destroy(GetComponent<Rigidbody>());
        }
        else if(col.collider.gameObject.layer == 9)
        {
            Destroy(GetComponent<Rigidbody>());
        }
    }

    private void selfDestruct()
    {
        Destroy(gameObject);
    }
}
