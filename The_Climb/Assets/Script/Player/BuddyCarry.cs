using UnityEngine;
using UnityEngine.Animations;

public class BuddyCarry : MonoBehaviour
{
    private GameObject buddy;             //Buddyのゲームオブジェクト
    public BuddyController buddyController;  //Buddyのスクリプト
    public PositionConstraint buddyPos;  //BuddyのPositionConstraint（おんぶに使ってる追従のコンポーネント）
    public PlayerState state;
    PlayerMove playerMove;

    public bool nearBuddy = false;       //Buddyが近くにいるか判定
    private bool nearCallBell = false;    //CallBellが近くにあるか判定
    private float callBellPosX = 0;       //CallBellのX座標

    void Start()
    {
        state = GetComponent<PlayerState>();
        playerMove = GetComponent<PlayerMove>();

        if (GameObject.Find("Buddy") != null)
        {
            buddy = GameObject.Find("Buddy");
            buddyController = buddy.GetComponent<BuddyController>();
            buddyPos = buddy.GetComponent<PositionConstraint>();
            state.carryingBuddy = true;
        }
    }

    void Update()
    {
        if (buddy != null)
        {
            //向いてる方向によっておんぶしてるバディの場所を調整
            if (!buddyController.beingKidnapped)
            {
                bool isUpsideDown = playerMove != null && playerMove.IsUpsideDown;
                float offsetY = isUpsideDown ? -1f : 1f;

                if (state.playerDirectionRight)
                {
                    buddyPos.translationOffset = new Vector3(-0.4f, offsetY, 0f);
                }
                else
                {
                    buddyPos.translationOffset = new Vector3(0.4f, offsetY, 0f);
                }
            }
            else
            {
                buddyPos.translationOffset = new Vector3(0f, 1f, 0f);
            }

            //Carryボタン（仮）
            if (Input.GetKeyDown(KeyCode.E))
            {
                // おんぶ解除：接地中かつおんぶ中かつ反転していない場合のみ可能
                if (state.isGrounded && state.carryingBuddy && (playerMove == null || !playerMove.IsUpsideDown))
                {
                    state.carryingBuddy = false;
                    buddyPos.constraintActive = false;
                    buddy.transform.position = transform.position + Vector3.up * 0.5f;
                }
                else if (nearBuddy && !state.carryingBuddy)  //おんぶしてない場合、バディをおんぶする
                {
                    state.carryingBuddy = true;
                    buddyPos.constraintActive = true;
                    buddyController.moving = false;
                }
                else if (!state.carryingBuddy && nearCallBell)  //ベルを鳴らしてバディを誘導
                {
                    buddyController.GuideTo(callBellPosX);
                }
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("CallBell"))
        {
            nearCallBell = true;  //CallBellが近くにある
            callBellPosX = other.transform.position.x;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CallBell"))
        {
            nearCallBell = false;  //CallBellが近くにない
        }
    }
}
