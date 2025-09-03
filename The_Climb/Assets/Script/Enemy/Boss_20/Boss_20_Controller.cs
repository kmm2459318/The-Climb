using UnityEngine;
using System.Collections;
using Zenject.Asteroids;
using System.Buffers.Text;



public class Boss_20_Controller : MonoBehaviour
{
    [Header("20階層のボス")]
    [Tooltip("20階層ボスのStatusファイルにある")]
    public Boss_20_StatusObjectScript status;   //Assetファイル
    [Tooltip("弾のプレハブ")]
    public GameObject Bullet_Prefab;         //弾のPrehab
    [Tooltip("弾の発射位置")]
    public Transform Bullet_Position;      　//弾の発射位置
    [Tooltip("弾の発射位置")]
    public Transform Player;

    private int EnemyLeftMax;                 //敵の移動は左の範囲
    private int EnemyRightMax;　　　　　　　　//敵の移動は右の範囲
    private float EnemyVertical;               //敵の縦移動
    private int BossMoveDirection;　　　　　  //敵の動く方向
    private float BulletTime;　　　　　　　　 //弾を発射するまでの時間
    private float ActionTime;　　　　　　　　　 　　　//休憩時間までの時間
    private float RestTime;                   //休憩中
    private float BossSpeed;　　　　　　　　　 //ボスの速さ
    private bool IsResting = false;　　　　　　 //ボスの動くかどうかの判定
    private Rigidbody Rb;
    private float Wave = 5.0f;                  //揺れ動く回数
    Vector3 AncPos;
    
    //初期化処理
    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Initialize();

    }
    void Update()
    {
        HandleShooting();
    }

    void FixedUpdate()
    {
        Move();
    }

    //ボスの初期状態の設定
    void Initialize()
    {
        BulletTime = status.Attack;
        BossMoveDirection = status.LEFT;
        ActionTime = status.Action_Time;
        RestTime = status.Rest_Time;
        BossSpeed = status.Speed;
        EnemyLeftMax = status.LEFT_Max;
        EnemyRightMax = status.RIGHT_Max;
        EnemyVertical = status.Vertical;
        AncPos = transform.position;

    }

    //ボスの動き
    void Move()
    {
        if (IsResting) return;
        AncPos += new Vector3(BossSpeed * BossMoveDirection * Time.fixedDeltaTime,
        Mathf.Sin(Time.time * Wave) * EnemyVertical, 0f);
        // 折り返し処理
        EnemyMovementRange(ref AncPos);
        Rb.MovePosition(AncPos);
        ActionTime -=　 Time.fixedDeltaTime;
        if (ActionTime <= 0f && !IsResting)
        {
            IsResting = true; 
            StartCoroutine(RestAndResume()); }
    }



    //ボスの休憩
    IEnumerator RestAndResume()
    {
        BossSpeed = 0f;
        EnemyVertical = 0f;
        yield return new WaitForSeconds(RestTime);
        BossSpeed = status.Speed;
        ActionTime = status.Action_Time;
        IsResting = false;
    }

    //動く方向の指定
    void EnemyMovementRange(ref Vector3 nextPosition)
    {
       if(nextPosition.x <= EnemyLeftMax) 
       { 
         BossMoveDirection = status.RIGHT; 
       }
        else if(nextPosition.x >= EnemyRightMax) 
        { 
         BossMoveDirection = status.LEFT; 
        } 
    }

    //弾の発射タイミング
    void HandleShooting()
    {
        BulletTime -= Time.deltaTime;
        if (BulletTime <= 0f)
        {
            Bullet();
            BulletTime = status.Attack;
        }
    }

    //発射　　　　　　　　　　
    void Bullet()
    {
        if (IsResting) return;
        {
          if (Player == null) return;

          float spacing = 0.5f; // 横の間隔（好みに応じて調整）

          Vector3 basePosition = Bullet_Position.position;
          Quaternion rotation = Bullet_Position.rotation;

          // -1.5, +1.5 にオフセット（左右2個ずつ均等に）
          float[] offsets = new float[] { -1.5f, 1.5f };

          foreach (float offset in offsets)
          {
            Vector3 spawnPos = basePosition + Bullet_Position.right * offset * spacing;
            GameObject bullet = Instantiate(Bullet_Prefab, spawnPos, rotation);

            BossBullet bulletScript = bullet.GetComponent<BossBullet>();
            if (bulletScript != null)
            {
                bulletScript.status = this.status;
                bulletScript.Initialize(Player);
            }
          }
        }
       

    }

}
