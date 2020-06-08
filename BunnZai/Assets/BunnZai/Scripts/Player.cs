﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //variables
    [Header("Linked Scripts")]
    PlayerBasicMovement mBasicMovement;
    PlayerAdvancedMovement mAdvancedMovement;
    PlayerCameraController mCameraController;
    PlayerCombatController mCombatController;

    [Header("Components")]
    [HideInInspector] public CapsuleCollider mPlayerCollider;
    [HideInInspector] public BoxCollider mGroundCheckCollider;
    [HideInInspector] public Rigidbody mRigidRef;

    [Header("Movement Parameters")]
    [SerializeField] bool mIsGrounded;
    [SerializeField] bool mIsWalled;
    [SerializeField] bool mAerialJumpUsed;

    [Header("Movement Variables")]
    public float mMaxWalkSpeed;
    public float mAcceleration;
    public float mMaxVelocity;
    public float mTurnTime = 0.1f;
    public float mJumpForce; //maybe rename to jumpheight instead? base on implementation

    [Header("Private Variables")]
    [HideInInspector] public float mForwardAxisDelta;
    [HideInInspector] public float mSidewaysAxisDelta;


    void Awake() //pre-start 
    {
        //get linked scripts
        mBasicMovement = GetComponent<PlayerBasicMovement>();
        mAdvancedMovement = GetComponent<PlayerAdvancedMovement>();
        mCameraController = GetComponent<PlayerCameraController>();
        mCombatController = GetComponent<PlayerCombatController>();

        //get components
        mPlayerCollider = GetComponent<CapsuleCollider>();
        mGroundCheckCollider = GetComponent<BoxCollider>();
        mRigidRef = GetComponent<Rigidbody>();
    }

    private void Update() //every frame, use for graphics/input
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
        UpdateInputValues();
    }

    void FixedUpdate() //50x/s (default value), use for physics
    {

        //moving walking movement to end to slightly improve coyote time
        mBasicMovement.AddMovementInput();
    }

    void UpdateInputValues() //update axis deltas based on movement input 
    {
        mForwardAxisDelta = Input.GetAxisRaw("ForwardsAxis");
        mSidewaysAxisDelta = Input.GetAxisRaw("SidewaysAxis");
    }

    public Vector3 GetInpitValues() //get vector3 with current input values 
    {
        return new Vector3(mSidewaysAxisDelta, 0, mForwardAxisDelta);
    }

    //check ground state using trigger
    private void OnTriggerEnter(Collider other)
    {
        mIsGrounded = true;
    }
    private void OnTriggerExit(Collider other)
    {
        mIsGrounded = false;
    }
}
