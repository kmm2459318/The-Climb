using UnityEngine;
using UnityEngine.Animations;
using TMPro;

public class BuddyCarry : MonoBehaviour
{
    private GameObject buddy;             //Buddyのゲームオブジェクト
    public BuddyController buddyController;  //Buddyのスクリプト
    public PositionConstraint buddyPos;  //BuddyのPositionConstraint（おんぶに使ってる追従のコンポーネント）
    private PlayerState state;
    private PlayerMove playerMove;
    private GameObject hukidashi;     //吹き出しオブジェクト
    private TextMeshPro hukidashiText;  //吹き出しテキスト

    public bool nearBuddy = false;       //Buddyが近くにいるか判定
    private bool nearCallBell = false;    //CallBellが近くにあるか判定
    private float callBellPosX = 0;       //CallBellのX座標
    private float callCooldown = 1.0f;    //CallBellのクールダウン時間
    private float callCooldownTimer = 0f; //CallBellのクールダウンタイマー

    [Header("コールベルの音")]
    [SerializeField] private AudioClip callBellSound;
    private AudioSource audioSource;

    void Start()
    {
        state = GetComponent<PlayerState>();
        playerMove = state.move;
        hukidashi = state.hukidashi;
        hukidashiText = state.hukidashiText;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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
        //コールベルのクールダウン処理
        if (callCooldown > callCooldownTimer)
        {
            callCooldownTimer += Time.deltaTime;
        }

        if (buddy != null)
        {
            //向いてる方向によっておんぶしてるバディの場所を調整
            if (!buddyController.beingKidnapped)
            {
                bool isUpsideDown = playerMove != null && playerMove.IsUpsideDown;
                float offsetY = isUpsideDown ? -1f : 1f;

                if (state.playerDirectionRight)
                {
                    buddyPos.translationOffset = new Vector3(-0.3f, offsetY, 0f);
                    if (state.carryingBuddy)
                        buddy.transform.localScale = new Vector3(0.58f, 0.58f, 0.58f);
                }
                else
                {
                    buddyPos.translationOffset = new Vector3(0.3f, offsetY, 0f);
                    if (state.carryingBuddy)
                        buddy.transform.localScale = new Vector3(-0.58f, 0.58f, 0.58f);
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
                    buddyPos.constraintActive = false;
                    buddy.transform.position = transform.position + Vector3.up * 0.5f;
                    state.carryingBuddy = false;
                }
                else if (nearBuddy && !state.carryingBuddy)  //おんぶしてない場合、バディをおんぶする
                {
                    buddyPos.constraintActive = true;
                    buddyController.moving = false;
                    state.carryingBuddy = true;
                }
                else if (!state.carryingBuddy && nearCallBell && callCooldown <= callCooldownTimer)  //ベルを鳴らしてバディを誘導
                {
                    if (callBellSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(callBellSound);
                    }

                    buddyController.GuideTo(callBellPosX);

                    if (callBellPosX - buddy.transform.position.x > 0)
                    {
                        buddy.transform.localScale = new Vector3(0.58f, 0.58f, 0.58f);
                    }
                    else if (callBellPosX - buddy.transform.position.x < 0)
                    {
                        buddy.transform.localScale = new Vector3(-0.58f, 0.58f, 0.58f);
                    }
                    callCooldownTimer = 0f;  //クールダウンリセット
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CallBell"))
        {
            nearCallBell = true;  //CallBellが近くにある
            callBellPosX = other.transform.position.x;
            //吹き出し表示
            hukidashi.SetActive(true);
            hukidashiText.text = "E";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CallBell"))
        {
            nearCallBell = false;  //CallBellが近くにない
            //吹き出し非表示
            hukidashi.SetActive(false);
        }
    }
}
