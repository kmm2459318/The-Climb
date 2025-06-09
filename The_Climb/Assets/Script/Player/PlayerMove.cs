using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //プレイヤーのRigidBody
    Rigidbody RigidBody;
    //プレイヤーの移動速度
    private float MoveSpeed = 3.0f;
    //プレイヤーの移動方向
    private float MoveInput = 0f;

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //移動
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) ||
            !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))  //止まる
        {
            MoveInput = 0f;
        }
        else if (Input.GetKey(KeyCode.A))  //左移動
        {
            MoveInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))  //右移動
        {
            MoveInput = 1f;
        }
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(MoveInput, 0, 0) * MoveSpeed * Time.fixedDeltaTime;

        // 現在位置 + 移動量
        RigidBody.MovePosition(RigidBody.position + move);
    }
}
