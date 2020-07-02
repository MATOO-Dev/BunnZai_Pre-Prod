using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //variables
    [Header("Linked Scripts")]
    PlayerBasicMovement mBasicMovement;
    PlayerAdvancedMovement mAdvancedMovement;
    PlayerCombatController mCombatController;
    PlayerAnimationController mAnimationController;
    PlayerCameraController mCameraController;

    [Header("Components")]
    [HideInInspector] public CapsuleCollider mPlayerCollider;
    [HideInInspector] public BoxCollider mGroundCheckCollider;
    [HideInInspector] public Rigidbody mRigidBody;
    [HideInInspector] public Camera mCamera;

    [Header("Movement Parameters")]
    public bool mIsGrounded;
    public bool mIsWalled;
    public bool mIsWallRunning;
    public bool mWallRunAvailable = true;
    public bool mIsDashing;
    public bool mAerialJumpUsed;

    [Header("Movement Variables")]
    public float mMaxWalkSpeed;             //max walking speed
    public float mWalkAcceleration;         //walk acceleration
    public float mDecelerationMultiplier;   //used for breaking
    public float mWalkTurnTime;             //time to turn when moving
    public float mStrafeTurnTime;           //time to turn when strafing
    public float mVelocityBuildup;          //multiplier for velocity buildup
    public float mMaxVelocity;              //horizontal terminal velocity
    public float mJumpHeight;               //targeted jump height
    public float mJumpDistanceForward;      //targeted forwards distance of jump
    public float mFallSpeedMultiplier;      //multiplier for falling acceleration
    public float mTerminalVelocity;         //terminal velocity
    public float mDashCooldown;             //dash cooldown, starts at beginning of dash (aka end of dash remaining cooldown = cooldown-duration)
    public float mDashDuration;             //dash duration
    public float mDashSpeed;
    public float mWallRunGravity;           //gravity multiplier during wallrun
    public float mWallJumpSpeed;
    public float mWallJumpHeight;
    [HideInInspector] public Quaternion mWallJumpDir;


    [Header("Local Variables")]
    [HideInInspector] public float mDashTimer;                //dash timer
    [HideInInspector] public float mForwardAxisDelta;
    [HideInInspector] public float mSidewaysAxisDelta;

    [Header("Debug Variables")]
    int placeholder;

    void Awake() //pre-start 
    {
        //get components
        mPlayerCollider = GetComponent<CapsuleCollider>();
        mGroundCheckCollider = GetComponent<BoxCollider>();
        mRigidBody = GetComponent<Rigidbody>();
        mCamera = this.gameObject.GetComponentInChildren<Camera>();

        //get linked scripts
        mBasicMovement = GetComponent<PlayerBasicMovement>();
        mAdvancedMovement = GetComponent<PlayerAdvancedMovement>();
        mCombatController = GetComponent<PlayerCombatController>();
        mAnimationController = GetComponent<PlayerAnimationController>();
        mCameraController = GetComponent<PlayerCameraController>();
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
            {
                mRigidBody.useGravity = false;
                mAdvancedMovement.Dash();
            }
        }
    }

    void FixedUpdate() //50x/s (default value), use for physics
    {
        if (mIsWalled && !mIsGrounded)
        {
            mAdvancedMovement.Wallrun();
        }
        if ((!mIsWalled || mIsGrounded) && mIsWallRunning)
            mAdvancedMovement.EndWallrun();

        if (mDashTimer > 0)
            mDashTimer -= Time.deltaTime;
        if (mDashTimer <= (mDashCooldown - mDashDuration) && mIsDashing)
        {
            mRigidBody.useGravity = true;
            mAdvancedMovement.EndDash();
        }
        if (!mIsWalled)
            mAdvancedMovement.mWallJumpUsable = true;

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
        if (mRigidBody.velocity.y <= 0 && mIsWalled)
            mRigidBody.AddForce(Physics.gravity * (mWallRunGravity - 1));
    }
}
