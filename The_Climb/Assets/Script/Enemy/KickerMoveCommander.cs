using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
//  移動スクリプトに移動値を渡す
public class KickerMoveCommander : MonoBehaviour, IWallHitTable
{
    [SerializeField] KickerStatus kickerStatus;    //  ステータスインスタンス
    EnemyMover enemyMover;    //  エネミームーバーインスタンス

    Vector2 Velocity;    //  キャラクター移動値

    float CurrentMoveSpd;    //  現在の移動速度
    int CurrentMoveDir;    //  現在の動く方向(X軸)

    void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();

        //  数値初期化
        Initialize();
    }
    void Start()
    {
        Velocity = new Vector3(CurrentMoveSpd * CurrentMoveDir * Time.fixedDeltaTime, 0f, 0f);
    }
    public void FixedUpdate()
    {
        Velocity = new Vector3(CurrentMoveSpd * CurrentMoveDir * Time.fixedDeltaTime, 0f, 0f);
        enemyMover.Move(Velocity);
    }
    //  初期化
    void Initialize()
    {
        CurrentMoveSpd = kickerStatus.MoveSpdProperty;
        CurrentMoveDir = kickerStatus.FirstMoveDirProperty;
    }
    //  壁に当たった時の処理
    public void OnHitWall()
    {
        CurrentMoveDir *= -1;
    }
}