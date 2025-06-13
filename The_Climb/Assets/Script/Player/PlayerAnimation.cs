using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private int spacePressCount = 0;

    void Start()
    {
        // Animator取得
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator が見つかりません。コンポーネントをアタッチしてください。");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressCount++;
            int animStep = (spacePressCount - 1) % 3 + 1;

            Debug.Log("スペース押された → アニメーション Step " + animStep);

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
        animator.SetTrigger("JumpAnimStep1");
    }

    void JumpAnimStep2()
    {
        animator.SetTrigger("JumpAnimStep2");
    }

    void JumpAnimStep3()
    {
        animator.SetTrigger("JumpAnimStep3");
    }
}