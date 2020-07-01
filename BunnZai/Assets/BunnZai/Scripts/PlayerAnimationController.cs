using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Player mPlayer;
    Animator mAnimator;

    public void Awake()
    {
        mPlayer = GetComponent<Player>();
        mAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //mAnimator.SetFloat("name", value);
        mAnimator.SetFloat("velocityOverall", mPlayer.mRigidBody.velocity.magnitude);
        mAnimator.SetFloat("velocityHorizontal", new Vector3(mPlayer.mRigidBody.velocity.x, 0, mPlayer.mRigidBody.velocity.z).magnitude);
        mAnimator.SetFloat("velocityVertical", mPlayer.mRigidBody.velocity.y);
        mAnimator.SetFloat("forwardAxisDelta", mPlayer.mForwardAxisDelta);
        mAnimator.SetFloat("sidewaysAxisDelta", mPlayer.mSidewaysAxisDelta);
        mAnimator.SetBool("isGrounded", mPlayer.mIsGrounded);
        mAnimator.SetBool("aerialJumpUsed", mPlayer.mAerialJumpUsed);
        mAnimator.SetBool("jumpPressed", Input.GetButtonDown("Jump"));
    }
}
