using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public enum GimmickType  //ギミックの種類
    { 
        appear = 0
    };
    /*
    appear：出現ギミック
    */ 
    public GimmickType type;  //ボタンの種類

    private GameObject target;  //ギミックの対象物

    private float posY = 0;  //ボタンのＹ座標
    private bool isPress = false;  //押してるか押してないか判定

    void Start()
    {
        posY = transform.position.y;

        //targetを子オブジェクトから取得
        if (transform.childCount > 0)
            target = transform.GetChild(0).gameObject;

        if (target == null)
            Debug.LogError(gameObject.name + "のtargetがnullです。ばーか❤");
    }

    void Update()
    {
        //押されてるとき
        //if (isPress)
        //{
        //    //PressSwtich();
        //}
        //else  //押されてないとき
        //{
        //    //PullSwitch();
        //}
    }

    private void PressSwtich()  //押されているとき
    {
        gameObject.transform.position = new Vector3(transform.position.x,  posY - 0.18f, 0);

        switch (type)
        {
            case GimmickType.appear:
                AppearGimmick(true);
                break;
                
        }
    }

    private void PullSwitch()  //押されていないとき
    {
        gameObject.transform.position = new Vector3(transform.position.x, posY, 0);

        switch (type)
        {
            case GimmickType.appear:
                AppearGimmick(false);
                break;

        }
    }

    private void AppearGimmick(bool on)
    {
        target.SetActive(on);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Buddy"))
        {
            isPress = false;
            PullSwitch();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Buddy"))
        {
            isPress = true;
            PressSwtich();
        }
    }
}
