using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

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
        positionConstraint.constraintActive = false;  //1度無効にする
        positionConstraint.SetSources(new System.Collections.Generic.List<ConstraintSource>());

        if (newTarget != null)
        {
            var source = new ConstraintSource
            {
                sourceTransform = newTarget,
                weight = 1f
            };
            positionConstraint.AddSource(source);
            currentSource = source;
            positionConstraint.constraintActive = true;
        }
        else
        {
            currentSource = default;
        }
    }
}
