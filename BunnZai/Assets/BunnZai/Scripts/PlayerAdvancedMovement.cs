using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerAdvancedMovement : MonoBehaviour
{
    Player mPlayer;
  
    private void Awake()
    {
        mPlayer = GetComponent<Player>();
    }

    public void Dash()
    {
        mPlayer.mDashTimer = mPlayer.mDashCooldown;
        mPlayer.mIsDashing = true;
        //mPlayer.mRigidBody.velocity = mPlayer.mDirection * new Vector3(1, 1, mPlayer.mDashSpeed);
        mPlayer.mRigidBody.AddForce(mPlayer.mDirection* new Vector3(1, 1, mPlayer.mDashSpeed),ForceMode.Impulse); //Ground collision hinders dash
    }

    public void EndDash()
    {
        mPlayer.mIsDashing = false;
        mPlayer.mRigidBody.velocity = mPlayer.mRigidBody.velocity.normalized;
    }

    public void Wallrun()
    {
        RaycastHit hitInfoRight;
        RaycastHit hitInfoLeft;
        bool isRight = Physics.Raycast(transform.position, mPlayer.transform.TransformDirection(Vector3.right), out hitInfoRight, 2f);
        bool isLeft = Physics.Raycast(transform.position, mPlayer.transform.TransformDirection(Vector3.left), out hitInfoLeft, 2f);
        mPlayer.mIsWallRunning = true;
       
        if (mPlayer.mWallRunAvailable)
        {
            if (isRight && hitInfoRight.collider.CompareTag("Wall"))
            {
                // mPlayer.mWallRunAvailable = false;  <- if only need to animate once
                // Animate right
            }

            if (isLeft && hitInfoLeft.collider.CompareTag("Wall"))
            {
                // mPlayer.mWallRunAvailable = false; <- if only need to animate once
                // Animate left
            }
        }

        //if(velocity > wallrunvelocity)
        //mPlayer.mRigidBody.velocity *= 0.995f;
    }

    public void EndWallrun()
    {
        //if(mPlayer.mIsWallRunning)
            //mPlayer.transform.rotation = new Quaternion(mPlayer.transform.rotation.x, mPlayer.transform.rotation.y, 0, 1);
        mPlayer.mWallRunAvailable = true;
        mPlayer.mIsWallRunning = false;
        
    }

    public void WallJump()
    {
        mPlayer.mRigidBody.velocity = mPlayer.mDirection * new Vector3(1, 1,mPlayer.mWallJumpSpeed);
    }

    public void HandleVelocityFalloff()
    {
        if (mPlayer.mIsGrounded && !Input.GetKeyDown("space"))
        {
            if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d"))
                mPlayer.mRigidBody.velocity *= 0.9f;
        }
        //new Vector3(rb.velocity.x - rb.velocity.x * Time.deltaTime * groundSlowdown, rb.velocity.y - rb.velocity.y * Time.deltaTime * groundSlowdown, rb.velocity.z - rb.velocity.z * Time.deltaTime * groundSlowdown);
    }
}
