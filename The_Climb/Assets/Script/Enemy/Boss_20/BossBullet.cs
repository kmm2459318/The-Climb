using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public Boss_20_StatusObjectScript status;
    private float speed;
    public float lifeTime = 5f;
    public GameObject player;
    public float hitRadious = 0.5f;

    private Vector3 targetPosition;
    private Vector3 moveDirection;
    private bool initialized = false;

    void Start()
    {
        // シーン内の "Player" タグが付いたオブジェクトを探す
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Playerタグのオブジェクトが見つかりません！");
            return;
        }

        speed = status.Attack_Speed;

        // プレイヤーの現在の位置を取得
        targetPosition = player.transform.position;
        moveDirection = (targetPosition - transform.position).normalized;
        initialized = true;
        
        // 一定時間後に自動で破棄
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!initialized) return;
        transform.position += moveDirection * speed * Time.deltaTime;
        if(player != null && Vector3.Distance(transform.position,targetPosition) < hitRadious)
        {
            Debug.Log("ヒット！（自作物理）");
            Destroy(gameObject);
        }
    }
}