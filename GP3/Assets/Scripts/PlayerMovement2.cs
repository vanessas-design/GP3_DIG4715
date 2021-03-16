using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement2 : MonoBehaviour
{
    //basically bringing the player controller component to the script
    public CharacterController controller;

    //giving the player a speed to move at
    public float speed = 12f;

    //stores the current velocity of the player
    Vector3 velocity;

    //giving a value to gravity / downward acceleration (Earth's gravitational acceleration is -9.81 m/s^2)
    public float gravity = -9.81f;

    //designated jumpheight for the player
    public float jumpHeight = 3f;

    //variable that verifies if the player is touching the ground
    public Transform groundCheck;
    //radius for the sphere used in the ground check
    public float groundDistance = 0.4f;
    //used to control what objects the sphere is checking for
    public LayerMask groundMask;

    //Used to control UI text
    
    //light
    
    //tells whether the player is grounded or not
    private bool isGrounded;

    //Used for Audio
    

    void Start()
    {
        
        
        
    }
    
    // Update is called once per frame
    void Update()
    {
        //checks to see if the player is grounded or not
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //checks to see if the player is on the ground. If the player is not grounded, they will fall.
        if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

        //getting player input using variables exclusive to the Update function (privatized within the function)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //converts input into a direction the player moves in
        Vector3 move = transform.right * x + transform.forward * z ;

        //telling the player controller to actually move the player at the end of the function
        //all of the code above needs to be run through before this can be reached and movement is in fact possible, might do this in fixedUpdate instead of a regular Update function
        //move * speed will move the player at a magnitude of whatever the value of the 'speed' variable is at
        //Time.deltaTime is added so that the changes in player speed are not dependant on the systems framerate
        controller.Move(move * speed * Time.deltaTime);

        //if statement that checks to see if the player is grounded and if they have pressed the "Jump" button
        if(Input.GetButtonDown("Jump") && isGrounded)
            {
                //if the player is grounded and they press the 'jump' button, their velocity on the y vectore will be modified by the equation below
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

        //I honestly don't remember what this does, maybe mess with it and see what breaks?
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        //allows the player to exit the game when the 'escape key is pressed'
        
        //Checking to see if the LeftShift key is down
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //when it is down, player's speed is double to 12
            speed = 12f;
        }
        //Checking to see if the LeftShift key is up
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            //when it is up, the player's speed is set to 6
            speed = 6f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
    }

    //Function used as an event handler for when the player collides with the Mummy or the Macguffin
    
}
