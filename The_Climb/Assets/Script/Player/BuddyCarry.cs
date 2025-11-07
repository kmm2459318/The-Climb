using UnityEngine;
using UnityEngine.Animations;

public class BuddyCarry : MonoBehaviour
{
    private GameObject buddy;             //Buddyのゲームオブジェクト
    public BuddyController buddyController;  //Buddyのスクリプト
    private PositionConstraint buddyPos;  //BuddyのPositionConstraint（おんぶに使ってる追従のコンポーネント）
    PlayerState state;

    public bool nearBuddy = false;       //Buddyが近くにいるか判定
    private bool nearCallBell = false;    //CallBellが近くにあるか判定
    private float callBellPosX = 0;       //CallBellのX座標

    void Start()
    {
        state = GetComponent<PlayerState>();

        if (GameObject.Find("Buddy") != null)
        {
            buddy = GameObject.Find("Buddy");
            buddyController = buddy.GetComponent<BuddyController>();
            buddyPos = buddy.GetComponent<PositionConstraint>();
            //state.carryingBuddy = true;
        }
    }

    void Update()
    {
        if (buddy != null)
        {
            //向いてる方向によっておんぶしてるバディの場所を調整
            if (!buddyController.beingKidnapped)
            {
                if (state.playerDirectionRight)
                {
                    buddyPos.translationOffset = new Vector3(-0.4f, 1f, 0f);
                }
                else
                {
                    buddyPos.translationOffset = new Vector3(0.4f, 1f, 0f);
                }
            }
            else
            {
                buddyPos.translationOffset = new Vector3(0f, 1f, 0f);
            }

            //Carryボタン（仮）
            if (Input.GetKeyDown(KeyCode.U) && state.isGrounded)
            {
                if (state.carryingBuddy)  //おんぶしてる場合、バディを降ろす
                {
                    state.carryingBuddy = false;
                    buddyPos.constraintActive = false;
                    buddy.transform.position = transform.position + Vector3.up * 0.5f;
                }
                else if (nearBuddy)  //おんぶしてない場合、バディをおんぶする
                {
                    state.carryingBuddy = true;
                    buddyPos.constraintActive = true;
                    buddyController.moving = false;
                }
            }

            //ベルを鳴らしてバディを誘導
            if (!state.carryingBuddy && Input.GetKeyDown(KeyCode.I) && nearCallBell)
            {
                buddyController.GuideTo(callBellPosX);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Buddy"))
        {
            nearBuddy = true;  //Buddyが近くにいる
        }
        else if (other.CompareTag("CallBell"))
        {
            nearCallBell = true;  //CallBellが近くにある
            callBellPosX = other.transform.position.x;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Buddy"))
        {
            nearBuddy = false;  //Buddyが近くにいない
        }
        else if (other.CompareTag("CallBell"))
        {
            nearCallBell = false;  //CallBellが近くにない
        }
    }
}
