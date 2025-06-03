using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //�v���C���[��RigidBody
    Rigidbody RigidBody;
    //�v���C���[�̈ړ����x
    private float MoveSpeed = 3.0f;
    //�v���C���[�̈ړ�����
    private float MoveInput = 0f;

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //�ړ�
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) ||
            !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))  //�~�܂�
        {
            MoveInput = 0f;
        }
        else if (Input.GetKey(KeyCode.A))  //���ړ�
        {
            MoveInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))  //�E�ړ�
        {
            MoveInput = 1f;
        }
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(MoveInput, 0, 0) * MoveSpeed * Time.fixedDeltaTime;

        // ���݈ʒu + �ړ���
        RigidBody.MovePosition(RigidBody.position + move);
    }
}
