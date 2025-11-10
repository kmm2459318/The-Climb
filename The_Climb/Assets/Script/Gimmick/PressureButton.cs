using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [SerializeField] private enum GimmickType  //ギミックの種類
    { 
        appear = 0,
        move = 1,
        moveLoop = 2,

    };
    /*
    appear：出現ギミック
    move：移動ギミック
    moveLoop：移動ギミック（ループ）
    */ 
    private GimmickType type;  //ボタンの種類

    [Header("continuouslyがONなら\n一度押したら反応し続けるボタンに")]
    [SerializeField] private bool continuously = false;  //一度押したら継続的に押され続ける仕様か

    [SerializeField] private GameObject target;  //ギミックの対象物
    private GameObject buttonModel;  //ボタンのモデル
    [SerializeField] private Vector3 movePoint;  //ギミック：moveの向かう地点

    private int pressCount = 0;  //現在押してる数
    private float posY = 0;  //ボタンのＹ座標
    private bool isPress = false;  //押してるか押してないか判定

    void Start()
    {
        posY = transform.position.y;

        //targetを子オブジェクトから取得
        if (transform.childCount > 0)
        {
            buttonModel = transform.GetChild(0).gameObject;
        }

        if (target == null)
            Debug.LogError(gameObject.name + "のtargetがnullです。ばーか❤");
        else
        {
            target.SetActive(false);
        }
        if (movePoint == Vector3.zero && (type == GimmickType.move || type == GimmickType.moveLoop))
            Debug.LogError(gameObject.name + "のmovePointが初期値です。ばーか❤");

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

        switch (type)
        {
            case GimmickType.appear:
                AppearGimmick(true);
                break;
            case GimmickType.move:
                MoveGimmick(true);
                break;
            case GimmickType.moveLoop:
                MoveGimmick(true);
                break;
        }
    }

    private void PullSwitch()  //押されていないとき
    {
        buttonModel.transform.position = new Vector3(transform.position.x, posY, 0);

        switch (type)
        {
            case GimmickType.appear:
                AppearGimmick(false);
                break;
            case GimmickType.move:
                MoveGimmick(false);
                break;
            case GimmickType.moveLoop:
                MoveGimmick(false);
                break;
        }
    }

    private void AppearGimmick(bool on)
    {
        target.SetActive(on);
    }

    private void MoveGimmick (bool on)
    {

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
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Buddy"))
        {
            pressCount++;
            PressSwtich();
        }
    }
}
