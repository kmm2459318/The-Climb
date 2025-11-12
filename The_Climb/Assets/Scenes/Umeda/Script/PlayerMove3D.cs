using UnityEngine;
using UnityEngine.InputSystem;
using TheClimb.Player;
using TheClimb.Astral;

public class PlayerMove3D : MonoBehaviour, IConveyorReceiver
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerJump jump;
    PlayerKnockBack knock;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference leftMoveAction;
    [SerializeField] private InputActionReference rightMoveAction;
    [SerializeField] private InputActionReference forwardMoveAction;
    [SerializeField] private InputActionReference backMoveAction;

    [Header("移動設定")]
    public float groundMoveForce = 0.7f;
    public float groundMaxSpeed = 6.46f;
    public float moveInput = 0f;
    public float moveInputZ = 0f;
    public float airMoveForce = 50f;
    public float airMaxSpeed = 10f;

    [Header("プレイヤー回転")]
    private Vector3 lastFacingDirection = Vector3.forward;

    public PlayerAnimation PlayerAnimation;

    private bool OnBelt = false;
    private Vector3 BeltVelocity = Vector3.zero;

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        jump = GetComponent<PlayerJump>();
        knock = GetComponent<PlayerKnockBack>();

        PlayerAnimation = GameObject.Find("pico_chan_chr_pico_00").GetComponent<PlayerAnimation>();

        // InputActionを有効化
        if (leftMoveAction != null) leftMoveAction.action.Enable();
        if (rightMoveAction != null) rightMoveAction.action.Enable();
        if (forwardMoveAction != null) forwardMoveAction.action.Enable();
        if (backMoveAction != null) backMoveAction.action.Enable();
    }

    void Update()
    {
        MoveOperation();
        HandleRotation();
    }

    void FixedUpdate()
    {
        if ((state.isGrounded || (!state.isGrounded && state.isJumpMoveOK && !state.isLeftWall && !state.isRightWall)) && !knock.knockBacking)
        {
            jump.coyoteCounter = 0f;
            GroundPlayerMove();
        }
        else
        {
            jump.coyoteCounter += Time.fixedDeltaTime;
            AirPlayerMove();
        }
    }

    private void MoveOperation()
    {
        // X方向
        if (leftMoveAction.action.IsPressed() && !state.isLeftWall) moveInput = -1f;
        else if (rightMoveAction.action.IsPressed() && !state.isRightWall) moveInput = 1f;
        else moveInput = 0f;

        // Z方向
        if (forwardMoveAction.action.IsPressed()) moveInputZ = 1f;
        else if (backMoveAction.action.IsPressed()) moveInputZ = -1f;
        else moveInputZ = 0f;

        // ★ 斜め入力時の速度を一定に保つ
        Vector3 currentInput = new Vector3(moveInput, 0f, moveInputZ);
        if (currentInput.sqrMagnitude > 1f)
            currentInput.Normalize();

        moveInput = currentInput.x;
        moveInputZ = currentInput.z;

        // 入力があれば向きを更新
        if (currentInput.sqrMagnitude > 0f)
            lastFacingDirection = currentInput;
    }

    private void HandleRotation()
    {
        if (lastFacingDirection.sqrMagnitude > 0f)
            transform.rotation = Quaternion.LookRotation(lastFacingDirection);
    }

    private void GroundPlayerMove()
    {
        if (OnBelt) RigidBody.AddForce(BeltVelocity, ForceMode.Acceleration);

        Vector3 force = new Vector3(moveInput, 0f, moveInputZ) * groundMoveForce;
        RigidBody.AddForce(force);

        Vector3 vel = RigidBody.linearVelocity;
        vel.x = Mathf.Clamp(force.x * Time.deltaTime * 1000f, -groundMaxSpeed, groundMaxSpeed);
        vel.z = Mathf.Clamp(force.z * Time.deltaTime * 1000f, -groundMaxSpeed, groundMaxSpeed);
        RigidBody.linearVelocity = new Vector3(vel.x, RigidBody.linearVelocity.y, vel.z);
    }

    private void AirPlayerMove()
    {
        Vector3 inputDir = new Vector3(moveInput, 0f, moveInputZ);
        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize(); // ★ こちらも安全のため正規化

        Vector3 force = inputDir * airMoveForce;

        switch (jump.landingJumpNumber)
        {
            case 1: force *= 1.2f; break;
            case > 1: force *= 1.4f; break;
        }

        RigidBody.AddForce(force, ForceMode.Acceleration);

        Vector3 horizontalVel = new Vector3(RigidBody.linearVelocity.x, 0f, RigidBody.linearVelocity.z);
        if (horizontalVel.magnitude > airMaxSpeed)
        {
            Vector3 clamped = horizontalVel.normalized * airMaxSpeed;
            RigidBody.linearVelocity = new Vector3(clamped.x, RigidBody.linearVelocity.y, clamped.z);
        }
    }

    public void SetOnBelt(bool OnBelt, Vector3 velocity)
    {
        this.OnBelt = OnBelt;
        this.BeltVelocity = velocity;
    }

    // ============================================================
    // ✅ PlayerAnimationUmeda 用の入力取得関数を追加
    // ============================================================
    public Vector2 GetMoveInput()
    {
        float x = 0f;
        float z = 0f;

        if (leftMoveAction != null && leftMoveAction.action.IsPressed()) x -= 1f;
        if (rightMoveAction != null && rightMoveAction.action.IsPressed()) x += 1f;
        if (forwardMoveAction != null && forwardMoveAction.action.IsPressed()) z += 1f;
        if (backMoveAction != null && backMoveAction.action.IsPressed()) z -= 1f;

        // ★ 正規化して常に等速
        Vector2 input = new Vector2(x, z);
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        return input;
    }
}
