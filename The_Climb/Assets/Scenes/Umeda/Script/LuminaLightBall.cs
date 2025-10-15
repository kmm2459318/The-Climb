using UnityEngine;

public class LuminaLightBall : MonoBehaviour
{
    [Header("発射設定")]
    public GameObject lightBallPrefab;   // 弾のPrefab（Rigidbody + 子にCollider + Light + LightRange付き）
    public Transform shootPoint;         // 発射位置
    public Vector3 shootDirection = new Vector3(1, 1, 0); // 発射角度
    public float shootSpeed = 10f;       // 発射速度

    [Header("バウンド設定")]
    public int maxBounces = 3;           // 最大バウンド回数
    public float lifeTime = 5f;          // 弾の寿命（秒）

    [Header("再生成設定")]
    public float respawnTime = 2f;       // 再発射間隔

    private float timer;

    void Update()
    {
        if (lightBallPrefab == null || shootPoint == null) return;

        timer += Time.deltaTime;
        if (timer >= respawnTime)
        {
            SpawnBall();
            timer = 0f;
        }
    }

    void SpawnBall()
    {
        GameObject ball = Instantiate(lightBallPrefab, shootPoint.position, Quaternion.identity);

        // Rigidbody に初速を与える
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootDirection.normalized * shootSpeed;
        }

        // 弾の挙動を管理するコンポーネントを追加
        BallBehaviour behaviour = ball.AddComponent<BallBehaviour>();
        behaviour.maxBounces = maxBounces;
        behaviour.lifeTime = lifeTime;
    }

    // ------------------------------
    // 内部クラス: 弾の挙動
    // ------------------------------
    private class BallBehaviour : MonoBehaviour
    {
        public int maxBounces;
        public float lifeTime;

        private int bounceCount = 0;
        private SphereCollider ballCollider;

        void Start()
        {
            // 子の LuminaLight の Collider を取得
            ballCollider = GetComponentInChildren<SphereCollider>();

            // lifeTime 秒後に Collider を縮小して Destroy
            Invoke(nameof(DisableColliderAndDestroy), lifeTime);
        }

        void OnCollisionEnter(Collision collision)
        {
            bounceCount++;
            if (bounceCount >= maxBounces)
            {
                DisableColliderAndDestroy();
            }
        }

        private void DisableColliderAndDestroy()
        {
            if (ballCollider != null)
            {
                ballCollider.radius = 0f; // Collider を無効化して OnTriggerExit を発火
            }

            Destroy(gameObject, 0.1f); // 少し遅延して Destroy
        }
    }
}
