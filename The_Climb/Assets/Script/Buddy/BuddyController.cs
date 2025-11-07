using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using System.Collections;

public class BuddyController : MonoBehaviour
{
    private Rigidbody RigidBody;
    private PlayerState state;
    private PositionConstraint positionConstraint;
    private ConstraintSource currentSource;

    private bool buddyDirectionRight;  //バディが右向いてるか判定、falseなら左向き
    public bool moving = false;        //Buddyが動いてるか判定
    private float speed = 4f;          //Buddyの移動スピード
    public float buddyTargetX = 0f;    //Buddyが向かうX座標
    public bool beingKidnapped = false;  //誘拐されてる

    [SerializeField] private bool isLeftWall;          //左壁判定
    [SerializeField] private bool isRightWall;         //右壁判定
    private LayerMask groundLayer;     //地面レイヤー

    private float buddyFallSpeed = -19f;  //プレイヤーの落下速度

    //バディが誘導される地点指定
    public void GuideTo(float x)
    {
        buddyTargetX = x;
        moving = true;

        //バディの向かう方向からどっち向くか決める
        if (buddyTargetX - transform.position.x > 0)
        {
            buddyDirectionRight = true;
        }
        else if (buddyTargetX - transform.position.x < 0)
        {
            buddyDirectionRight = false;
        }
    }

    void Start()
    {
        state = GameObject.Find("PlayerModel").GetComponent<PlayerState>();
        positionConstraint = GetComponent<PositionConstraint>();
        groundLayer = GameLayer.ToMask(GameLayers.GROUND);
        RigidBody = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //左壁判定（カプセル形）
        isLeftWall = Physics.CheckCapsule(transform.position + Vector3.left * 0.3f + Vector3.up * 0.49f, transform.position + Vector3.left * 0.3f + Vector3.down * 0.49f, 0.001f, groundLayer);
        //右壁判定（カプセル形）
        isRightWall = Physics.CheckCapsule(transform.position + Vector3.right * 0.3f + Vector3.up * 0.49f, transform.position + Vector3.right * 0.3f + Vector3.down * 0.49f, 0.001f, groundLayer);

        //おんぶされてるとき重力働かないように
        if (state.carryingBuddy)
        {
            RigidBody.useGravity = false;
        }
        else
        {
            RigidBody.useGravity = true;
        }

        //誘導により動く
        if (moving)
        {
            BuddyMove();
        }

        //落下速度調整
        if (RigidBody.linearVelocity.y < buddyFallSpeed)
        {
            RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, buddyFallSpeed, 0);
        }
    }

    //誘導地点に向かって動く
    private void BuddyMove()
    {
        Vector3 pos = transform.position;
        Vector3 target = new Vector3(buddyTargetX, pos.y, pos.z);
        //移動
        transform.position = Vector3.MoveTowards(pos, target, speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - buddyTargetX) < 0.05f || (isLeftWall && !buddyDirectionRight) || (isRightWall && buddyDirectionRight))
        {
            moving = false;
        }
    }

    //Buddyの追従するオブジェクトを変える
    public void SetConstraintTarget(Transform newTarget)
    {
        // Constraint無効化して評価停止
        positionConstraint.constraintActive = false;

        // 現在のSourcesを完全クリア
        var emptyList = new System.Collections.Generic.List<ConstraintSource>();
        positionConstraint.SetSources(emptyList);

        // 新しいSourceリストを個別に生成
        if (newTarget != null)
        {
            var newSources = new System.Collections.Generic.List<ConstraintSource>();

            var source = new ConstraintSource
            {
                sourceTransform = newTarget,
                weight = 1f
            };

            newSources.Add(source);

            // AddSourcesではなくSetSourcesを直接使用（コピー動作）
            positionConstraint.SetSources(newSources);

            // 状態保持
            currentSource = source;

            // Buddyを強制的に新しい位置へ同期（安全対策）
            transform.position = newTarget.position;

            // 1フレーム後に再有効化
            StartCoroutine(EnableConstraintNextFrame());
        }
        else
        {
            currentSource = default;
        }
    }

    private IEnumerator EnableConstraintNextFrame()
    {
        yield return null; // 1フレーム待って再評価させる
        positionConstraint.constraintActive = true;
    }

    private void LateUpdate()
    {
        if (positionConstraint != null)
        {
            var srcs = new System.Collections.Generic.List<ConstraintSource>();
            positionConstraint.GetSources(srcs);
            if (srcs.Count == 0)
            {
                Debug.LogWarning($"[{Time.frameCount}] Sourceが消失しました: {gameObject.name}");
            }
        }
    }
}
