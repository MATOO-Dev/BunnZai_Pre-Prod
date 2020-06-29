using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallCheck : MonoBehaviour
{
    Player mPlayer;
    private void Start()
    {
        mPlayer = this.gameObject.GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
            mPlayer.mIsWalled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall") && mPlayer.mIsWalled)
            mPlayer.mIsWalled = false;
    }
}
