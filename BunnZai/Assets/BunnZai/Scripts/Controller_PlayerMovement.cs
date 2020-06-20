using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO:
/// 
/// ---REWRITE Avatar Rotation. It needs to be decoupled from the rb.velocity(maybe just the PITCH component...) and save it's state for whenever the character doesnt move
/// 
/// ---DEVBUG!!!!: Airstrafing adds velocity to character!!! Introduced while tackling the next point on the list.
/// ---Write proper airstrafing. It should allow a greater amount of control without itself impacting velocity. Rotate velocity vector?
/// 
/// ---Configure walljumping to be satisfying.
///    --> When jumping along the wall the original velocity is only marginally deviant from the resulting reflextion
///         so the player can endlessly accelerate along a straight wall
/// 
/// </summary>

public class Controller_PlayerMovement : MonoBehaviour
{
    public Transform playerAvatar;      //World representation of the player pawn. Has has the player collider on it!
    public Transform camCrane;
    public CapsuleCollider playerCollider;
    [Header("Variables")]
    public float accelSpeed;            //Acceleration modifier
    public float airAccelSpeed;         //Air acceleration modifier
    public float maxGroundSpeed = 3;    //Maximum running speed
    public float groundSlowdown = 5;    //Slowdown factor when walking on foot
    public float dashSpeed = 15f;
    public Rigidbody rb;
    [Header("Hidden Variables")]
    Vector3 jumpVector;
    Vector3 movementVector;
    Quaternion flatQ;                   //Quaternion for horizontal rotation
    Quaternion camViewDir;              // Current direction in relation to camera view, flattened to only Z,X axes.
    //Quaternion relInputDir;           // CAM Relative input direction, flattened to 2d.
    Quaternion moveDir;                 //Current direction of movement of the Pawn
    Quaternion direction;
   
    bool wallhit = false;
    bool dashing = false;
    bool dashAllowed = true;
    bool onGround = false;
    float dashTimer = 0;
    float gravityScale = 0;


