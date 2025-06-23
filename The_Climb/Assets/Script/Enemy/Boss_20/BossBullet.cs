using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 5f;

    public GameObject player;               // 発射時に代入されるプレイヤー（Inspectorではなくスクリプトから）
    private Vector3 targetPosition;
    private bool initialized = false;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Playerタグのオブジェクトが見つかりません！");
            return;
        }

        // プレイヤーの現在の位置を取得
        targetPosition = player.transform.position;

        initialized = true;

        // 一定時間後に自動で破棄
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!initialized) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}
