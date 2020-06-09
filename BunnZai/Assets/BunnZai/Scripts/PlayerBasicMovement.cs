using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicMovement : MonoBehaviour
{
    Player mPlayer;
    float turningVelocity;
    //change to getcomponent with cam/cinemachine
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
            float directionAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smooth out player rotation towards that angle
            float smoothedRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, directionAngle, ref turningVelocity, mPlayer.mTurnTime);
            //apply smoothed rotation
            transform.rotation = Quaternion.Euler(0, smoothedRotation, 0);

            //add movement force to rigidbody
            Vector3 moveDirection = Quaternion.Euler(0, directionAngle, 0) * Vector3.forward;
            if (mPlayer.mRigidRef.velocity.magnitude < mPlayer.mMaxWalkSpeed)
                mPlayer.mRigidRef.velocity = moveDirection * mPlayer.mMaxWalkSpeed;
            //known issue: moving backwards spazzes the player out, because they keep rotating every tick, so backward direction changes every tick
        }
        else
        {
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