using UnityEngine;
using System.Collections;
using Zenject.Asteroids;
using System.Buffers.Text;



public class Boss_20_Controller : MonoBehaviour
{
    [Header("20階層のボス")]
    [Tooltip("Aseetファイル")]
    public Boss_20_StatusObjectScript status;   //Assetファイル
    [Tooltip("弾のプレハブ")]
    public GameObject Bullet_Prefab;            //弾のPrehab
    [Tooltip("弾の発射位置")]
    public Transform Bullet_Position;      //弾の発射位置
    [Tooltip("弾の発射位置")]
    public Transform Player;

    private int Enemy_Left_Max;                 //敵の移動は左の範囲
    private int Enemy_Right_Max;　　　　　　　　//敵の移動は右の範囲
    private float Enemy_Vertical;               //敵の縦移動
    private int Boss_Move_Direction;　　　　　  //敵の動く方向
    private float Bullet_Timer;　　　　　　　　 //弾を発射するまでの時間
    private float Rest_Timer;　　　　　　　　　 //休憩時間
    private float Boss_Speed;　　　　　　　　　 //ボスの速さ
    private bool IsResting = false;　　　　　　 //ボスの動くかどうかの判定
    private Rigidbody Rb;
    private float Wave = 5.0f;                  //揺れ動く回数
    private Boss_20_Knockback knockbackScript;

    //初期化処理
    void Awake()
    {
        knockbackScript = GetComponent<Boss_20_Knockback>();
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
        Bullet_Timer = status.Attack;
        Boss_Move_Direction = status.LEFT;
        Rest_Timer = status.Rest;
        Boss_Speed = status.Speed;
        Enemy_Left_Max = status.LEFT_Max;
        Enemy_Right_Max = status.RIGHT_Max; 
        Enemy_Vertical = status.Vertical;

    }

  //ボスの動き
  void Move() 
  { 
      if (IsResting || knockbackScript.IsKnockbacking) return;
      Vector3 WaveMotion = new Vector3(0f, Mathf.Sin(Time.time * Wave) * Enemy_Vertical, 0f); 
      Vector3 NewPosition = Rb.position + new Vector3(Boss_Speed * Boss_Move_Direction * Time.fixedDeltaTime, 0f) + WaveMotion; 
      EnemyMovementRange(ref NewPosition); 
      Rb.MovePosition(NewPosition); Rest_Timer -= Time.fixedDeltaTime;
     

     if (Rest_Timer <= 0f && !IsResting) 
     { 
            IsResting = true; StartCoroutine(RestAndResume()); } 
     }



    //ボスの休憩
    IEnumerator RestAndResume()
    {
        Debug.Log("減速開始");
        Boss_Speed = 0f;
        Debug.Log("停止完了");
        yield return new WaitForSeconds(3f);
        Debug.Log("休憩終了");
        Boss_Speed = status.Speed;
        Rest_Timer = status.Rest;
        IsResting = false;
    }

    //動く方向の指定
    void EnemyMovementRange(ref Vector3 nextPosition)
    {
       if(nextPosition.x <= Enemy_Left_Max) 
       { 
         Boss_Move_Direction = status.RIGHT; 
       }
        else if(nextPosition.x >= Enemy_Right_Max) 
        { 
         Boss_Move_Direction = status.LEFT; 
        } 
    }

    //弾の発射タイミング
    void HandleShooting()
    {
        Bullet_Timer -= Time.deltaTime;
        if (Bullet_Timer <= 0f)
        {
            Bullet();
            Bullet_Timer = status.Attack;
        }
    }

    //発射　　　　　　　　　　
    void Bullet()
    {
        if (IsResting || knockbackScript.IsKnockbacking) return;
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
