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

    [SerializeField]
    private GameObject levelOneCinematic;
    [SerializeField]
    private GameObject levelTwoCinematic;
    [SerializeField]
    private GameObject levelThreeCinematic;
    [SerializeField]
    private GameObject hubCinematic;

    //level cinematics
    private static bool cinematicOneNotDone = true;
    private static bool cinematicTwoNotDone = true;
    private static bool cinematicThreeNotDone = true;
    private static bool cinematicHubNotDone = true;

    private GameObject dialogueObject;
    private AudioSource dialogueSource;
    private GameObject dialogueTextObject;
    private Text DialogueText;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        activeScene = SceneManager.GetActiveScene();
        ammoText.text = "Ammo: " + ammoCount.ToString();
        cameraForward = Camera.main.transform.forward;

        //Cinematics
        if (activeScene.name == "KitchenLevel")
        {
            Cinematic(2, cinematicOneNotDone);
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            Cinematic(4, cinematicTwoNotDone);
        }
        else if (activeScene.name == "PlayGroundLevel")
        {
            Cinematic(3, cinematicThreeNotDone);
        }

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
                Cinematic(1, cinematicHubNotDone);
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
        if(paused == false)
        {
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
            } 
            else if (jumpKeyPressed)
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
            } 
            else if (fireKeyPressed)
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
            } 
            else if (removeKeyPressed)
            {
                removeKeyPressed = false;
            }
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
            rb.MovePosition(rb.position + moveVector * Time.deltaTime * speed + movingBlocks.direction * Time.deltaTime * movingBlocks.speed);
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
            firstHubVisit = true;
            cinematicThreeNotDone = true;
            cinematicTwoNotDone = true;
            cinematicOneNotDone = true;
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
    private void Cinematic(int cinematicNumber, bool notDoneBefore)
    {
        if(notDoneBefore)
        {
            //hub level
            if(cinematicNumber == 1)
            {
                Instantiate(hubCinematic);
                cinematicHubNotDone = false;
                paused = true;
                dialogueObject = GameObject.FindWithTag("dialogue");
                dialogueSource= dialogueObject.GetComponent<AudioSource>();
                dialogueTextObject = GameObject.FindWithTag("DialogueText");
                DialogueText = dialogueTextObject.GetComponent<Text>();
                DialogueText.text = "W-where...where am I...how’d did I…*sigh*. Are these, my memories….why am I here?";
                Invoke("Textshift1", 16.0f);
                Invoke("Textshift2", 26.0f);
                Invoke("Textshift3", 38.0f);
                Invoke("CinematicEnd", dialogueSource.clip.length);
            }
            //kitchen level
            else if(cinematicNumber == 2)
            {
                Instantiate(levelTwoCinematic);
                cinematicTwoNotDone = false;
                paused = true;
                dialogueObject = GameObject.FindWithTag("dialogue");
                dialogueSource= dialogueObject.GetComponent<AudioSource>();
                dialogueTextObject = GameObject.FindWithTag("DialogueText");
                DialogueText = dialogueTextObject.GetComponent<Text>();
                DialogueText.text = "Wait, I remember this day! I begged my mom all day to let me stay home, I tried to fake a cold but she saw right through that.";
                Invoke("Textshift1", 8.0f);
                Invoke("Textshift2", 13.0f);
                Invoke("Textshift3", 19.0f);
                Invoke("Textshift4", 26.0f);
                Invoke("Textshift5", 32.0f);
                Invoke("Textshift6", 38.0f);
                Invoke("Textshift7", 42.0f);
                Invoke("Textshift8", 48.0f);
                Invoke("Textshift9", 57.0f);        
                Invoke("CinematicEnd", dialogueSource.clip.length);
            }
            //PlayGround level
            else if(cinematicNumber == 3)
            {
                Instantiate(levelThreeCinematic);
                cinematicThreeNotDone = false;
                paused = true;
                dialogueObject = GameObject.FindWithTag("dialogue");
                dialogueSource= dialogueObject.GetComponent<AudioSource>();
                dialogueTextObject = GameObject.FindWithTag("DialogueText");
                DialogueText = dialogueTextObject.GetComponent<Text>();
                DialogueText.text = "My mom always loved taking me to the playground. She always said I was a creative kid and really enjoyed watching me pretend to be a cowboy or a pirate.";
                Invoke("Textshift1", 8.0f);
                Invoke("Textshift2", 12.0f);
                Invoke("Textshift3", 17.0f);
                Invoke("Textshift4", 25.0f);
                Invoke("Textshift5", 33.0f);
                Invoke("Textshift6", 41.0f);
                Invoke("Textshift7", 48.0f);
                Invoke("Textshift8", 55.0f);
                Invoke("CinematicEnd", dialogueSource.clip.length);
            }
            //Bedroom level
            else
            {
                Instantiate(levelOneCinematic);
                cinematicOneNotDone = false;
                paused = true;
                dialogueObject = GameObject.FindWithTag("dialogue");
                dialogueSource= dialogueObject.GetComponent<AudioSource>();
                dialogueTextObject = GameObject.FindWithTag("DialogueText");
                DialogueText = dialogueTextObject.GetComponent<Text>();
                DialogueText.text = "My mom always supported my addiction to games. She really loved playing them herself too. We would play board games like monopoly, and just classic games like hide and seek.";
                Invoke("Textshift1", 9.0f);
                Invoke("Textshift2", 6.0f);
                Invoke("Textshift3", 6.0f);
                Invoke("Textshift4", 6.0f);
                Invoke("Textshift5", 6.0f);
                Invoke("Textshift6", 5.0f);
                Invoke("Textshift7", 6.0f);
                Invoke("Textshift8", 6.0f);
                Invoke("Textshift9", 10.0f);
                Invoke("Textshift10", 10.0f);
                Invoke("CinematicEnd", dialogueSource.clip.length);
            }
        }
    }
    private void Textshift1()
    {
        if (activeScene.name == "HubLevel")
        {
            DialogueText.text = "wait...mom? These not just any memories, these are the ones...with mom.";
        }
        else if (activeScene.name == "KitchenLevel")
        {
            DialogueText.text = "She said I could stay if I helped her baked, but I hated cooking.";
        }
        else if (activeScene.name == "PlayGroundLevel")
        {
            DialogueText.text = "I remember the day I was The Might Knight of Castle Yamada!";
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = "But I remember when she got me a gameboy for my birthday one year, and it was amazing!";
        }
    }
    private void Textshift2()
    {
        if(activeScene.name == "HubLevel")
        {
            DialogueText.text = "All my favorite ones...and  are these...portals? Maybe I can...see her again? Hm.";
        }
        else if (activeScene.name == "KitchenLevel")
        {
            DialogueText.text = "However I figured that pushing through a few hours of baking were better than a whole day of math so, I said fine.";
        }
        else if (activeScene.name == "PlayGroundLevel")
        {
            DialogueText.text = "I made a super cool Sandcastle, and used my favorite shovel as my “sword”, but right before I was done,";
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = "The first game I was able to buy for it was Super Mario, and I try my best to beat as many levels as I could before bedtime.";
        }
    }
    private void Textshift3()
    {
        if(activeScene.name == "HubLevel")
        {
            DialogueText.text = "They seem to be...calling me. Maybe they can help find a way to get me out of here.";
        }
        else if (activeScene.name == "KitchenLevel")
        {
            DialogueText.text = "Mom said that we were going to bake cookies to bring to my aunt’s house the next day, and my favorite kind too, “super” chocolate chip.";
        }
        else if (activeScene.name == "PlayGroundLevel")
        {
            DialogueText.text = "one of the other kid’s frisbee accidentally flew into it and knocked down half of my castle. I was so devastated. I fell to my knees and started crying.";
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = ". One night I was really struggling on the last level,  just didn’t know how to beat it, and my mom heard me getting frustrated.";
        }
    }
    private void Textshift4()
    {
        if (activeScene.name == "KitchenLevel")
        {
            DialogueText.text = "I thought it would be cool to get to know how my mom made her famous cookies, and I actually ended up loving it.";
        }
        else if (activeScene.name == "PlayGroundLevel")
        {
            DialogueText.text = "The kid who threw the frisbee felt really bad about it too and apologized, but I wasn’t mad at him, just sad.";
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = "She came in my room and asked what was wrong, and I told her how annoyed I was getting with the level.";
        }
    }
    private void Textshift5()
    {
        if (activeScene.name == "KitchenLevel")
        {
            DialogueText.text = "She let me help her with every part, and she even let me mix the dough with her favorite wooden spoon.";
        }
        else if (activeScene.name == "PlayGroundLevel")
        {
            DialogueText.text = "My mom then picked me off me knees and wiped my tears, and told me that sometimes things we love get broken, but all it does is add history to it.";
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = "She then sat with me, and propped up my favorite Teddy Bear, Jerry, next to me.";
        }
    }
    private void Textshift6()
    {
        if (activeScene.name == "KitchenLevel")
        {
            DialogueText.text = "I don’t know what made it her favorite, maybe because it was her only one.";
        }
        else if (activeScene.name == "PlayGroundLevel")
        {
            DialogueText.text = "And with that history, it made it more beautiful. She told me she would help me rebuild it, no matter how long it took.";
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = "She told me that the two of them were my cheerleaders and were going to cheer me on until I could finish the level. Now I was determine to get it done!";
        }
    }
    private void Textshift7()
    {
        if (activeScene.name == "KitchenLevel")
        {
            DialogueText.text = "After that day I loved baking with her, and we always made such big messes when we did.";
        }
        else if (activeScene.name == "PlayGroundLevel")
        {
            DialogueText.text = "And after what seemed like hours, Castle Yamada was done! I even decided to leave a piece of the fallen sandcastle in the front.";
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = "The two of them gave me support the whole time. Mom even let me stay up pass my bedtime till I could do it!";
        }
    }
    private void Textshift8()
    {
        if (activeScene.name == "KitchenLevel")
        {
            DialogueText.text = "Mom always says that messes can be beautiful, that life requires a little bit of chaos, or at least... she used to say…";
        }
        else if (activeScene.name == "PlayGroundLevel")
        {
            DialogueText.text = "The two of us were now crowned protectors of the castle. I think I can actually...see my shovel. Castle Yamada, here I come.";
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = "After a few more attempts, I finally beat it, and my mom told me how proud she was. She told me that whenever I had a goal, I was always determined to get to that finish.";
        }
    }
    private void Textshift9()
    {
        if (activeScene.name == "KitchenLevel")
        {
            DialogueText.text = "Mom always says that messes can be beautiful, that life requires a little bit of chaos, or at least... she used to say…";
        }
        else if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = "She tucked me in after that with Jerry, and I remember even now, no matter what I goals I set, I can still hear my mom cheering me on.";
        }
    }
    private void Textshift10()
    {
        if (activeScene.name == "BedRoomLevel")
        {
            DialogueText.text = "I can hear Jerry too, with my imaginary voice I gave him. I think...I think I actually see him...over there...I got to get to him!";
        }
    }
    private void CinematicEnd()
    {
        paused = false;
        Destroy(GameObject.FindWithTag("Cinematic"));
    }
}