using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableScript : MonoBehaviour
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
        Invoke("blockStop", .0625f);
    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.collider.gameObject.tag == "Ground")
        {
            blockStop();
        }
        else if(col.collider.gameObject.layer == 8)
        {
            blockStop();
        }
        else if(col.collider.gameObject.layer == 9)
        {
            blockStop();
        }
        else if(col.collider.gameObject.layer == 10)
        {
            blockStop();
        }
    }

    private void blockStop()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            rigidbody.velocity = Vector3.zero;
            Destroy(GetComponent<Rigidbody>());
        }
    }

    private void selfDestruct()
    {
        Destroy(gameObject);
    }
}
