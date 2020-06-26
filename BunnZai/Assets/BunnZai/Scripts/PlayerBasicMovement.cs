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
            float speedToUse = 0;
            float timeToUse = 0;
            if (mPlayer.mIsGrounded)
            {
                speedToUse = mPlayer.mMaxWalkSpeed;
                timeToUse = mPlayer.mWalkTurnTime;
            }
            else
            {
                speedToUse = mPlayer.mRigidBody.velocity.magnitude;
                //speedToUse = mPlayer.mMaxWalkSpeed;
                timeToUse = mPlayer.mStrafeTurnTime;
            }
            //get the angle of desired player movement
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smooth out player rotation towards that angle
            float directionAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turningVelocity, timeToUse);
            //apply smoothed rotation
            transform.rotation = Quaternion.Euler(transform.rotation.x, directionAngle, transform.rotation.z);

            //add movement force to rigidbody
            Vector3 moveDirection = Quaternion.Euler(0f, directionAngle, 0f) * Vector3.forward * speedToUse;

            //potentially implement acceleration instead of instant max speed later on
            mPlayer.mRigidBody.velocity = new Vector3(moveDirection.x, mPlayer.mRigidBody.velocity.y, moveDirection.z);
        }
        else if (mPlayer.mIsGrounded)
        {
            //deceleration if nothing is pressed
            //mPlayer.mRigidRef.velocity *= mPlayer.mDecelerationMultiplier, but in fancy;
            mPlayer.mRigidBody.velocity = new Vector3(mPlayer.mRigidBody.velocity.x * mPlayer.mDecelerationMultiplier, mPlayer.mRigidBody.velocity.y, mPlayer.mRigidBody.velocity.z * mPlayer.mDecelerationMultiplier);
        }
    }

    public void Jump()
    {
        if (mPlayer.mIsGrounded || (!mPlayer.mIsGrounded && !mPlayer.mAerialJumpUsed))
        {
            mPlayer.mRigidBody.velocity = new Vector3(mPlayer.mRigidBody.velocity.x, mPlayer.mJumpVelocity, mPlayer.mRigidBody.velocity.y);
            mPlayer.mRigidBody.velocity = mPlayer.mRigidBody.velocity + (transform.forward * mPlayer.mJumpVelocityForward);
        }
        if (!mPlayer.mIsGrounded)
            mPlayer.mAerialJumpUsed = true;
    }

    public void UpdateVelocities()
    {
        if (mPlayer.mRigidBody.velocity.y < 0)
            mPlayer.mRigidBody.velocity = new Vector3(mPlayer.mRigidBody.velocity.x, mPlayer.mRigidBody.velocity.y * mPlayer.mFallSpeedMultiplier, mPlayer.mRigidBody.velocity.z);

        if (mPlayer.mRigidBody.velocity.y < -mPlayer.mTerminalVelocity)
        {
            mPlayer.mRigidBody.velocity = new Vector3(mPlayer.mRigidBody.velocity.x, -mPlayer.mTerminalVelocity, mPlayer.mRigidBody.velocity.z);
        }

        Vector3 horizontalVelocity = new Vector3(mPlayer.mRigidBody.velocity.x, 0, mPlayer.mRigidBody.velocity.z);
        if (horizontalVelocity.magnitude > mPlayer.mMaxVelocity)
        {
            horizontalVelocity = horizontalVelocity.normalized * mPlayer.mMaxVelocity;
            mPlayer.mRigidBody.velocity = new Vector3(horizontalVelocity.x, mPlayer.mRigidBody.velocity.y, horizontalVelocity.z);
        }
    }
}