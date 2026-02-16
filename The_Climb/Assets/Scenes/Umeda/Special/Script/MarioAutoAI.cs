using UnityEngine;
using static MarioController;

public class MarioAutoAI : MonoBehaviour
{
    [Header("移動設定")]
    public float directionChangeInterval = 3f;
    public float jumpInterval = 2.5f;
    public float dashChance = 0.4f;

    float dirTimer;
    float jumpTimer;

    int currentDirection = 1;

    void Update()
    {
        HandleDirection();
        HandleJump();
        HandleDash();
    }

    void HandleDirection()
    {
        dirTimer += Time.deltaTime;

        if (dirTimer >= directionChangeInterval)
        {
            dirTimer = 0;
            currentDirection = Random.value > 0.5f ? 1 : -1;
        }

        MarioInput.Left = currentDirection == -1;
        MarioInput.Right = currentDirection == 1;
    }

    void HandleJump()
    {
        jumpTimer += Time.deltaTime;

        if (jumpTimer >= jumpInterval)
        {
            jumpTimer = 0;
            MarioInput.Jump = true;
            Invoke(nameof(ResetJump), 0.1f);
        }
    }

    void ResetJump()
    {
        MarioInput.Jump = false;
    }

    void HandleDash()
    {
        MarioInput.Dash = Random.value < dashChance;
    }

    void OnDisable()
    {
        // 停止時は入力リセット
        MarioInput.Left = false;
        MarioInput.Right = false;
        MarioInput.Jump = false;
        MarioInput.Dash = false;
    }
}
