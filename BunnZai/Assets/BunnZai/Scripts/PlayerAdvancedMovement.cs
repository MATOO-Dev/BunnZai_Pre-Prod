using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerAdvancedMovement : MonoBehaviour
{
    Player mPlayer;
    private float mWallJumpCooldown;
    private float mWallJumpTimer;
    public bool mWallJumpUsable = true;
    private Vector3 mSpeedBeforeDash;

    private Vector3 normal;
    private void Awake()
    {
        mPlayer = GetComponent<Player>();
    }

    public void Dash()
    {
        mSpeedBeforeDash = mPlayer.mRigidBody.velocity;
        mPlayer.mDashTimer = mPlayer.mDashCooldown;
        mPlayer.mIsDashing = true;
        mPlayer.mRigidBody.velocity += mPlayer.mDirection * new Vector3(0, 0, mPlayer.mDashSpeed);
    }

    public void EndDash()
    {
        mPlayer.mIsDashing = false;
        mPlayer.mRigidBody.velocity = mSpeedBeforeDash;
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
            mPlayer.mWallRunAvailable = false;
            if (mPlayer.mRigidBody.velocity.y < 0)
                mPlayer.mRigidBody.velocity = new Vector3(mPlayer.mRigidBody.velocity.x, 0, mPlayer.mRigidBody.velocity.z);
            if (isRight && hitInfoRight.collider.CompareTag("Wall"))
            {
                normal = hitInfoRight.normal;
                if (Input.GetKeyDown(KeyCode.A))
                    mPlayer.mWallJumpDir.eulerAngles = new Vector3(normal.x + 10, normal.y, normal.z);
                if (Input.GetKeyDown(KeyCode.D))
                    mPlayer.mWallJumpDir.eulerAngles = new Vector3(normal.x - 10, normal.y, normal.z);

                // mPlayer.mWallRunAvailable = false;  <- if only need to animate once
                // Animate right
            }
            if (isLeft && hitInfoLeft.collider.CompareTag("Wall"))
            {
                normal = hitInfoLeft.normal;
                if (Input.GetKeyDown(KeyCode.A))
                    mPlayer.mWallJumpDir.eulerAngles = new Vector3(normal.x - 10, normal.y, normal.z);
                if (Input.GetKeyDown(KeyCode.D))
                    mPlayer.mWallJumpDir.eulerAngles = new Vector3(normal.x + 10, normal.y, normal.z);
                // mPlayer.mWallRunAvailable = false; <- if only need to animate once
                // Animate left
            }
            mPlayer.mWallJumpDir = Quaternion.Euler(normal);
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
        if (mWallJumpUsable)
        {
            mWallJumpUsable = false;
            mWallJumpTimer = mWallJumpCooldown;
            mPlayer.mRigidBody.velocity += mPlayer.mDirection * new Vector3(0, 0, mPlayer.mWallJumpSpeed);
            mPlayer.mRigidBody.velocity = new Vector3(mPlayer.mRigidBody.velocity.x, mPlayer.mRigidBody.velocity.y + mPlayer.mWallJumpHeight + mPlayer.mRigidBody.velocity.z);
        }
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
