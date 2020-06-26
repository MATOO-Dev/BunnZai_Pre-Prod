using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallCheck : MonoBehaviour
{
    public Player mPlayer;
    private void Start()
    {
        mPlayer = this.gameObject.GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        mPlayer.mIsWalled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        mPlayer.mIsWalled = false;
    }
}
