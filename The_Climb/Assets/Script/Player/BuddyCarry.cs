using UnityEngine;
using UnityEngine.Animations;

public class BuddyCarry : MonoBehaviour
{
    private GameObject buddy;             //Buddyのゲームオブジェクト
    private PositionConstraint buddyPos;  //BuddyのPositionConstraint（おんぶに使ってる追従のコンポーネント）
    PlayerState state;

    private bool carryingBuddy = true;    //Buddyをおんぶしてる状態か判定
    public bool nearBuddy = false;        //Buddyが近くにいるか判定
    private bool buddyMoving = false;     //Buddyが動いてるか判定
    private float buddyTargetX = 0f;      //Buddyが向かうX座標
    private float buddyMoveSpeed = 4f;    //Buddyの移動速度

    void Start()
    {
        buddy = GameObject.Find("Buddy");
        buddyPos = buddy.GetComponent<PositionConstraint>();
        state = GetComponent<PlayerState>();
    }

    void Update()
    {
        //向いてる方向によっておんぶしてるバディの場所を調整
        if (buddyPos != null && state.playerDirectionRight)
        {
            buddyPos.translationOffset = new Vector3(-0.4f, 1f, 0f);
        }
        else
        {
            buddyPos.translationOffset = new Vector3(0.4f, 1f, 0f);
        }

        //Carryボタン（仮）
        if (Input.GetKeyDown(KeyCode.C) && state.isGrounded)
        {
            if (carryingBuddy)  //おんぶしてる場合、バディを降ろす
            {
                carryingBuddy = false;
                buddyPos.constraintActive = false;
                nearBuddy = true;
                buddy.transform.position = transform.position + Vector3.up * 0.5f;
            }
            else if (nearBuddy)  //おんぶしてない場合、バディをおんぶする
            {
                carryingBuddy = true;
                buddyPos.constraintActive = true;
            }
        }

        //ベルを鳴らしてバディを誘導
        if (!carryingBuddy && Input.GetKeyDown(KeyCode.B))
        {
            GuideBuddy(gameObject.transform.position.x);
        }

        //バディが誘導により動く
        BuddyMove();
    }

    //バディを誘導地点まで動かす（壁に当たると止まる）
    private void GuideBuddy(float guidePoint)
    {
        buddyTargetX = guidePoint;
        buddyMoving = true;
    } 

    private void BuddyMove()
    {
        // バディ誘導移動処理
        if (!carryingBuddy && buddyMoving)
        {
            Vector3 buddyPosNow = buddy.transform.position;
            Vector3 targetPos = new Vector3(buddyTargetX, buddyPosNow.y, buddyPosNow.z);

            // 移動
            buddy.transform.position = Vector3.MoveTowards(buddyPosNow, targetPos, buddyMoveSpeed * Time.deltaTime);
        }

        // 目標に到達もしくはおんぶしたら停止
        if (Mathf.Abs(buddy.transform.position.x - buddyTargetX) < 0.05f || carryingBuddy)
        {
            buddyMoving = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Buddy"))
        {
            nearBuddy = true;  //Buddyが近くにいる
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Buddy"))
        {
            nearBuddy = false;  //Buddyが近くにいない
        }
    }
}
