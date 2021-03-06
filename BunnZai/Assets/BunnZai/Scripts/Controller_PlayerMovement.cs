﻿using System.Collections;
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
    public float accelSpeed;            //Acceleration modifier
    public float airAccelSpeed;         //Air acceleration modifier
    public float maxGroundSpeed = 3;    //Maximum running speed
    public float groundSlowdown = 5;    //Slowdown factor when walking on foot
    Rigidbody rb;
    Vector3 jumpVector;
    Vector3 movementVector;
    Quaternion flatQ;                   //Quaternion for horizontal rotation
    Quaternion camViewDir;              // Current direction in relation to camera view, flattened to only Z,X axes.
    //Quaternion relInputDir;           // CAM Relative input direction, flattened to 2d.
    Quaternion moveDir;                 //Current direction of movement of the Pawn
    RaycastHit feetHit;
    bool playerGRND;                    //Stores player "standing" state for each frame


    // Start is called before the first frame update
    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        camViewDir = new Quaternion();
        moveDir = new Quaternion();
    }

    // Update is called once per frame
    void Update() 
    {
        MovePlayer();


        jumpVector = new Vector3(0, 5, Mathf.Clamp(rb.velocity.magnitude, 0, 2));
        camViewDir.eulerAngles = new Vector3(0, camCrane.rotation.eulerAngles.y, camCrane.rotation.eulerAngles.z); //Strips CAM ANGLE of it's PITCH, to use the rest for MOVEMENT DIRECTION
        moveDir = Quaternion.LookRotation(rb.velocity, Vector3.up); //Looks through vector to get rotation/direction. Up-vector is needed to stabilize against spin along the vector.

        //Visualize vectors
        Debug.DrawRay(playerAvatar.position, playerAvatar.rotation * Vector3.forward, Color.blue, 0);
        Debug.DrawRay(playerAvatar.position, playerAvatar.rotation * Vector3.left, Color.green, 0);
        Debug.DrawRay(playerAvatar.position, playerAvatar.rotation * Vector3.right, Color.red, 0);
        Debug.DrawRay(playerAvatar.position, (playerAvatar.rotation * Vector3.right) + (playerAvatar.rotation * Vector3.forward), Color.yellow, 0);
        Debug.DrawRay(playerAvatar.position, (playerAvatar.rotation * Vector3.left) + (playerAvatar.rotation * Vector3.forward), Color.cyan, 0);

        Debug.DrawRay(playerAvatar.position, rb.velocity.normalized, Color.white, 0, false);
    }

    //This handles pawn movemnt, including outside of player input
    void MovePlayer() {
        float strafe = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        CheckGRND();
        if (Input.GetKeyDown("space") && playerGRND)
        {
            JumpPlayer();
        } else if (Input.GetKeyDown("space") && !playerGRND) {
            CheckWJ();
        }

        /// !!!! TODO: SEPARATE MOVEMENT FOR GROUNDED AND UNGROUNDED PLAYER !!!!
        /// 
        ///--Ground/Airborne checks should be done anyway, at least for animation reasons
        ///--Grounded player doesn't use CAM PITCH for the movement vector, as running into
        ///the ground or upwards doesn't make sense. Airborne player on the other hand might 
        ///need pitched movement for jump/flight control.
        ///


        if (playerGRND == true)
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
        else if (playerGRND == false) {
            //Behavior if player is airborne
            movementVector = camViewDir * (new Vector3(strafe, 0f, forward) * airAccelSpeed * Time.deltaTime);     //Maps forward/strafe controls in relation to PITCHLESS CAM DIR; Results in movement direction vector.
            rb.velocity += movementVector;
            Quaternion rotHelp = new Quaternion();
            rotHelp.eulerAngles = new Vector3(-Mathf.Clamp(moveDir.eulerAngles.x, -20, 20), moveDir.eulerAngles.y, 0);
            playerAvatar.rotation = rotHelp;   //Set Avatar Rotation to match movement vector
        }
        //Debug.Log(rb.velocity.magnitude);
        HandleVelocityFalloff();
    }

    //This function should yeet the player
    void JumpPlayer() {

        rb.AddForce(moveDir * jumpVector, ForceMode.Impulse);
        Debug.Log("Player Jumped!");
    
    }
    void WallJump()
    {

        rb.AddForce(moveDir * new Vector3(0, 3, 7), ForceMode.Impulse);
        Debug.Log("Player Jumped!");

    }

    //This function should cast something under the player to check if he collides with a walkable surface
    void CheckGRND() {
        playerGRND = Physics.SphereCast(playerAvatar.transform.position+new Vector3(0, 0.5f, 0), 0.2f, Vector3.down, out feetHit, 0.4f);
    }

    //This function should check collision with walls and let player jump off of them.
    //IMPORTANT: Should the wall jumping be dependant on movement direction? EG not jumping off of walls you're flying away from?.. 
    //...well actually t. spheres were already supposed to be cast in relation to movDir, so... But what if you rayhit a wall w/ normal that is facing the same direction as moveDir?(How tho..)
    //
    //Would just 4 spherecasts be enough?
    //Things necessary to know for the jump:
    //-Wall touch point normal
    //-How many walls were touched?
    //-In case of multiple walls: closest wall?
    void CheckWJ()
    {
        float rayLength = 1;
        RaycastHit[] totalHits = new RaycastHit[5];
        bool[] checkers = new bool[5];
        RaycastHit contactPoint = new RaycastHit();

        //Check for impacts
        checkers[0] = Physics.Raycast(playerAvatar.position, playerAvatar.rotation * Vector3.forward, out totalHits[0], rayLength);
        checkers[1] = Physics.Raycast(playerAvatar.position, playerAvatar.rotation * Vector3.left, out totalHits[1], rayLength);
        checkers[2] = Physics.Raycast(playerAvatar.position, playerAvatar.rotation * Vector3.right, out totalHits[2], rayLength);
        checkers[3] = Physics.Raycast(playerAvatar.position, (playerAvatar.rotation * Vector3.right) + (playerAvatar.rotation * Vector3.forward), out totalHits[3], rayLength);
        checkers[4] = Physics.Raycast(playerAvatar.position, (playerAvatar.rotation * Vector3.left) + (playerAvatar.rotation * Vector3.forward), out totalHits[4], rayLength);

        //Determine the closest impact point
        float dist = 100;
        for (int i = 0; i < checkers.Length; i++)
        {
            if (checkers[i])
            {
                if (dist > totalHits[i].distance)
                {
                    contactPoint = totalHits[i];
                    dist = totalHits[i].distance;
                }
            }
        }
        //Visualize vectors
        Debug.DrawRay(playerAvatar.position, playerAvatar.rotation * Vector3.forward, Color.blue, 0);
        Debug.DrawRay(playerAvatar.position, playerAvatar.rotation * Vector3.left, Color.green, 0);
        Debug.DrawRay(playerAvatar.position, playerAvatar.rotation * Vector3.right, Color.red, 0);
        Debug.DrawRay(playerAvatar.position, (playerAvatar.rotation * Vector3.right) + (playerAvatar.rotation * Vector3.forward), Color.yellow, 0);
        Debug.DrawRay(playerAvatar.position, (playerAvatar.rotation * Vector3.left) + (playerAvatar.rotation * Vector3.forward), Color.cyan, 0);



        if (checkers[0] || checkers[1] || checkers[2] || checkers[3] || checkers[4])
        {
            Debug.Log("Walljump Digga!");
            Vector3 reflectVelocity = Vector3.Reflect(rb.velocity, contactPoint.normal);
            if (reflectVelocity == rb.velocity)
            {
               reflectVelocity = Quaternion.AngleAxis(-90, Vector3.up) * rb.velocity;
               rb.velocity = reflectVelocity;
               Debug.Log("Reflection equals initial direction!");
            }
            else
            {
                rb.velocity = reflectVelocity;
            }
            WallJump();
            Debug.DrawRay(contactPoint.point, contactPoint.normal, Color.green, 2);
        }

    }
    void HandleVelocityFalloff() {
        if (playerGRND && !Input.GetKeyDown("space")) {
            if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d"))
            {
                Debug.Log("Slowing Down!");
                rb.velocity = new Vector3(rb.velocity.x - rb.velocity.x*Time.deltaTime * groundSlowdown, rb.velocity.y - rb.velocity.y * Time.deltaTime* groundSlowdown, rb.velocity.z - rb.velocity.z * Time.deltaTime * groundSlowdown);
            }
        }

    }

    ///SANITY NOTES
    ///
    ///-- Transform.Translate has a HUGE ISSUE: It does not affect velocity. Meaning that a running doesn't alter trajectory of a jump,
    ///stopping movement midair drops the player like a stone.
    /// 
    ///-- In reality moving objects don't GRADUALLY receive mov. energy unassisted(eg without help of external force/motorisation/mechanical locomotion).
    ///JUMP/THROW/GUNFIRE/LAUNCH energy is received as an IMPULSE.
    ///     -- Does something like "potential/stored energy" make sense?
    ///-- Unity's RIGIDBODY tracks the total velocity of objects by default.
    ///     Pros:
    ///     -- Already built, needs no dev time.
    ///     Cons:
    ///     -- It's subject to INTERNAL systematic PHYSICS CALCULATIONS. 
    ///         Altering that for the purposes of gameplay is very likely to be HARD.
}
