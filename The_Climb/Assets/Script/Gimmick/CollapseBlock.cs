using Unity.VisualScripting;
using UnityEngine;

public class CollapseBlock : MonoBehaviour
{
    public float CollapseDelay = 1f;   //崩れるまでの遅延時間
    public float RespawnDelay  = 3f;　 //リスポーンするまでの時間

    private bool IsCollapsing = false; //崩れる処理が進行中か判定
    private float CollapseTimer;       //崩れるまでのタイマー
    private float RespawnTimer;        //リスポーンまでのタイマー

    private GameObject VisualPart;     //見た目用の子オブジェクト(足場の見た目部分)
    private Collider HitBoxCollider;   //当たり判定用コライダー

    void Start()
    {
        VisualPart = transform.GetChild(0).gameObject; //非表示にする足場の見た目(子オブジェクト)を取得
        HitBoxCollider = GetComponent<Collider>();     //自身のコライダーを取得
        CollapseTimer = CollapseDelay;                 //タイマーを初期化
    }

    void Update()
    {
        //プレイヤーが乗った時、崩れるまでのタイマーを進める
        if(IsCollapsing)
        {
            Debug.Log("isCollapse内");
            CollapseTimer -= Time.deltaTime;
            if(CollapseTimer <= 0)
            {
                Collapse();
            }
        }

        //非アクティブ時、リスポーンタイマーを進める
        if (VisualPart != null && !VisualPart.activeSelf)
        {
            Debug.Log("非アクティブ中のリスポーン処理");
            RespawnTimer -= Time.deltaTime;
            if(RespawnTimer <=0)
            {
                Respawn();
            }
        }
    }

    //プレイヤーが上に乗った時に崩れる処理を進行
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && !IsCollapsing)
        {
            foreach(ContactPoint contact in collision.contacts)
            {
                if(Vector3.Dot(-contact.normal,Vector3.up) > 0.8f)
                {
                    IsCollapsing = true;
                    break;
                }
            }
        }
    }

    //足場を崩す処理
    void Collapse()
    {

        //コライダーを無効化
        if (HitBoxCollider != null)
        {
            HitBoxCollider.enabled = false;
        }

        //見た目を非表示(子オブジェクト)
        if(VisualPart != null)
        {
            VisualPart.SetActive(false);
        }

        CollapseTimer = CollapseDelay;
        IsCollapsing = false;
        RespawnTimer = RespawnDelay;
    }

    //足場を元に戻す処理
    void Respawn()
    {
        if (HitBoxCollider != null)
        {
            HitBoxCollider.enabled = true;
        }

        if(VisualPart != null)
        {
            VisualPart.SetActive(true);
        }
    }
}
