using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicMovement : MonoBehaviour
{
    Player mPlayer;
    float turningVelocity;
    //change to getcomponent with cam/cinemachine
    //add script to cam to get player reference
    public Transform cam;

    public void Awake()
    {
        mPlayer = GetComponent<Player>();
    }

    public void AddMovementInput()
    {

        //get normalized direction based on input
        Vector3 inputDirection = mPlayer.GetInpitValues().normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            //get the angle of player movement
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smooth out player rotation towards that angle
            float directionAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turningVelocity, mPlayer.mTurnTime);
            //apply smoothed rotation
            transform.rotation = Quaternion.Euler(0f, directionAngle, 0f);

            //add movement force to rigidbody
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (mPlayer.mRigidRef.velocity.magnitude < mPlayer.mMaxWalkSpeed)
                mPlayer.mRigidRef.velocity = moveDirection.normalized * mPlayer.mMaxWalkSpeed;
        }
        else
        {
            //deceleration if nothing is pressed
            mPlayer.mRigidRef.velocity *= mPlayer.mDecelerationMultiplier;
        }
    }

    public void Jump()
    {

    }

    public void DoubleJump()
    {

    }

}