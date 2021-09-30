using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isGrounded;
    public bool isSprinting;
    Transform camera;
    float vertical, horizontal;
    float mouseX, mouseY;
    Vector3 velocity;
    public float speedwalk = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float playerWidth = 0.15f;
    public float boundsTolerance = 0.1f;
    float verticalMomentum=0;
    bool jumpRequest;
    public float gravity = -9.8f;
    World world;

    private void Start()
    {

        world = GameObject.Find("World").GetComponent<World>();
        camera = transform.GetChild(0);
        if (camera == null)
            camera = GameObject.Find("Main Camera").transform;
    }
    private void FixedUpdate()
    {
        calculateVelocity();

        if (jumpRequest)
            Jump();

        transform.Rotate(Vector3.up * mouseX);
        camera.Rotate(Vector3.right * -mouseY);
        transform.Translate(velocity, Space.World);
    }
    private void Update()
    {
        getPlayerInputs();
      
    }
    void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }
    void calculateVelocity()
    {
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * speedwalk;

        //falling damage i guess
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z) > 0 && front || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x) > 0 && right || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);


    }
    void getPlayerInputs()
    {
        //GetAxis return smoothed sensivity. But GetAxisRaw only return 0 or -1. 
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        mouseY = Mathf.Clamp(mouseY, -30, 60);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            isSprinting = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isSprinting = false;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            jumpRequest = true;

    }

    public float checkDownSpeed(float downSpeed)
    {
        if(
          world.CheckForVoxel(transform.position.x - playerWidth,transform.position.y+downSpeed,transform.position.z - playerWidth) ||
      world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth) ||
      world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth) ||
      world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth) 
          )
                 {
            isGrounded = true;
            return 0;
                 }
        else
        {
            isGrounded = false;
            return downSpeed;
        }

    }

    public float checkUpSpeed(float upSpeed)
    {
        if (
      world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth) ||
      world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth) ||
      world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth) ||
      world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)
          )
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            return upSpeed;
        }

    }

    public bool front
    {
        get { if (
                world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z + playerWidth) ||
                world.CheckForVoxel(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth) 
                )
            {
                return true;
            }

            else
                return false;
                
            }
    }

    public bool back
    {
        get
        {
            if (
              world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z - playerWidth) ||
              world.CheckForVoxel(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth)
              )
            {
                return true;
            }

            else
                return false;

        }
    }

    public bool left
    {
        get
        {
            if (
              world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y, transform.position.z) ||
              world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z)
              )
            {
                return true;
            }

            else
                return false;

        }
    }

    public bool right
    {
        get
        {
            if (
              world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y, transform.position.z) ||
              world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z)
              )
            {
                return true;
            }

            else
                return false;

        }
    }

}

