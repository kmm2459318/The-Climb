using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
//  移動スクリプトに移動値を渡す
public class KickerMoveCommander : MonoBehaviour, IWallHitTable
{
    Coroutine JumpLoop;
    [Header("Instance")]
    [SerializeField] KickerStatus kickerStatus;    //  ステータスインスタンス
    EnemyMover enemyMover;    //  エネミームーバーインスタンス

    Vector3 Velocity;    //  キャラクター移動値

    float CurrentMoveSpd;    //  現在の移動速度
    float CurrentJumpForce;    //  現在のジャンプ力
    float CurrentJumpFrequency;    //  現在のジャンプ頻度
    [Header("Move Value")]
    [SerializeField] int MoveDir;    //  現在の動く方向(X軸)


    void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();

        //  数値初期化
        Initialize();
    }
    void Start()
    {
        Velocity = new Vector3(CurrentMoveSpd * MoveDir * Time.fixedDeltaTime, 0f, 0f);
        JumpLoop = StartCoroutine(PeriodicallyJump());
    }
    public void FixedUpdate()
    {
        Velocity = new Vector3(CurrentMoveSpd * MoveDir * Time.fixedDeltaTime, Velocity.y, 0f);
        //  基本移動
        enemyMover.BaseMove(Velocity);
    }
    //  初期化
    void Initialize()
    {
        CurrentMoveSpd = kickerStatus.MoveSpdProperty;
        CurrentJumpForce = kickerStatus.JumpForceProperty;
        CurrentJumpFrequency = kickerStatus.JumpFrequencyProperty;
        if(MoveDir == 0)
        {
            MoveDir = 1;
        }
        else 
        {
            MoveDir = (int)Mathf.Sign(MoveDir);
        }
    }
    //  壁に当たった時の処理
    public void OnHitWall()
    {
        MoveDir *= -1;
    }
    //  定期的なジャンプのループ
    IEnumerator PeriodicallyJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(CurrentJumpFrequency);
            //  ジャンプ
            enemyMover.Jump(CurrentJumpForce);
        }

    }
}