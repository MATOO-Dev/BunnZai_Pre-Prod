using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerAdvancedMovement : MonoBehaviour
{
    Player mPlayer;
    private float mGravityScale;

    private void Awake()
    {
        mPlayer = GetComponent<Player>();
    }

    private void Start()
    {
        //mPlayer.transform.Rotate(0, 0, 90);
    }

    public void Dash()
    {
        mPlayer.mDashTimer = mPlayer.mDashCooldown;
        mPlayer.mIsDashing = true;
        mPlayer.mRigidBody.velocity += mPlayer.mDirection * new Vector3(1, 1, 20);
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
        bool isRight = Physics.Raycast(transform.position, mPlayer.transform.TransformDirection(Vector3.right), out hitInfoRight, 1f);
        bool isLeft = Physics.Raycast(transform.position, mPlayer.transform.TransformDirection(Vector3.left), out hitInfoLeft, 1f);
        mPlayer.mIsWallRunning = true;
        if (mPlayer.mWallRunAvailable)
        {
            mPlayer.mRigidBody.velocity = new Vector3(mPlayer.mRigidBody.velocity.x, 0, mPlayer.mRigidBody.velocity.z);

            if (isRight && hitInfoRight.collider.CompareTag("Wall"))
            {
                mPlayer.mWallRunAvailable = false;
                mPlayer.transform.Rotate(0, 0, 10);  //Collider Problem
            }

            if (isLeft && hitInfoLeft.collider.CompareTag("Wall"))
            {
                mPlayer.mWallRunAvailable = false;
                mPlayer.transform.Rotate(0, 0, -10);
            }
        }

        //if(velocity > wallrunvelocity)
        //mPlayer.mRigidBody.velocity *= 0.995f;
    }

    public void EndWallrun()
    {
        mPlayer.mWallRunAvailable = true;
        mPlayer.mIsWallRunning = false;
        mPlayer.transform.rotation = new Quaternion(mPlayer.transform.rotation.x, mPlayer.transform.rotation.y, 0, 1);
    }

    public void WallJump()
    {
        mPlayer.mRigidBody.velocity = mPlayer.mDirection * new Vector3(1, 1, 30);
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
