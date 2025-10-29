using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.GraphicsBuffer;

public class StalkerHandController : MonoBehaviour
{
    private GameObject buddy;
    private BuddyController buddyController;
    private GameObject player;
    private PlayerState playerState;
    private PlayerKnockBack playerKnock;

    private GameObject mainStalker;
    private GameObject childStalker1;
    private GameObject childStalker2;
    private GameObject childStalker3;

    private enum stalkerHand {wait, stalk, stop};
    private stalkerHand stalkerState = stalkerHand.stalk;

    private float waitTime = 1.0f;       //行動：Waitの時間
    private float stalkTime = 2.0f;      //行動：Stalkの時間
    private float stopTime = 0.5f;       //行動：Stopの時間
    private float stalkerTimer = 0f;     //行動ローテーション用タイマー
    private float speed = 8.0f;          //移動速度
    public bool isKidnapping = false;   //誘拐中用判定
    private Vector3 stalkTarget = Vector3.zero;

    void Start()
    {
        if (GameObject.FindWithTag("Buddy") != null)
        {
            buddy = GameObject.FindWithTag("Buddy");
            buddyController = buddy.GetComponent<BuddyController>();
        }

        if (GameObject.FindWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player");
            playerState = player.GetComponent<PlayerState>();
            playerKnock = player.GetComponent<PlayerKnockBack>();
        }

        mainStalker = transform.GetChild(0).gameObject;
        childStalker1 = transform.GetChild(1).gameObject;
        childStalker2 = transform.GetChild(2).gameObject;
        childStalker3 = transform.GetChild(3).gameObject;
    }

    void Update()
    {
        //誘拐中
        if (isKidnapping)
        {
            transform.LookAt(Vector3.zero);
            transform.Translate(transform.forward * (speed / 2) * Time.deltaTime, Space.World);
        }
        else  //追跡行動ローテーション
        {
            if (buddyController.beingKidnapped)
            {
                stalkTarget = player.transform.position;
            }
            else
            {
                stalkTarget = buddy.transform.position;
            }

                //行動ローテーション用タイマー
                stalkerTimer += Time.deltaTime;

            //行動：Wait
            if (stalkerTimer <= waitTime)
            {
                stalkerState = stalkerHand.wait;
                transform.LookAt(stalkTarget);
            }
            else if (stalkerTimer <= waitTime + stalkTime)  //行動：Stalk
            {
                stalkerState = stalkerHand.stalk;
                transform.Translate(mainStalker.transform.forward * speed * Time.deltaTime, Space.World);
            }
            else  //行動：Stop
            {
                stalkerState = stalkerHand.stop;

                if (stalkerTimer > waitTime + stalkTime + stopTime)
                {
                    stalkerTimer = 0f;
                }
            }
        }
    }



    //Buddyを横取り！
    private void BuddyGet()
    {
        isKidnapping = true;
        buddyController.beingKidnapped = true;
        buddyController.SetConstraintTarget(this.transform);
    }

    //Buddy救出＆その敵消滅
    public void ReleaseBuddy()
    {
        buddyController.beingKidnapped = false;
        buddyController.SetConstraintTarget(player.transform);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!buddyController.beingKidnapped)
        {
            if (other.CompareTag("Buddy") && !playerState.carryingBuddy)  //Buddyが孤立してる場合
            {
                BuddyGet();
            }
            else if (other.CompareTag("Player") && playerState.carryingBuddy)  //Buddyをおんぶしてる場合
            {
                //敵とプレイヤーの位置でノックバックの方向を決める
                int dir = mainStalker.transform.position.x - other.gameObject.transform.position.x <= 0 ? 1 : -1;
                playerKnock.DoKnockBack(dir); //ノックバック
                BuddyGet();
            }
        }
    }
}
