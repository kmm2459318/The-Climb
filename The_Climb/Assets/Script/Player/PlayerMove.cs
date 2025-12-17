using UnityEngine;
using UnityEngine.InputSystem;
using TheClimb.Item;
using TheClimb.Player;
using TheClimb.Astral;
using System.Collections;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour, IConveyorReceiver
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerJump jump;
    PlayerSpecialAction special;
    PlayerKnockBack knock;
    VectorToPlanetCalculator vectorToPlanetCaluculator;    //  天体までのベクトルを計算するクラス

    [SerializeField] private InputActionReference leftMoveAction;
    [SerializeField] private InputActionReference rightMoveAction;
    [SerializeField] private ImpactBallController inpactBallController;

    [SerializeField] private bool reverseHorizontal = false;

    [SerializeField] private bool upsideDown = false; // 天井歩行モード
    [SerializeField] private float customGravity = 9.81f; // 通常重力に近い値

    IPlayerDataProvider PlayerDataProvider;    //  プレイヤーのデータプロバイダ
    IPlanetDataProvider PlanetDataProvider;    //  天体のデータプロバイダ

    private float groundMoveForce = 0.7f;     //プレイヤーの地上移動速度
    public float groundMaxSpeed = 6.459797f;   //プレイヤーの地上最高速度記憶
    public float moveInput = 0f;        //プレイヤーの移動方向
    private float airMoveForce = 40f;    //空中での移動速度
    public float airMaxSpeed = 9f;     //空中での速度制限

    public bool slipping = false;        //着地後勢い止めず滑ってる判定
    public Vector3 slipVelocity;                //滑り時のVelocity

    public float MoveInput => moveInput; // ←読み取り専用プロパティ
    public PlayerAnimation PlayerAnimation;
    public PlayerState State => state;

    private bool OnBelt = false;                 //ベルトコンベアに乗っているか
    private Vector3 BeltVelocity = Vector3.zero; //ベルトコンベアの速度(未接触時はゼロ)

    public bool IsUpsideDown => upsideDown;

    //void Awake()
    //{
    //    PlayerContext.Instance.RegistPlayerMove(this);

    //    //PlayerDataProvider = PlayerContext.Instance._PlayerDataProvider;
    //    //PlanetDataProvider = PlanetContext.Instance._PlanetDataProvider;

    //    vectorToPlanetCaluculator = new VectorToPlanetCalculator(PlanetDataProvider, PlayerDataProvider);
    //}

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        jump = gameObject.GetComponent<PlayerJump>();
        special = gameObject.GetComponent<PlayerSpecialAction>();
        knock = gameObject.GetComponent<PlayerKnockBack>();

        PlayerAnimation = GameObject.Find("pico_chan_chr_pico_00").GetComponent<PlayerAnimation>();

        if (upsideDown)
        {
            // プレイヤーを上下反転表示（天井に張り付くように）
            Vector3 scale = transform.localScale;
            scale.y *= -1;
            transform.localScale = scale;
        }

        RigidBody.useGravity = false;

    }

    private void ApplyCustomGravity()
    {
        if (upsideDown)
        {
            RigidBody.useGravity = false;

            // 天井歩行モードでも、ジャンプ中は重力を加えない
            if (!state.isGrounded && !jump.jumping)
            {
                float gravityScale = 0.8f;
                RigidBody.AddForce(Vector3.up * customGravity * gravityScale, ForceMode.Acceleration);
            }
        }
        else
        {
            // 通常時はUnityの重力を使用
            RigidBody.useGravity = true;
        }
    }

    public void ToggleUpsideDown()
    {
        upsideDown = !upsideDown; // 状態を反転

        // プレイヤーの「中心位置」と「高さ」を考慮して移動量を計算
        // Pivotが足元にある場合：Center.yはHeight/2 -> 移動量はHeight
        // Pivotが中心にある場合：Center.yは0 -> 移動量は0
        // 公式：MoveAmount = 2 * Center.y * ScaleY

        float moveAmountY = 0f;
        float scaleY = Mathf.Abs(transform.localScale.y);
        bool foundValidCollider = false;

        // 1. CharacterController
        var charController = GetComponent<CharacterController>();
        if (charController != null)
        {
            moveAmountY = 2 * charController.center.y * scaleY;
            foundValidCollider = true;
        }
        else
        {
            // 2. CapsuleCollider
            var capsule = GetComponent<CapsuleCollider>();
            if (capsule != null)
            {
                moveAmountY = 2 * capsule.center.y * scaleY;
                foundValidCollider = true;
            }
            else
            {
                // 3. Collider (Boundsから中心オフセットを計算)
                // 小さすぎるCollider（足元のトリガーなど）は無視する
                var cols = GetComponents<Collider>();
                foreach (var col in cols)
                {
                    if (!col.isTrigger && col.bounds.size.y > 0.5f)
                    {
                        float centerOffset = col.bounds.center.y - transform.position.y;
                        moveAmountY = 2 * centerOffset;
                        foundValidCollider = true;
                        break;
                    }
                }
            }
        }

        // 4. まだ有効なColliderが見つかっていない場合、子オブジェクトを探す
        if (!foundValidCollider)
        {
            var childCols = GetComponentsInChildren<Collider>();
            float maxBoundsY = -999f;
            float minBoundsY = 999f;
            bool foundChild = false;

            foreach (var c in childCols)
            {
                if (c.gameObject != gameObject && !c.isTrigger && c.bounds.size.y > 0.1f)
                {
                    maxBoundsY = Mathf.Max(maxBoundsY, c.bounds.max.y);
                    minBoundsY = Mathf.Min(minBoundsY, c.bounds.min.y);
                    foundChild = true;
                }
            }

            if (foundChild)
            {
                float boundsCenterY = (maxBoundsY + minBoundsY) / 2f;
                float centerOffset = boundsCenterY - transform.position.y;
                moveAmountY = 2 * centerOffset;
                foundValidCollider = true;
            }
        }

        // 5. Renderer (Boundsから) - Colliderがない場合の最終手段
        if (!foundValidCollider)
        {
            var rend = GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                float centerOffset = rend.bounds.center.y - transform.position.y;
                moveAmountY = 2 * centerOffset;
                foundValidCollider = true;
            }
        }

        // 6. デフォルト（足元ピボットと仮定して1.8m移動）
        if (!foundValidCollider)
        {
            moveAmountY = 1.8f;
            Debug.LogWarning("[GravityFlip] Center could not be detected. Using default 1.8f.");
        }

        // 見た目の上下反転（遅延させることで位置移動とのズレを目立たなくする）
        StartCoroutine(DelayedVisualFlip(0.1f));

        //// 慣性は横方向だけ維持（縦をリセット）
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);

        RigidBody.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);

        // プレイヤー位置の調整
        // 重力が下の時に切り替えたら上にずらす（＝通常→天井で上にずらす）
        if (upsideDown)
        {
            // 通常→天井：上に移動
            transform.position += Vector3.up * moveAmountY;
        }
        else
        {
            // 天井→通常：下に移動
            transform.position -= Vector3.up * moveAmountY;
        }

        //// 地面の判定をリセット
        state.isGrounded = false;

       Debug.Log($"{name} が上下反転！（現在: {(upsideDown ? "天井" : "地面")}）");
    }

    public void ResetGravity()
    {
        if (upsideDown)
        {
            upsideDown = false;

            // 見た目を元に戻す
            Vector3 scale = transform.localScale;
            scale.y = Mathf.Abs(scale.y); // 正の値にする
            transform.localScale = scale;

            // 重力設定を戻す（ApplyCustomGravityで処理されるが、念のため）
            RigidBody.useGravity = true;

            Debug.Log("リスポーンに伴い重力をリセットしました");
        }
    }

    private IEnumerator DelayedVisualFlip(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y) * (upsideDown ? -1 : 1);
        transform.localScale = scale;
    }

    private void Update()
    {
        //移動キー操作
        if (!special.meteorDrop && !knock.knockBacking)　//ノックバック、メテオドロップ中は不可
        {
            MoveOperation();
        }
        CheckStuck();
    }

    void FixedUpdate()
    {
        if (inpactBallController != null)
        {
            //移動
            if (special.highJumpChargeCounter == 0f && inpactBallController.currentState is not InpactBallExplosionState)
            {
                if ((state.isGrounded || (!state.isGrounded && state.isJumpMoveOK && !state.isLeftWall && !state.isRightWall)) && !knock.knockBacking)
                {
                    jump.coyoteCounter = 0f;

                    //プレイヤー地上の移動
                    GroundPlayerMove();
                }
                else
                {
                    jump.coyoteCounter += Time.fixedDeltaTime;

                    //プレイヤー空中の移動
                    AirPlayerMove();
                }
            }
        }
        //移動
        if (special.highJumpChargeCounter == 0f)
        {
            if ((state.isGrounded || (!state.isGrounded && state.isJumpMoveOK && !state.isLeftWall && !state.isRightWall)) && !knock.knockBacking)
            {
                jump.coyoteCounter = 0f;

                //プレイヤー地上の移動
                GroundPlayerMove();
            }
            else
            {
                jump.coyoteCounter += Time.fixedDeltaTime;

                //プレイヤー空中の移動
                AirPlayerMove();
            }
        }

        ApplyCustomGravity();
    }

    private void MoveOperation()
    {
        if (state.inputManager.leftHeld && state.inputManager.rightHeld ||
            !state.inputManager.leftHeld && !state.inputManager.rightHeld)  //止まる
        {
            moveInput = 0f;
        }
        else if (state.inputManager.leftHeld && !state.isLeftWall)  //左移動
        {
            moveInput = -1f;
            state.playerDirectionRight = false;
        }
        else if (state.inputManager.rightHeld && !state.isRightWall)  //右移動
        {
            moveInput = 1f;
            state.playerDirectionRight = true;
        }
        else
        {
            moveInput = 0f;
        }

        if (reverseHorizontal)
        {
            moveInput *= -1f;
        }

    }


    private void GroundPlayerMove()
    {
        //ベルトコンベア上の場合はベルトコンベアの力を加える
        if (OnBelt)
        {
            RigidBody.AddForce(BeltVelocity, ForceMode.Acceleration);
        }

        if (moveInput != 0f)
        {
            // 地上：慣性なし、即応する左右移動
            Vector3 force = new Vector3(moveInput, 0f, 0f) * groundMoveForce;
            RigidBody.AddForce(force);
            RigidBody.linearVelocity = new Vector3(force.x * Time.deltaTime * 1000.0f, RigidBody.linearVelocity.y, 0f);
        }
        else if(!special.meteorHighJump && !jump.jumpCoolActive && RigidBody.linearVelocity.x != 0f)
        {
            RigidBody.linearVelocity = new Vector3(0f, RigidBody.linearVelocity.y, 0f);
        }

        
    }

    private void AirPlayerMove()
    {
        // 空中：左右に力を加える
        Vector3 force = new Vector3(moveInput, 0f, 0f) * airMoveForce;
        switch (jump.landingJumpNumber)
        {
            case 0:
                {
                    break;
                }
            case 1:
                {
                    force *= 1.2f;
                    break;
                }
            default:
                {
                    force *= 1.4f;
                    break;
                }
        }
        RigidBody.AddForce(force, ForceMode.Acceleration);

        // 最大空中速度を制限
        if (!special.quickJumpUsed && !special.highJumpUsed)
        {
            airMaxSpeed = 10f;
        }
        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);
        if (horizontalVelocity.magnitude > airMaxSpeed)
        {
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * airMaxSpeed, RigidBody.linearVelocity.y, RigidBody.linearVelocity.z);
        }

        //ハイジャンプ後徐々に早くするよ
        if (special.highJumpUsed)
        {
            if (airMaxSpeed > 10f)
            {
                airMaxSpeed = 10f;
            }
            else if (airMaxSpeed < 0.1f)
            {
                airMaxSpeed += airMaxSpeed;
            }
            else if (airMaxSpeed < 10f)
            {
                airMaxSpeed += 0.3f;
            }
        }
        else if (special.quickJumpUsed) //クイックジャンプ後徐々に遅くするよ
        {
            if (airMaxSpeed > 10f)
            {
                //Debug.Log(maxAirSpeed);
                airMaxSpeed -= 0.14f;
            }
        }
    }

    public void SetOnBelt(bool OnBelt, Vector3 velocity)
    {
        this.OnBelt = OnBelt;
        this.BeltVelocity = velocity;
    }

    [Header("埋まり判定設定")]
    public float stuckCheckDelay = 0.5f; // 埋まり判定が確定するまでの時間
    public CapsuleCollider stuckDetectionCollider; // 埋まり判定専用のコライダー（未設定なら自動検出）
    public Collider[] collidersToDisableOnUnstuck; // 脱出時に一時的に無効化するコライダー（足元のSphereColliderなど）
    private float stuckTimer = 0f;

    private void CheckStuck()
{
    // カプセルコライダーを取得
    Vector3 point1, point2;
    float radius;

    CapsuleCollider capsule = stuckDetectionCollider != null ? stuckDetectionCollider : GetComponent<CapsuleCollider>();
    if (capsule != null)
    {
        float shrink = 0.1f;
        float maxScale = Mathf.Max(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
        radius = capsule.radius * maxScale - shrink;
        float height = capsule.height * maxScale - (shrink * 2);

        Vector3 center = capsule.transform.TransformPoint(capsule.center);
        point1 = center + Vector3.up * (height / 2f - radius);
        point2 = center - Vector3.up * (height / 2f - radius);
    }
    else
    {
        return;
    }

    // コライダー取得
    Collider[] hitColliders = Physics.OverlapCapsule(point1, point2, radius);

    bool isStuck = false;
    foreach (var col in hitColliders)
    {
        if (col.gameObject != gameObject && !col.isTrigger)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Ground") && !col.CompareTag("Nosink"))
            {
                isStuck = true;
                break;
            }
        }
    }

    if (isStuck)
    {
        stuckTimer += Time.deltaTime;
        if (stuckTimer > stuckCheckDelay)
        {
            Debug.LogWarning("プレイヤーの埋まりを検知しました。位置を修正します。");

            float direction = upsideDown ? 1f : -1f;
            transform.position += Vector3.up * direction * 3.0f;

            if (collidersToDisableOnUnstuck != null && collidersToDisableOnUnstuck.Length > 0)
            {
                StartCoroutine(TemporarilyDisableColliders());
            }

            stuckTimer = 0f;
        }
    }
    else
    {
        stuckTimer = 0f;
    }
}

    private IEnumerator TemporarilyDisableColliders()
    {
        // 指定されたコライダーを無効化
        foreach (var col in collidersToDisableOnUnstuck)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        // 0.1秒待機
        yield return new WaitForSeconds(0.1f);

        // 再有効化
        foreach (var col in collidersToDisableOnUnstuck)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }
    }

}
