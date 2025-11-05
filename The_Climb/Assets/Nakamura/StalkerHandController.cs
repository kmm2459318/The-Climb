using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

public class StalkerHandController : MonoBehaviour
{
    private GameObject buddy;
    [HideInInspector] public BuddyController buddyController;
    private GameObject player;
    [HideInInspector] public PlayerState playerState;
    [HideInInspector] public PlayerKnockBack playerKnock;

    [HideInInspector] public Transform mainStalker;
    private Transform childStalker1;
    private Transform childStalker2;
    private Transform childStalker3;
    private Transform[] stalkers = new Transform[4];

    private enum stalkerHand {wait, stalk, stop};
    private stalkerHand stalkerState = stalkerHand.stalk;

    private float waitTime = 1.0f;       //行動：Waitの時間
    private float stalkTime = 2.0f;      //行動：Stalkの時間
    private float stopTime = 0.5f;       //行動：Stopの時間
    private float stalkerTimer = 0f;     //行動ローテーション用タイマー
    private float speed = 7.0f;          //移動速度
    private float childRadius = 2f;      //追従の半径
    public bool isKidnapping = false;    //誘拐中用判定
    private Vector3 home;                //誘拐して連れて帰る地点
    private Vector3 stalkTarget = Vector3.zero;  //追跡ターゲット

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

        home = transform.position;

        mainStalker = transform.GetChild(0).gameObject.transform;
        childStalker1 = transform.GetChild(1).gameObject.transform;
        childStalker2 = transform.GetChild(2).gameObject.transform;
        childStalker3 = transform.GetChild(3).gameObject.transform;
        stalkers[0] = mainStalker;
        stalkers[1] = childStalker1;
        stalkers[2] = childStalker2;
        stalkers[3] = childStalker3;
    }

    void Update()
    {
        //誘拐中
        if (isKidnapping)
        {
            mainStalker.transform.LookAt(home);
            transform.Translate(mainStalker.transform.forward * (speed / 2) * Time.deltaTime, Space.World);
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

                //LookAt時にZ前→X前の補正を入れて狙いを定める
                mainStalker.transform.LookAt(stalkTarget);
                mainStalker.transform.Rotate(0f, -90f, 0f);
            }
            else if (stalkerTimer <= waitTime + stalkTime) //行動：Stalk
            {
                stalkerState = stalkerHand.stalk;

                //ターゲットまでの方向ベクトル
                Vector3 targetDir = (stalkTarget - mainStalker.transform.position).normalized;

                //今の方向ベクトル
                Vector3 currentDir = mainStalker.transform.right;

                //角度
                float diffY = targetDir.y - currentDir.y;

                //曲がる
                float rotSpeed = 60f * Time.deltaTime; //角度調整速度
                float rotZ = Mathf.Clamp(diffY * 20f, -rotSpeed, rotSpeed); //小さく安定させる

                mainStalker.transform.localEulerAngles += new Vector3(0f, 0f, rotZ);

                //進む
                mainStalker.transform.position += mainStalker.transform.right * speed * Time.deltaTime;
            }
            else //Stop
            {
                stalkerState = stalkerHand.stop;
                StartCoroutine(ChildPosReset());

                if (stalkerTimer > waitTime + stalkTime + stopTime)
                {
                    stalkerTimer = 0f;
                }
            }

            //他のストーカーハンドが捕まえたら消滅
            if (buddyController.beingKidnapped)
                Destroy(gameObject);
        }

        //ChildStalkerの挙動
        for (int i = 0; i < stalkers.Length - 1; i++)
            if (Vector3.Distance(stalkers[i].position, stalkers[i + 1].position) > childRadius)
            {
                //方向ベクトルを求める
                Vector3 direction = (stalkers[i + 1].position - stalkers[i].position).normalized;

                //childRadius の位置に押し戻す
                stalkers[i + 1].position = stalkers[i].position + direction * childRadius;
            }
    }

    //ChildStalkerをMainStalkerHandの位置に
    private IEnumerator ChildPosReset()
    {
        for (float j = 0; j < 0.5f; j += Time.deltaTime)
        {
            for (int i = 0; i < stalkers.Length - 1; i++)
                stalkers[i + 1].position = Vector3.Lerp(stalkers[i + 1].position, stalkers[i].position, 9f * Time.deltaTime);
            yield return null;
        }
    }

    //Buddyを取られる！
    public void BuddyGet()
    {
        isKidnapping = true;
        buddyController.beingKidnapped = true;
        buddyController.SetConstraintTarget(mainStalker.transform);
        StartCoroutine(ChildPosReset());
    }

    //Buddy救出＆その敵消滅
    public void ReleaseBuddy()
    {
        buddyController.beingKidnapped = false;
        buddyController.SetConstraintTarget(player.transform);
        Destroy(gameObject);
    }
}
