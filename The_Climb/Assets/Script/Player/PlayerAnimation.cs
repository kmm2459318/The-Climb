using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private int spacePressCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressCount++;
            int animStep = (spacePressCount - 1) % 3 + 1; // 1 �� 2 �� 3 �� 1 �Əz��

            switch (animStep)
            {
                case 1:
                    JumpAnimStep1();
                    break;
                case 2:
                    JumpAnimStep2();
                    break;
                case 3:
                    JumpAnimStep3();
                    break;
            }
        }
    }

    void JumpAnimStep1()
    {
        Debug.Log("�A�j���[�V���� 1 ���Đ�");
        // ������ Animation1 �Đ��������L�q
    }

    void JumpAnimStep2()
    {
        Debug.Log("�A�j���[�V���� 2 ���Đ�");
        // ������ Animation2 �Đ��������L�q
    }

    void JumpAnimStep3()
    {
        Debug.Log("�A�j���[�V���� 3 ���Đ�");
        // ������ Animation3 �Đ��������L�q
    }
}
