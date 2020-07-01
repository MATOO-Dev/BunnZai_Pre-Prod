using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Player mPlayer;

    public void Awake()
    {
        mPlayer = GetComponent<Player>();
    }
}
