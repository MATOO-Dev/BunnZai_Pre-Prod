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
        mPlayer.mIsWallRunning = true;
        RaycastHit hitInfoRight;
        RaycastHit hitInfoLeft;
        if (Physics.Raycast(transform.position, mPlayer.transform.TransformDirection(Vector3.right), out hitInfoRight, 1f) &&
            hitInfoRight.collider.CompareTag("Wall") && mPlayer.mWallRunAvailable)
        {
            mPlayer.mWallRunAvailable = false;
            //mPlayer.transform.Rotate(0, 0, 90);
            transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, 0, 90), Time.deltaTime);
        }

        if (Physics.Raycast(transform.position, mPlayer.transform.TransformDirection(Vector3.left), out hitInfoLeft, 1f) &&
            hitInfoLeft.collider.CompareTag("Wall") && mPlayer.mWallRunAvailable)
        {
            mPlayer.mWallRunAvailable = false;
            //mPlayer.transform.Rotate(0, 0, -90);
        }
        //if(velocity > wallrunvelocity)
        //mPlayer.mRigidBody.velocity *= 0.995f;
    }

    public void EndWallrun()
    {
        mPlayer.mWallRunAvailable = true;
        mPlayer.mIsWallRunning = false;
        //mPlayer.transform.rotation = new Quaternion(mPlayer.transform.rotation.x, mPlayer.transform.rotation.y, 0, 1);
    }

    public void WallJump()
    {
        mPlayer.mRigidBody.velocity = mPlayer.mDirection * new Vector3(1, 1, 15);
        mPlayer.mIsWallRunning = false;
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
