using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Audio code
    public AudioSource musicSource;

    public AudioClip musicClipLose;
    public AudioClip musicClipJump;
    public AudioClip musicClipMac;
    public AudioClip musicClipBlock;

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
    private GrappleGun grappleGun;

    //Block placement and removal variables
    private ThrowableScript throwableScript;
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

    [SerializeField]
    private Animator Animation;

    [SerializeField]
    private GameObject pauseScreen;

    public bool paused = false;

    private Rigidbody movingPlatformRigidBody;
    private bool onPlatform = false;
    private MovingBlocks movingBlocks;

    [SerializeField]
    private GameObject stage1Light;
    [SerializeField]
    private GameObject stage2Light;
    [SerializeField]
    private GameObject stage3Light;

    private static bool firstHubVisit = true;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        activeScene = SceneManager.GetActiveScene();
        ammoText.text = "Ammo: " + ammoCount.ToString();
        cameraForward = Camera.main.transform.forward;

        /*
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
        */

        Cursor.lockState = CursorLockMode.Locked;

        if(activeScene.name == "HubLevel")
        {
            if(stage1 == true)
            {
                stage1Light.SetActive(false);
            }
            if(stage2 == true)
            {
                stage2Light.SetActive(false);
            }
            if(stage3 == true)
            {
                stage3Light.SetActive(false);
            }
            if (firstHubVisit)
            {
                transform.position = new Vector3(0, 13, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (paused == false)
        {
            movementStats();
        }

        endState();
        if (Input.GetAxisRaw("Cancel") != 0 && paused == false)
        {
            paused = true;
            Instantiate(pauseScreen);
        }
        if (movement == Vector3.zero)
        {
            Animation.SetInteger ("intController", 0);
        }
        if (movement != Vector3.zero)
        {
            Animation.SetInteger ("intController", 1);
        }
        if (Input.GetAxisRaw("Jump") != 0)
        {
            Animation.SetInteger ("intController", 2);
        }
        if (Input.GetAxisRaw("Fire1") != 0)                                     // this axis will be != 0 if the 'e' key is pressed
        {   
            Animation.SetInteger ("intController", 3);
        }
        if (Input.GetAxisRaw("Fire2") != 0)                                     // this axis will be != 0 if the 'e' key is pressed
        {
            Animation.SetInteger ("intController", 3);
        }
        if (movement == Vector3.zero && grappleGun.grappling == true)                                     // this axis will be != 0 if the 'e' key is pressed
        {
            Animation.SetInteger ("intController", 4);
        }
    }

    void FixedUpdate()
    {
        // Perform movement 
        if (movement != Vector3.zero)
        {
            move(movement);      // in FixedUpdate because we want to apply force at regular intervals
        }
        else
        {
            if (onPlatform)
            {
                if (movingBlocks.isWaiting == false)
                {
                    rb.MovePosition(rb.position + movingBlocks.direction * Time.deltaTime * movingBlocks.speed);
                }
            }
        }

        //jumping
        if (Input.GetAxisRaw("Jump") != 0)
        {
            if (!jumpKeyPressed)
            {
                jumpKeyPressed = true;
                jump();
                musicSource.clip = musicClipJump;
                musicSource.Play();
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
                musicSource.clip = musicClipBlock;
                musicSource.Play();
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
        else if (other.tag == "mp")
        {
            isJumping = false;
            movingPlatformRigidBody = other.GetComponent<Rigidbody>();
            onPlatform = true;
            movingBlocks = other.GetComponent<MovingBlocks>();
        }
        else if (other.tag == "MacGuffin")
        {
            MacGuffinCollection(other.gameObject);
            musicSource.clip = musicClipMac;
            musicSource.Play();
        }
        else
        {
            Teleport(other.tag);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "mp")
        {
            onPlatform = false;
        }
    }

    // Called Functions \\

    public void movementStats()
    {
        // Calculate movement
        Vector3 _moveHor = transform.right * Input.GetAxisRaw("Horizontal");    // returns float between 1 and -1 related to input. see Edit --> Proj settings --> Input
        Vector3 _moveVec = transform.forward * Input.GetAxisRaw("Vertical");
        if (grappleGun.grappling == true)
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
        if (onPlatform)
        {
            if (movingBlocks.isWaiting == false)
            {
                rb.MovePosition(rb.position + moveVector * Time.deltaTime * speed + movingBlocks.direction * Time.deltaTime * movingBlocks.speed);
            }
        }
        else
        {
            rb.MovePosition(rb.position + moveVector * Time.deltaTime * speed);    // This is applying a force to the rigidbody, which is a physics calculation
        }
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
        if (!isJumping && grappleGun.grappling != true)
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
            GameObject _projectileObject = Instantiate(blockPlace, rb.position + cameraForward + Vector3.up * .2f, Quaternion.identity); // local variables start w/ and underscore
            ammoCount--;
            ammoText.text = "Ammo: " + ammoCount.ToString();
            throwableScript = _projectileObject.GetComponent<ThrowableScript>();
            throwableScript.Launch(cameraForward, 2000);
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
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Win");
        }
        else if (transform.position.y < -15.0f)
        {
            Loss();
        }
    }
    public void MacGuffinCollection(GameObject other)
    {
        if (activeScene.name == "KitchenLevel")
            {
                Destroy(other);
                stage1 = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("HubLevel");
            }
            else if (activeScene.name == "BedRoomLevel")
            {
                Destroy(other);
                stage2 = true;
                SceneManager.LoadScene("HubLevel");
            }
            else if (activeScene.name == "PlayGroundLevel")
            {
                Destroy(other);
                stage3 = true;
                SceneManager.LoadScene("HubLevel");
            }
        
    }
    public void Teleport(string tag)
    {
        firstHubVisit = false;

        if (tag == "Teleporter1")
        {
            if (stage1 == false)
            {
                SceneManager.LoadScene("KitchenLevel");
            }
        }
        else if (tag == "Teleporter2")
        {
            if (stage2 == false)
            {
                SceneManager.LoadScene("BedRoomLevel");
            }
        }
        else if (tag == "Teleporter3")
        {
            if (stage3 == false)
            {
                SceneManager.LoadScene("PlayGroundLevel");
            }
        }
        //return to hub world
        else if (tag == "TeleporterHome")
        {
            SceneManager.LoadScene("MainScene");
        }
    }
    public void Loss()
    {
        //ammoCount = 3;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Lose");
    }
}