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
    [HideInInspector] public CapsuleCollider mPlayerCollider;
    [HideInInspector] public BoxCollider mGroundCheckCollider;
    [HideInInspector] public Rigidbody mRigidBody;

    [Header("Movement Parameters")]
    public bool mIsGrounded;
    public bool mIsWalled;
    public bool mIsWallRunning;
    public bool mWallRunAvailable;
    public bool mIsDashing;
    public bool mAerialJumpUsed;

    [Header("Movement Variables")]
    public float mMaxWalkSpeed;             //max walking speed
    public float mWalkAcceleration;         //walk acceleration
    public float mDecelerationMultiplier;   //used for breaking
    public float mWalkTurnTime;             //time to turn when moving
    public float mStrafeTurnTime;           //time to turn when strafing
    public float mMaxVelocity;              //~ horizontal terminal velocity
    public float mJumpVelocity;             //velocity applied when jumping
    public float mJumpVelocityForward;      //velocity applied forward when jumping
    public float mFallSpeedMultiplier;      //multiplier for falling acceleration
    public float mTerminalVelocity;         //terminal velocity
    public float mDashCooldown;             //dash cooldown, starts at beginning of dash (aka end of dash remaining cooldown = cooldown-duration)
    public float mDashDuration;             //dash duration
    public float mWallRunGravity;           //gravity multiplier during wallrun


    [Header("Local Variables")]
    public float mDashTimer;                //dash timer
    public Quaternion mDirection;
    [HideInInspector] public float mForwardAxisDelta;
    [HideInInspector] public float mSidewaysAxisDelta;

    [Header("Debug Variables")]
    int placeholder;

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
        mRigidBody = GetComponent<Rigidbody>();
    }

    private void Update() //every frame (fps dependant), use for graphics/input
    {
        UpdateInputValues();
        //call jump functions
        if (Input.GetButtonDown("Jump"))
        {
            if (!mIsWallRunning)
                mBasicMovement.Jump();
            else
                mAdvancedMovement.WallJump();
        }
        if (Input.GetButtonDown("Dash"))
        {
            if (mDashTimer <= 0)
                mAdvancedMovement.Dash();
        }
    }

    void FixedUpdate() //50x/s (default value), use for physics
    {
        if (mIsWalled && !mIsGrounded)
        {
            Debug.Log("attempting wallrun");
            mAdvancedMovement.Wallrun();
        }
        if ((!mIsWalled || mIsGrounded) && mIsWallRunning)
            mAdvancedMovement.EndWallrun();

        if (mDashTimer > 0)
            mDashTimer -= Time.deltaTime;
        if (mDashTimer <= (mDashCooldown - mDashDuration) && mIsDashing)
            mAdvancedMovement.EndDash();


        mDirection = transform.rotation;
        mBasicMovement.AddMovementInput();
        mBasicMovement.UpdateVelocities();
        UpdateExternalForces();
    }

    void UpdateInputValues() //update axis deltas based on movement input 
    {
        mForwardAxisDelta = Input.GetAxis("ForwardsAxis");
        mSidewaysAxisDelta = Input.GetAxis("SidewaysAxis");
    }

    public Vector3 GetInpitValues() //get vector3 with current input values 
    {
        return new Vector3(mSidewaysAxisDelta, 0f, mForwardAxisDelta);
    }

    public void UpdateExternalForces()
    {
        if (mRigidBody.velocity.y < 0 && mIsWalled)
            mRigidBody.AddForce(Physics.gravity * (mWallRunGravity - 1));
    }
}
