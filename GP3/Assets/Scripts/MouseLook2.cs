using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook2 : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    public Transform playerBody;

    private float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //hiding and locking mouse cursor
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        //setting the rotation of the player camera on the x and y rotational axis based on mouse input
        // Time.deltaTime makes it so that the rotation is not frame dependant
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //set to -= because += would be the same as inverting the Y-axis
        xRotation -= mouseY;

        //clamiping rotation, preventing you from looking too far up or too far down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

    }
}
