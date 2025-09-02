using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private RespawnGroundCheck respawnGroundCheckLeft;
    private RespawnGroundCheck respawnGroundCheckRight;

    private bool leftGroundCheck = false; //プレイヤーの左のリスポーン地面判定
    private bool rightGroundCheck = false; //プレイヤーの右のリスポーン地面判定
    private Vector3 lastSavePos; //リスポーン位置

    void Start()
    {
        respawnGroundCheckLeft = GameObject.Find("RespawnGroundCehckLeft").GetComponent<RespawnGroundCheck>();
        respawnGroundCheckRight = GameObject.Find("RespawnGroundCehckRight").GetComponent<RespawnGroundCheck>();
        lastSavePos = gameObject.transform.position;
    }

    void Update()
    {
        //リスポーンする地面のチェックを随時更新
        leftGroundCheck = respawnGroundCheckLeft.isRespawnGrounded;
        rightGroundCheck = respawnGroundCheckRight.isRespawnGrounded;

        //どっちもtrueの時リスポーン地点を随時更新
        if (leftGroundCheck && rightGroundCheck)
        {
            lastSavePos = transform.position + new Vector3(0, 0.1f, 0);
        }

        //落ちた判定(今は簡易)
        if (transform.position.y < -4.3f)
        {
            transform.position = lastSavePos;
        } 
    }
}
