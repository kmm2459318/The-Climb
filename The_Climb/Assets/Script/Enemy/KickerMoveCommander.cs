using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(CharacterGroundChecker))]
[RequireComponent(typeof(LandGroundNotifier))]
//  移動スクリプトに移動値を渡す
public class KickerMoveCommander : MonoBehaviour, IWallHitTable, ILandingHandler
{
    public enum KickerCommanderMethod    //  このクラス内の関数一覧
    {
        MOVE,
        IS_EDGE_POS,
        FLIP_MOVE_DIR,
        JUMP,
    }
    

    [Header("Instance")]
    [SerializeField] KickerStatus kickerStatus;    //  ステータスインスタンス
    KickerStatBlock kickerStatBlock;    //  ステータスインスタンス
    EnemyMover enemyMover;    //  エネミームーバーインスタンス
    CharacterGroundChecker characterGroundChecker;    //  グラウンドチェッカーインスタンス
    EnemyStateMachine enemyStateMachine;
    public Dictionary<KickerCommanderMethod, ICommand> CommanderMethodMap;    //  このスクリプトの関数の辞書
    public event Action OnJumpTime;    //  ジャンプタイムのサブスク
    public event Action OnLandGround;    //  地面着地のサブスク
    Coroutine JumpLoop;    //  ジャンプループコルーチンの変数

    ICommandProvider commandProvider;
    IEnemyStateFactory enemyStateFactory;

    Vector3 Velocity;    //  キャラクター移動値
    Vector3 EdgeRayOffset;    //  端を検知するRayのオフセット

    [Header("Move Value")]
    [SerializeField] MoveDir CurrentMoveDir;    //  現在の動く方向(X軸)
    float CurrentMoveSpd;    //  現在の移動速度
    float CurrentJumpForce;    //  現在のジャンプ力
    float CurrentJumpFrequency;    //  現在のジャンプ頻度

    void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();
        kickerStatBlock = kickerStatus.GetStats(EnemyMode.NORMAL);
        characterGroundChecker = GetComponent<CharacterGroundChecker>();
        enemyStateMachine = new EnemyStateMachine();
        commandProvider = new DefaultCommandProvider(this);
        enemyStateFactory = new EnemyStateFactory(this, enemyStateMachine);

        //  初期状態をWalkに変更
        CommanderMethodMap =commandProvider.GetCommandMap();
        enemyStateMachine.ChangeState(enemyStateFactory.CreateWalkState());
        
        //  数値初期化
        Initialize();
    }
    void Start()
    {
        //  定期的なジャンプのループを開始
        JumpLoop = StartCoroutine(PeriodicallyJump());
    }
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, Vector3.down * characterGroundChecker.GroundCheckDisProperty, Color.red);
        Debug.DrawRay(EdgeRayOffset + transform.position, Vector3.down * characterGroundChecker.GroundCheckDisProperty, Color.gray);
        enemyStateMachine.FixedUpdate();
    }
    //  初期化
    void Initialize()
    {
        //CommanderMethodMap = new Dictionary<KickerCommanderMethod, ICommand>
        //{
        //    {KickerCommanderMethod.MOVE, new ActionCommand(Move)},
        //    {KickerCommanderMethod.IS_EDGE_POS, new FuncCommand<bool>(IsEdgePos) },
        //    {KickerCommanderMethod.FLIP_MOVE_DIR, new ActionCommand(FlipMoveDir)},
        //    {KickerCommanderMethod.JUMP, new ActionCommand(Jump)},
        //};

        EdgeRayOffset = kickerStatBlock.EdgeRayOffset;
        if(CurrentMoveDir != MoveDir.NONE && Mathf.Sign(EdgeRayOffset.x) != Mathf.Sign((int)CurrentMoveDir))
        {
            Debug.LogWarning($"CurrentMoveDir : {CurrentMoveDir} and EdgeRayOffset direction of x :{EdgeRayOffset.x} is not same direction.{Environment.NewLine}  Fix to CurrentMoveDirection");
            EdgeRayOffset *= (int)MoveDir.LEFT;
        }

        CurrentMoveSpd = kickerStatBlock.MoveSpd;
        CurrentJumpForce = kickerStatBlock.JumpForce;
        CurrentJumpFrequency = kickerStatBlock.JumpFrequency;
    }
    //  定期的なジャンプ発火ループ
    IEnumerator PeriodicallyJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(CurrentJumpFrequency);
            if (characterGroundChecker.CheckIsGround())
            {
                OnJumpTime?.Invoke();
            }
        }
    }
    //  移動
    public void Move()
    {
        Velocity = new Vector3(CurrentMoveSpd * (int)CurrentMoveDir * Time.fixedDeltaTime, Velocity.y, 0f);
        //  基本移動
        enemyMover.BaseMove(Velocity);
    }
    //  端か判定する
    public bool IsEdgePos()
    {
        return !characterGroundChecker.CheckIsGround(EdgeRayOffset + this.transform.position) &&
                characterGroundChecker.CheckIsGround();
    }
    //  移動方向反転
    public void FlipMoveDir()
    {
        CurrentMoveDir = CurrentMoveDir switch
        {
            MoveDir.RIGHT => MoveDir.LEFT,
            MoveDir.LEFT => MoveDir.RIGHT,
            _ => MoveDir.NONE,
        };
        EdgeRayOffset.x *= -1f;
    }
    //  ジャンプ
    public void Jump()
    {
        enemyMover.Jump(CurrentJumpForce);
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
        EdgeRayOffset.x *= -1f;
    }
    //    ステージに着地した時の処理
    public void OnLandStage()
    {
        OnLandGround?.Invoke();
    }
}