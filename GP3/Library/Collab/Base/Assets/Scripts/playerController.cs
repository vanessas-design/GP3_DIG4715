using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [SerializeField]
    private float lookSensitivity = 3f;
    private Vector3 movement;
    private Vector3 rotation;
    private Vector3 cameraRotation;

    [SerializeField]  //<----- this will make variable appear in editor even though it's private
    private float speed = 5f;
    [SerializeField]
    private float jumpForce = 8.0f;

    private Rigidbody rb;
    private Camera cam;

    private bool jumpKeyPressed = false;
    private bool isJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        movementStats();
        
        if (Input.GetAxisRaw("Jump") != 0)
        {
            if (!jumpKeyPressed)
            {
                jumpKeyPressed = true;
                jump();
            }
        } else if (jumpKeyPressed)
        {
            jumpKeyPressed = false;
        }
    }

    void FixedUpdate()
    {
        // Perform movement 
        if (movement != Vector3.zero)
        {
            move(movement);      // in FixedUpdate because we want to apply force at regular intervals
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            isJumping = false;
        }
    }

    // Called Functions \\

    public void movementStats()
    {
        // Calculate movement
        Vector3 _moveHor = transform.right * Input.GetAxisRaw("Horizontal");    // returns float between 1 and -1 related to input. see Edit --> Proj settings --> Input
        Vector3 _moveVec = transform.forward * Input.GetAxisRaw("Vertical");
        movement = (_moveHor + _moveVec).normalized;

        // Calculate rotation
        float _yRot = Input.GetAxisRaw("Mouse X");                              // mouse X axis is left/right, when we move mouse left/right, we look around our Y axis (left/right)

        rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        //Perform rotation 
        rotatePlayer(rotation);                                // in Update because we want rotation to be as smooth as the game

        // Calculate Camera rotation
        float _xRot = Input.GetAxisRaw("Mouse Y");                              // mouse Y axis is up/down, when we move mouse up/down, we look around our X axis (up/down)

        _xRot = Mathf.Clamp(_xRot, -90, 90);

        cameraRotation = new Vector3(_xRot, 0f, 0f) * lookSensitivity;

        // Perform camera rotation 
        rotateCam(cameraRotation);
    }

    public void move(Vector3 moveVector)
    {
        rb.MovePosition(rb.position + moveVector * Time.deltaTime * speed);    // This is applying a force to the rigidbody, which is a physics calculation
    }
    public void rotatePlayer(Vector3 rotationVector)
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationVector));
    }

    public void rotateCam(Vector3 camRotationVector)
    {
        if (cam != null)
        {
            cam.transform.Rotate(camRotationVector * -1);                          // in Update because we want rotation to be as smooth as the game
        }
    }
    public void jump()
    {
        if (!isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
}