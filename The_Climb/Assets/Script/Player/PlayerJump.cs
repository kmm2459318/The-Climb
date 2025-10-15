using UnityEngine;

public class PlayerJump : MonoBehaviour
{
<<<<<<< HEAD
    // ==========================================================
    // QÆ
    // ==========================================================
    private Rigidbody RigidBody;
    private PlayerState state;
    private PlayerMove move;
    private PlayerSpecialAction special;
    private PlayerAnimation playerAnimation;

    // ==========================================================
    // ƒWƒƒƒ“ƒvİ’èiInspector•\¦j
    // ==========================================================
    [Header("ƒWƒƒƒ“ƒvƒpƒ[")]
    public float groundJumpPower = 11f;
    public float landingLowJumpPower = 13f;
    public float landingHighJumpPower = 15f;
    public float maxJumpSpeed = 12f;
    [SerializeField] private AnimationCurve jumpCurve = new AnimationCurve();
=======
    Rigidbody RigidBody;
    PlayerState state;
    PlayerMove move;
    PlayerSpecialAction special;
    PlayerKnockBack knock;
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995

    // ==========================================================
    // ƒWƒƒƒ“ƒvó‘ÔiInspector•\¦j
    // ==========================================================
    [Header("ƒWƒƒƒ“ƒvó‘Ô")]
    public bool jumping = false;
    public int landingJumpNumber = 0;
    public bool jumpCoolActive = false;

    // ==========================================================
    // ƒWƒƒƒ“ƒv“à•”—piInspector”ñ•\¦j
    // ==========================================================
    private float jumpTime;
    private float coyoteTime = 0.13f;
    public float coyoteCounter = 0f;
    private float jumpCoolTime = 0.2f;
    private float jumpCoolCounter = 0f;
    private bool isJumpQueued = false;
    private float jumpQueueTime = 0.2f;
    private float jumpQueueCounter = 0f;
    private float jumpTimeMax = 0.2f;
    private float jumpTimeMaxSaving = 0.2f;

    // ==========================================================
    // ƒgƒ‰ƒ“ƒ|ƒŠƒ“ŠÖ˜A
    // ==========================================================
    [Header("ƒgƒ‰ƒ“ƒ|ƒŠƒ“")]
    public bool isOnTrampoline = false;
    public float TrampolinePower = 1.5f;
    private bool TrampolineJumping = false;
    private float TrampolineGraceTime = 0.15f;
    private float TrampolineTimer = 0f;

<<<<<<< HEAD
    // ==========================================================
    // ƒCƒxƒ“ƒg
    // ==========================================================
    public event Action OnJumped;

