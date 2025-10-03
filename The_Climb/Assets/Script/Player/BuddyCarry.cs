using UnityEngine;
using UnityEngine.Animations;

public class BuddyCarry : MonoBehaviour
{
    private GameObject buddy;             //Buddyのゲームオブジェクト
    private BuddyController buddyController;  //Buddyのスクリプト
    private PositionConstraint buddyPos;  //BuddyのPositionConstraint（おんぶに使ってる追従のコンポーネント）
    PlayerState state;

    public bool carryingBuddy = true;    //Buddyをおんぶしてる状態か判定
    private bool nearBuddy = false;       //Buddyが近くにいるか判定

    void Start()
    {
        buddy = GameObject.Find("Buddy");
        buddyController = buddy.GetComponent<BuddyController>();
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
                buddy.transform.position = transform.position + Vector3.up * 0.5f;
            }
            else if (nearBuddy)  //おんぶしてない場合、バディをおんぶする
            {
                carryingBuddy = true;
                buddyPos.constraintActive = true;
                buddyController.moving = false;
            }
        }

        //ベルを鳴らしてバディを誘導
        if (!carryingBuddy && Input.GetKeyDown(KeyCode.B))
        {
            buddyController.GuideTo(gameObject.transform.position.x);
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