    // Start is called before the first frame update
    void Start()
    {
        camViewDir = new Quaternion();
        moveDir = new Quaternion();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;
        if (dashTimer <= 2.7f && dashing)
            EndDash();
        if (dashTimer <= 0)
        {
            dashAllowed = true;
            Debug.Log("You can dash again!");
        }

        Debug.Log(dashAllowed);
        jumpVector = new Vector3(0, 5, Mathf.Clamp(rb.velocity.magnitude, 0, 2));
        camViewDir.eulerAngles = new Vector3(0, camCrane.rotation.eulerAngles.y, camCrane.rotation.eulerAngles.z); //Strips CAM ANGLE of it's PITCH, to use the rest for MOVEMENT DIRECTION
        moveDir = Quaternion.LookRotation(rb.velocity, Vector3.up); //Looks through vector to get rotation/direction. Up-vector is needed to stabilize against spin along the vector.
        direction = playerAvatar.rotation;
    }
    private void FixedUpdate()
    {
        rb.AddForce(Physics.gravity * gravityScale);
    }
    //This handles pawn movemnt, including outside of player input
    void MovePlayer()
    {
        float strafe = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        //if (!onGround && wallhit)
        //    rb.velocity = Vector3.zero;
        if (!onGround && wallhit)
            Wallrun();
        else
            EndWallrun();
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
            JumpPlayer();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!onGround && wallhit)
                WallJump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashAllowed)
            Dash();
        /// !!!! TODO: SEPARATE MOVEMENT FOR GROUNDED AND UNGROUNDED PLAYER !!!!
        /// 
        ///--Ground/Airborne checks should be done anyway, at least for animation reasons
        ///--Grounded player doesn't use CAM PITCH for the movement vector, as running into
        ///the ground or upwards doesn't make sense. Airborne player on the other hand might 
        ///need pitched movement for jump/flight control.
        ///
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (onGround)
            {
                //Behavior if player is grounded
                //PITCHLESS CHARACTER MOVEMENT

                movementVector = camViewDir * (new Vector3(strafe, 0f, forward) * accelSpeed * Time.deltaTime);     //Maps forward/strafe controls in relation to PITCHLESS CAM DIR; Results in movement direction vector.
                rb.velocity = Vector3.ClampMagnitude(rb.velocity + movementVector, maxGroundSpeed);

                //Handle avatar rotation
                //ROTATE PLAYER AVATAR ON GROUND
                //Can this be done simpler?

                Quaternion rotHelp = new Quaternion();
                rotHelp.eulerAngles = new Vector3(0, moveDir.eulerAngles.y, moveDir.eulerAngles.z);
                playerAvatar.rotation = rotHelp;   //Set Avatar Rotation to match movement vector
                //END PITCHLESS MOVEMENT
            }
            else
            {
                //Behavior if player is airborne
                movementVector = camViewDir * (new Vector3(strafe, 0f, forward) * airAccelSpeed * Time.deltaTime);     //Maps forward/strafe controls in relation to PITCHLESS CAM DIR; Results in movement direction vector.
                rb.velocity += movementVector;
                Quaternion rotHelp = new Quaternion();
                rotHelp.eulerAngles = new Vector3(-Mathf.Clamp(moveDir.eulerAngles.x, -20, 20), moveDir.eulerAngles.y, 0);
                playerAvatar.rotation = rotHelp;   //Set Avatar Rotation to match movement vector
            }
        }
        //Debug.Log(rb.velocity.magnitude);
        HandleVelocityFalloff();
    }

    //This function should yeet the player
    void JumpPlayer()
    {
        rb.velocity = direction * jumpVector;
        onGround = false;
    }
    void Dash()
    {
        dashTimer = 3;
        dashing = true;
        rb.velocity += direction * new Vector3(1, 1, dashSpeed);
        dashAllowed = false;
    }
    void EndDash()
    {
        dashing = false;
        rb.velocity = rb.velocity.normalized;
    }
    void Wallrun()
    {
        //Todo: lower velocity && keep on wall
        gravityScale = -0.85f;
        rb.velocity *= 0.995f;
    }

    void EndWallrun()
    {
        //Todo: lower velocity && keep on wall
        gravityScale = 0;
    }
    void WallJump()
    {
        rb.velocity = direction * new Vector3(1, 1, 15);
        wallhit = false;
        dashAllowed = true;
    }
    void HandleVelocityFalloff()
    {
        if (onGround && !Input.GetKeyDown("space"))
        {
            if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d"))
                rb.velocity *= 0.9f;
        }
        //new Vector3(rb.velocity.x - rb.velocity.x * Time.deltaTime * groundSlowdown, rb.velocity.y - rb.velocity.y * Time.deltaTime * groundSlowdown, rb.velocity.z - rb.velocity.z * Time.deltaTime * groundSlowdown);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            //dashAllowed = true;
            onGround = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            onGround = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
            wallhit = true;
       
    }
    private void OnTriggerExit(Collider other)
    {
        wallhit = false;
    }
    //bool CheckWallrun()
    //{
    //    RaycastHit hitInfoRight;
    //    RaycastHit hitInfoLeft;
    //    if (Physics.Raycast(transform.position, playerAvatar.TransformDirection(Vector3.right), out hitInfoRight, 1f) && hitInfoRight.collider.CompareTag("Wall"))
    //    {
    //        if (!rightWallhit)
    //            rightWallhit = true;
    //        return true;
    //    }
    //    if (Physics.Raycast(transform.position, playerAvatar.TransformDirection(Vector3.left), out hitInfoLeft, 1f) && hitInfoLeft.collider.CompareTag("Wall"))
    //    {
    //        if (!leftWallhit)
    //            leftWallhit = true;
    //        return true;
    //    }
    //    return false;
    //}
}
