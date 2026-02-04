using UnityEngine;

public class PressureButton : MonoBehaviour
{
    private enum GimmickType  //ギミックの種類（種類を増やしたい場合はここに追加）
    { 
        appear = 0,
        destroy = 1
    };
    /*
    appear：出現ギミック
    destroy：消滅ギミック
    */
    [SerializeField] private GimmickType type;  //ボタンの種類

    [Header("continuouslyがONなら\n一度押したら反応し続けるボタンに")]
    [SerializeField] private bool continuously = false;  //一度押したら継続的に押され続ける仕様か

    [Header("リスポーン後も状態を維持するか")]
    public bool keepStateAfterRespawn = false;
    [Header("識別用ID（維持する場合必須）")]
    public string buttonID = "";

    [SerializeField] private GameObject target;  //ギミックの対象物
    private PlayerState playerState;
    private GameObject buttonModel;  //ボタンのモデル
    [SerializeField] private Vector3 movePoint;  //ギミックの：moveの向かう地点

    public int pressCount = 0;  //現在押してる数
    private float posY = 0;  //ボタンのＹ座標

    void Awake()
    {
        posY = transform.position.y;
    }

    void Start()
    {
        playerState = FindAnyObjectByType<PlayerState>();

        //targetを子オブジェクトから取得
        if (transform.childCount > 0)
        {
            buttonModel = transform.GetChild(0).gameObject;
        }

        if (target == null)
            Debug.LogError(gameObject.name + "のtargetがnullです。ばーか❤");
        else if (type == GimmickType.appear)
        {
            target.SetActive(false);
        }
    }

    void Update()
    {
        //押されてるとき
        //if (isPress)
        //{
        //    PressSwtich();
        //}
        //else  //押されてないとき
        //{
        //    PullSwitch();
        //}
    }

    private void PressSwtich()  //押されているとき
    {
        buttonModel.transform.position = new Vector3(transform.position.x,  posY - 0.18f, 0);

        //ギミックの処理（種類を増やしたい場合はここに関数を追加）
        switch (type)
        {
            case GimmickType.appear:
                AppearGimmick(true);
                break;
            case GimmickType.destroy:
                AppearGimmick(false);
                break;
        }
    }

    private void PullSwitch()  //押されていないとき
    {
        buttonModel.transform.position = new Vector3(transform.position.x, posY, 0);

        //ギミックの処理
        switch (type)
        {
            case GimmickType.appear:
                AppearGimmick(false);
                break;
            case GimmickType.destroy:
                AppearGimmick(true);
                break;
        }
    }

    //出現・消滅のギミック処理
    private void AppearGimmick(bool on)
    {
        target.SetActive(on);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!continuously)  //継続的ボタンか否か
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Buddy"))
            {
                pressCount--;
                if (pressCount == 0)
                    PullSwitch();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || (other.gameObject.CompareTag("Buddy") && !playerState.carryingBuddy))
        {
            pressCount++;
            PressSwtich();
        }
    }

    //リスポーン後に強制的に押下状態にする
    public void SetPressedInstant()
    {
        if (pressCount == 0)
        {
            pressCount = 1; //とりあえず1にする
        }
        PressSwtich();
    }
}
