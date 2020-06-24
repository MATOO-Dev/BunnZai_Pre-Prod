using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    public Player mPlayer;
    private void Start()
    {
        mPlayer = this.gameObject.GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        mPlayer.mFloorAmount++;
        mPlayer.mIsGrounded = true;
        mPlayer.mAerialJumpUsed = false;
    }

    private void OnTriggerExit(Collider other)
    {
        mPlayer.mFloorAmount--;
        if (mPlayer.mFloorAmount == 0)
            mPlayer.mIsGrounded = false;
    }
}
