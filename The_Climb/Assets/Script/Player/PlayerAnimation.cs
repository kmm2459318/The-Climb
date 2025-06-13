using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private int spacePressCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressCount++;
            int animStep = (spacePressCount - 1) % 3 + 1; // 1 → 2 → 3 → 1 と循環

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
        Debug.Log("アニメーション 1 を再生");
        // ここに Animation1 再生処理を記述
    }

    void JumpAnimStep2()
    {
        Debug.Log("アニメーション 2 を再生");
        // ここに Animation2 再生処理を記述
    }

    void JumpAnimStep3()
    {
        Debug.Log("アニメーション 3 を再生");
        // ここに Animation3 再生処理を記述
    }
}
