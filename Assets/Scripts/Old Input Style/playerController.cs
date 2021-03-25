﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private Vector3 cameraForward;

    private bool jumpKeyPressed = false;
    private bool isJumping = false;

    [SerializeField]
    private grappleGun GrappleGun;

    //Block placement and removal variables
    private throwableScript ThrowableScript;
    private int ammoCount = 3;
    [SerializeField]
    private Text ammoText;
    private Vector3 raycastLine;
    [SerializeField]
    private GameObject blockPlace;
    private bool fireKeyPressed = false;
    private bool removeKeyPressed = false;

    //Stage Completion Variables
    private static bool stage1 = false;
    private static bool stage2 = false;
    private static bool stage3 = false;
    private Scene activeScene;
    [SerializeField]
    private GameObject MacGuffin;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        activeScene = SceneManager.GetActiveScene();
        ammoText.text = "Ammo: " + ammoCount.ToString();
        cameraForward = Camera.main.transform.forward;
        //spawning in the MacGuffin
        if (activeScene.name == "Level1")
        {
            if (stage1 == false)
            {
                Instantiate(MacGuffin, new Vector3 (-5, 0, 14), Quaternion.identity);
            }
        }
        else if (activeScene.name == "Level2")
        {
            if (stage2 == false)
            {
                Instantiate(MacGuffin, new Vector3 (3, 0, 5), Quaternion.identity);
            }   
        }
        else if (activeScene.name == "Level3")
        {
            if (stage3 == false)
            {
                Instantiate(MacGuffin, new Vector3 (-4, 0.5f, 7), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        movementStats();
        endState();
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
        
    }

    void FixedUpdate()
    {
        // Perform movement 
        if (movement != Vector3.zero)
        {
            move(movement);      // in FixedUpdate because we want to apply force at regular intervals
        }

        //jumping
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

        //placing a block
        if (Input.GetAxisRaw("Fire1") != 0)                                     // this axis will be != 0 if the 'e' key is pressed
        {
            if (!fireKeyPressed)
            {
                fireKeyPressed = true;
                deployToy();
            }
        } else if (fireKeyPressed)
        {
            fireKeyPressed = false;
        }

        //removing a placed block
        if (Input.GetAxisRaw("Fire2") != 0)                                     // this axis will be != 0 if the 'e' key is pressed
        {
            if (!removeKeyPressed)
            {
                removeKeyPressed = true;
                removeToy();
            }
        } else if (removeKeyPressed)
        {
            removeKeyPressed = false;
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
        else if (other.tag == "MacGuffin")
        {
            MacGuffinCollection(other.gameObject);
        }
        else
        {
            Teleport(other.tag);
        }
    }

    // Called Functions \\

    public void movementStats()
    {
        // Calculate movement
        Vector3 _moveHor = transform.right * Input.GetAxisRaw("Horizontal");    // returns float between 1 and -1 related to input. see Edit --> Proj settings --> Input
        Vector3 _moveVec = transform.forward * Input.GetAxisRaw("Vertical");
        if (GrappleGun.grappling == true)
        {
            movement = Vector3.zero;
        }
        else
        {
            movement = (_moveHor + _moveVec).normalized;
        }

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
        cameraForward = Camera.main.transform.forward;
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
    public void deployToy()
    {
        if( ammoCount <= 0){ return; }

        raycastLine = cameraForward;
        //collide terrain
        if (Physics.Raycast(rb.position + Vector3.up * .5f, raycastLine, 1.0f,LayerMask.GetMask("terrain")))
        {
            // do nothing
            print("terrain too close");
        }
        //collide toy
        else if (Physics.Raycast(rb.position + Vector3.up * .5f, raycastLine, 1.0f,LayerMask.GetMask("toy")))
        {
            // do nothing
            print("terrain too close");
        }
        //No terrain too close. Can place block.
        else
        {
            GameObject _projectileObject = Instantiate(blockPlace, rb.position + cameraForward + Vector3.up * .3f, Quaternion.identity); // local variables start w/ and underscore
            ammoCount--;
            ammoText.text = "Ammo: " + ammoCount.ToString();
            ThrowableScript = _projectileObject.GetComponent<throwableScript>();
            ThrowableScript.Launch(cameraForward, 2000);
        }
    }
    public void removeToy()
    {
        raycastLine = cameraForward;
        //collide toy
        RaycastHit hit;
        //if it hits a toy
        if (Physics.Raycast(rb.position + Vector3.up * .5f, raycastLine, out hit , 2.5f,LayerMask.GetMask("toy")))
        {
            if (hit.collider.tag == "toyBody")
            {
                Destroy(hit.collider.gameObject);
                ammoCount++;
                ammoText.text = "Ammo: " + ammoCount.ToString();
            }
            else if (hit.collider.tag == "Ground")
            {
                Destroy(hit.transform.parent.gameObject);
                ammoCount++;
                ammoText.text = "Ammo: " + ammoCount.ToString();
            }
        }
    }
    public void endState()
    {
        if(stage1 && stage2 && stage3)
        {
            //ammoCount = 3;
            stage1 = false;
            stage2 = false;
            stage3 = false;
            SceneManager.LoadScene("winScene");
        }
        else if (transform.position.y < -50.0f)
        {
            //ammoCount = 3;
            SceneManager.LoadScene(activeScene.name);
        }
    }
    public void MacGuffinCollection(GameObject other)
    {
        if (activeScene.name == "Level1")
            {
                Destroy(other);
                stage1 = true;
            }
            else if (activeScene.name == "Level2")
            {
                Destroy(other);
                stage2 = true;
            }
            else if (activeScene.name == "Level3")
            {
                Destroy(other);
                stage3 = true;
            }
    }
    public void Teleport(string tag)
    {
        if (tag == "Teleporter1")
        {
            SceneManager.LoadScene("Level1");
        }
        else if (tag == "Teleporter2")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (tag == "Teleporter3")
        {
            SceneManager.LoadScene("Level3");
        }
        //return to hub world
        else if (tag == "TeleporterHome")
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}