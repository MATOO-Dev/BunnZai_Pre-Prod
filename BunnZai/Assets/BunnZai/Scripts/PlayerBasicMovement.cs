using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Jumptype
{
    AddForce,
    SetVelocity
}

public class PlayerBasicMovement : MonoBehaviour
{
    Player mPlayer;
    float turningVelocity;
    float aerialTurningVelocity;
    //change to getcomponent with cam/cinemachine
    //add script to cam to get player reference
    public Transform cam;
    public Jumptype JType = Jumptype.AddForce;

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
            float speedToUse = 0;
            float timeToUse = 0;
            if (mPlayer.mIsGrounded)
            {
                speedToUse = mPlayer.mMaxWalkSpeed;
                timeToUse = mPlayer.mWalkTurnTime;
            }
            else
            {
                speedToUse = mPlayer.mRigidRef.velocity.magnitude;
                timeToUse = mPlayer.mStrafeTurnTime;
            }
            //get the angle of desired player movement
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smooth out player rotation towards that angle
            float directionAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turningVelocity, timeToUse);
            //apply smoothed rotation
            transform.rotation = Quaternion.Euler(0f, directionAngle, 0f);

            //add movement force to rigidbody
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * speedToUse;

            if (mPlayer.mRigidRef.velocity.magnitude < mPlayer.mMaxWalkSpeed) //could be used later for acceleration instead of instant max speed
                mPlayer.mRigidRef.velocity = new Vector3(moveDirection.x, mPlayer.mRigidRef.velocity.y, moveDirection.z);
        }
        else
        {
            //deceleration if nothing is pressed
            //mPlayer.mRigidRef.velocity *= mPlayer.mDecelerationMultiplier, but in fancy;
            mPlayer.mRigidRef.velocity = new Vector3(mPlayer.mRigidRef.velocity.x * mPlayer.mDecelerationMultiplier, mPlayer.mRigidRef.velocity.y, mPlayer.mRigidRef.velocity.z * mPlayer.mDecelerationMultiplier);
        }
    }

    public void Jump()
    {
        if (JType == Jumptype.AddForce)
            mPlayer.mRigidRef.AddForce(Vector3.up * mPlayer.mJumpForce);
        else
            mPlayer.mRigidRef.velocity = new Vector3(mPlayer.mRigidRef.velocity.x, mPlayer.mJumpVelocity, mPlayer.mRigidRef.velocity.y);
    }

    //could be integrated into jump() when refactoring
    public void DoubleJump()
    {
        if (JType == Jumptype.AddForce)
            mPlayer.mRigidRef.AddForce(Vector3.up * mPlayer.mJumpForce);
        else
            mPlayer.mRigidRef.velocity = new Vector3(mPlayer.mRigidRef.velocity.x, mPlayer.mJumpVelocity, mPlayer.mRigidRef.velocity.y);
        mPlayer.mAerialJumpUsed = true;
    }
}