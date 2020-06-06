using System.Collections;
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
    CapsuleCollider mPlayerCollider;
    BoxCollider mGroundCheckCollider;
    Rigidbody mRidigRef;

    [Header("Movement Parameters")]
    [SerializeField] bool mIsGrounded;
    [SerializeField] bool mIsWalled;

    [Header("Movement Variables")]
    [SerializeField] float mMaxWalkSpeed;
    [SerializeField] float mAcceleration;
    [SerializeField] float mMaxVelocity;

    [Header("Private Variables")]
    float mForwardAxisDelta;
    float mSidewaysAxisDelta;

    //pre-start
    void Awake()
    {
        mBasicMovement = GetComponent<PlayerBasicMovement>();
        mAdvancedMovement = GetComponent<PlayerAdvancedMovement>();
        mCameraController = GetComponent<PlayerCameraController>();
        mCombatController = GetComponent<PlayerCombatController>();

        mGroundCheckCollider = GetComponent<BoxCollider>();
    }

    //50x/s (default value)
    void FixedUpdate()
    {
        //e.g. mBasicMovement.Jump();

        UpdateInputValues();
        CheckGroundState();


        //moving walking movement to end to slightly improve coyote time
        mBasicMovement.AddMovementInput();
    }

    //update axis deltas based on movement input
    void UpdateInputValues()
    {
        mForwardAxisDelta = Input.GetAxis("ForwardsAxis");
        mSidewaysAxisDelta = Input.GetAxis("SidewaysAxis");
    }

    //check ground state using raycast
    bool CheckGroundState()
    {
        //300 is placeholder for distance
        //alternatively add layer mask
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        return Physics.Raycast(transform.position, Vector3.down, 300);
        //return Collider.cast
    }

    private void OnTriggerEnter(Collider other)
    {
        mIsGrounded = true;
    }
    private void OnTriggerExit(Collider other)
    {
        mIsGrounded = false;
    }
}
