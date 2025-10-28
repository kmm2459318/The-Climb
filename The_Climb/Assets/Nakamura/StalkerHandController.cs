using UnityEngine;

public class StalkerHandController : MonoBehaviour
{
    private Transform buddy;

    private enum stalkerHand {wait, stalk, stop};
    private stalkerHand stalkerState = stalkerHand.stalk;

    private float waitTime = 1.0f;       //行動：Waitの時間
    private float stalkTime = 2.0f;      //行動：Stalkの時間
    private float stopTime = 0.5f;       //行動：Stopの時間
    private float stalkerTimer = 0f;     //行動ローテーション用タイマー
    private float speed = 8.0f;          //移動速度
    private bool isKidnapping = false;   //誘拐中用判定

    void Start()
    {
        if (GameObject.Find("Buddy") != null)
        {
            buddy = GameObject.Find("Buddy").GetComponent<Transform>();
        }
    }

    void Update()
    {
        //行動ローテーション用タイマー
        stalkerTimer += Time.deltaTime;

        //行動：Wait
        if (stalkerTimer <= waitTime)
        {
            stalkerState = stalkerHand.wait;
            transform.LookAt(buddy);
        }
        else if (stalkerTimer <= waitTime + stalkTime)  //行動：Stalk
        {
            stalkerState = stalkerHand.stalk;
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }
        else  //行動：Stop
        {
            stalkerState = stalkerHand.stop;

            if (stalkerTimer > waitTime + stalkTime + stopTime)
            {
                stalkerTimer = 0f;
            }
        }

        //誘拐中
        if (isKidnapping)
        {

        }
    }
}
