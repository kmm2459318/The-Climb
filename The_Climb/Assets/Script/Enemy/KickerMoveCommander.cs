using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(CharacterGroundChecker))]
//  移動スクリプトに移動値を渡す
public class KickerMoveCommander : MonoBehaviour, IWallHitTable
{
    Coroutine JumpLoop;    //  ジャンプループコルーチンの変数
    [Header("Instance")]
    [SerializeField] KickerStatus kickerStatus;    //  ステータスインスタンス
    KickerStatBlock kickerStatBlock;    //  ステータスインスタンス
    EnemyMover enemyMover;    //  エネミームーバーインスタンス

    Vector3 Velocity;    //  キャラクター移動値

    float CurrentMoveSpd;    //  現在の移動速度
    float CurrentJumpForce;    //  現在のジャンプ力
    float CurrentJumpFrequency;    //  現在のジャンプ頻度
    [Header("Move Value")]
    [SerializeField] MoveDir CurrentMoveDir;    //  現在の動く方向(X軸)


    void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();
        kickerStatBlock = kickerStatus.GetStats(EnemyStates.NORMAL);
        //  数値初期化
        Initialize();
    }
    void Start()
    {
        Velocity = new Vector3(CurrentMoveSpd * (int)CurrentMoveDir * Time.fixedDeltaTime, 0f, 0f);
        //  定期的なジャンプのループを開始
        JumpLoop = StartCoroutine(PeriodicallyJump());
    }
    public void FixedUpdate()
    {
        Velocity = new Vector3(CurrentMoveSpd * (int)CurrentMoveDir * Time.fixedDeltaTime, Velocity.y, 0f);
        //  基本移動
        enemyMover.BaseMove(Velocity);
    }
    //  初期化
    void Initialize()
    {
        CurrentMoveSpd = kickerStatBlock.MoveSpd;
        CurrentJumpForce = kickerStatBlock.JumpForce;
        CurrentJumpFrequency = kickerStatBlock.JumpFrequency;
    }
    //  壁に当たった時の処理
    public void OnHitWall()
    {
        if(CurrentMoveDir != MoveDir.NONE && CurrentMoveDir == MoveDir.RIGHT)
        {
            CurrentMoveDir = MoveDir.LEFT;
        }
        else if(CurrentMoveDir == MoveDir.LEFT)
        {
            CurrentMoveDir = MoveDir.RIGHT;
        }
    }
    //  定期的なジャンプのループ
    IEnumerator PeriodicallyJump()
    {
        CharacterGroundChecker characterGroundChecker = GetComponent<CharacterGroundChecker>();
        
        while (true)
        {
            yield return new WaitForSeconds(CurrentJumpFrequency);

            if(characterGroundChecker.CheckIsGround())
            //  ジャンプ
            enemyMover.Jump(CurrentJumpForce);
        }

    }
}
//  以下コード保存所
//if ((int)CurrentMoveDir == 0)
//{
//    (int)CurrentMoveDir = 1;
//}
//else
//{
//    MoveDir = (int)Mathf.Sign(MoveDir);
//}