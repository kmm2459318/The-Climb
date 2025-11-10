using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThroughBlockPlayer : MonoBehaviour
{

    [SerializeField] private Rigidbody rbody;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Vector3 groundOffset;

    private Vector3 moveDirection;
    private bool isJump;
    private bool isCrouch;

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        isJump = Input.GetButtonDown("Jump");
        isCrouch = Input.GetAxis("Vertical") <= -0.1;       // 下ボタン入力時

        Jump();
        DescendPlatform();
    }

    /// <summary>
    /// 更新処理（一定間隔）
    /// </summary>
    private void FixedUpdate()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    private void Jump()
    {
        if (isJump && !isCrouch)
        {
            rbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// 足場から降りる
    /// </summary>
    private void DescendPlatform()
    {
        // しゃがみ状態でジャンプ入力時に降りる
        if (isCrouch && isJump)
        {
            var radius = 1.0f;
            var layerNo = LayerMask.NameToLayer("Platform");
            var platforms = Physics.OverlapSphere(transform.position + groundOffset, radius, 1 << layerNo)
                                    .Select(platform => platform.GetComponent<Platform>())
                                    .OrderBy(platform => platform?.transform.position.x)
                                    .ToList();

            if (!platforms.Any()) { return; }

            // 範囲内の足場を全て無効化する
            foreach (var platform in platforms)
            {
                platform.DisablePlatform();
            }
        }
    }
}