    // ==========================================================
    // ‰Šú‰»
    // ==========================================================
=======
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
<<<<<<< HEAD
        move = GetComponent<PlayerMove>();
        special = GetComponent<PlayerSpecialAction>();
        playerAnimation = GetComponent<PlayerAnimation>();
=======
        move = gameObject.GetComponent<PlayerMove>();
        special = gameObject.GetComponent<PlayerSpecialAction>();
        knock = gameObject.GetComponent<PlayerKnockBack>();
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
    }

    // ==========================================================
    // “ü—ÍEƒWƒƒƒ“ƒvˆ—
    // ==========================================================
    void Update()
    {
<<<<<<< HEAD
        JumpOperation();
=======
        //ã‚¸ãƒ£ãƒ³ãƒ—ã‚­ãƒ¼æ“ä½œ
        if (!knock.knockBacking)ã€€//ãƒãƒƒã‚¯ãƒãƒƒã‚¯ä¸­ã¯ä¸å¯
        {
            JumpOperation();
        }
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995

        // ƒWƒƒƒ“ƒvƒN[ƒ‹ƒ^ƒCƒ€ŠÇ—
        if (jumpCoolActive)
        {
            jumpCoolCounter += Time.deltaTime;
            state.isGrounded = false;
            state.isJumpMoveOK = false;

            if (jumpCoolCounter > jumpCoolTime)
            {
                jumpCoolActive = false;
                jumpCoolCounter = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (jumping)
        {
            jumpTime += Time.fixedDeltaTime;
            float JumpPower = landingJumpNumber >= 2 ? landingHighJumpPower :
                              landingJumpNumber == 1 ? landingLowJumpPower :
                              groundJumpPower;

            if (isOnTrampoline)
            {
                TrampolineJumping = true;
                TrampolineTimer = TrampolineGraceTime;
            }

            Jump(JumpPower);
        }

        // ƒgƒ‰ƒ“ƒ|ƒŠƒ“Œø‰Ê‚ÌŠÇ—
        if (TrampolineJumping)
        {
            TrampolineTimer -= Time.fixedDeltaTime;
            if (TrampolineTimer <= 0)
            {
                TrampolineJumping = false;
            }
        }
    }

    // ==========================================================
    // ƒWƒƒƒ“ƒv‘€ì”»’è
    // ==========================================================
    private void JumpOperation()
    {
<<<<<<< HEAD
        if (Input.GetKeyDown(state.keyBind.playerJump) && !special.meteorHighJumpOK && !isJumpQueued)
=======
        //ã‚¸ãƒ£ãƒ³ãƒ—ã‚­ãƒ¼æŠ¼ã•ã‚ŒãŸ
        if (state.inputManager.jumpDown && !special.meteorHighJumpOK && !isJumpQueued)
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
        {
            isJumpQueued = true;
            jumpQueueCounter = 0f;
        }

        if ((coyoteCounter <= coyoteTime || state.isJumpMoveOK) && !jumpCoolActive && special.highJumpChargeCounter < special.highJumpChargeTime)
        {
            if (isJumpQueued)
            {
                jumping = true;
                jumpCoolActive = true;
                jumpTime = 0f;
                jumpTimeMax = jumpTimeMaxSaving;
                isJumpQueued = false;
<<<<<<< HEAD
                OnJumped?.Invoke();
=======
                //Debug.Log(RigidBody.linearVelocity.y);
                //Debug.Log("trueå¾Œ"+special.headingAttack);
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995

                if (state.landingJumpOn)
                {
<<<<<<< HEAD
                    state.LandingJumpReset();
                }
            }
            else if (Input.GetKey(state.keyBind.playerJump) && special.meteorHighJumpOK && state.landingJumpOn)
=======
                    landingJumpNumber++;
                    state.LandingJumpReset();
                }
            }
            else if (state.inputManager.jumpHeld && special.meteorHighJumpOK && state.landingJumpOn)  //ãƒ¡ãƒ†ã‚ªãƒ‰ãƒ­ãƒƒãƒ—ã‹ã‚‰ã®ãƒã‚¤ã‚¸ãƒ£ãƒ³ãƒ—
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
            {
                if (special.meteorDropCounter >= special.meteorDropTime)
                {
                    jumpCoolActive = true;
                    special.meteorHighJump = true;
                    landingJumpNumber++;
<<<<<<< HEAD
                    OnJumped?.Invoke();
=======
                    special.headingAttack.SetActive(true);
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
                }
                special.meteorHighJumpOK = false;
                state.LandingJumpReset();
            }
        }

        if (jumping)
        {
<<<<<<< HEAD
            if (Input.GetKeyUp(state.keyBind.playerJump) && jumpTime <= jumpTimeMaxSaving * 0.5f)
=======
            if (state.inputManager.jumpUp && jumpTime <= jumpTimeMaxSaving * 1 / 2)
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
            {
                jumpTimeMax = jumpTimeMaxSaving * 0.5f;
            }
        }

        if (isJumpQueued)
        {
            jumpQueueCounter += Time.deltaTime;
            if (jumpQueueCounter > jumpQueueTime)
            {
                isJumpQueued = false;
            }
        }
    }

    // ==========================================================
    // ƒWƒƒƒ“ƒvÀs
    // ==========================================================
    public void Jump(float jumpPower)
    {
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, RigidBody.linearVelocity.z);

<<<<<<< HEAD
        if (TrampolineJumping)
=======
        //ãƒˆãƒ©ãƒ³ãƒãƒªãƒ³ã«ä¹—ã£ã¦ã„ã‚‹å ´åˆã‚¸ãƒ£ãƒ³ãƒ—åŠ›ã‚’ä¸Šã’ã‚‹
        if(TrampolineJumping)
        {
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
            jumpPower *= TrampolinePower;

        float time = jumpTime / jumpTimeMaxSaving;
        float power = jumpPower * jumpCurve.Evaluate(time);

        if (jumpTime >= jumpTimeMax)
            jumping = false;

        RigidBody.AddForce(power * Vector3.up, ForceMode.Impulse);

        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);
        if (horizontalVelocity.magnitude > maxJumpSpeed)
        {
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * maxJumpSpeed,
                                                   RigidBody.linearVelocity.y,
                                                   RigidBody.linearVelocity.z);
        }
    }
}
