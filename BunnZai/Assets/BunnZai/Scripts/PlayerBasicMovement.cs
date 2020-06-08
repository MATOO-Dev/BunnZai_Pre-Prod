using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicMovement : MonoBehaviour
{
    Player mPlayer;
    float turningVelocity;

    public void Awake()
    {
        mPlayer = GetComponent<Player>();
    }

    public void AddMovementInput()
    {
        //get normalized direction based on input
        Vector3 movementDirection = mPlayer.GetInpitValues().normalized;

        if (movementDirection.magnitude >= 0.1f)
        {
            //get the angle of player movement
            float directionAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
            //smooth out player rotation towards that angle
            float smoothedRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, directionAngle, ref turningVelocity, mPlayer.mTurnTime);
            //apply smoothed rotation
            transform.rotation = Quaternion.Euler(0, smoothedRotation, 0);
            //add movement force to rigidbody
            mPlayer.mRigidRef.velocity = movementDirection * mPlayer.mMaxWalkSpeed;
        }
    }

    public void Jump()
    {

    }

    public void DoubleJump()
    {

    }

}