using UnityEngine;

public class LuminaLightBall : MonoBehaviour
{
    [Header("発射設定")]
    public GameObject lightBallPrefab;   // 弾のPrefab
    public Transform shootPoint;         // 発射位置
    public Vector3 shootDirection = new Vector3(1.015f, 1f, 0f);
    public float shootSpeed = 9.9f;

    [Header("バウンド設定")]
    public int maxBounces = 100;
    public float lifeTime = 5f;

    [Header("再生成設定")]
    public float respawnTime = 3.25f;

    [Header("サウンド設定")]
    public AudioClip bounceSound;
    [Range(0f, 1f)]
    public float volume = 50f;

    [Tooltip("音を鳴らす対象レイヤー（複数選択可）")]
    public LayerMask soundLayers;

    [Header("3Dサウンド距離設定")]
    public float minDistance = 8f;
    public float maxDistance = 30f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

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

        if (ball.TryGetComponent<Rigidbody>(out var rb))
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = shootDirection.normalized * shootSpeed;
#else
            rb.velocity = shootDirection.normalized * shootSpeed;
#endif
        }

        // 弾挙動管理
        BallBehaviour behaviour = ball.AddComponent<BallBehaviour>();
        behaviour.maxBounces = maxBounces;
        behaviour.lifeTime = lifeTime;
        behaviour.bounceSound = bounceSound;
        behaviour.volume = volume;
        behaviour.soundLayers = soundLayers;
        behaviour.minDistance = minDistance;
        behaviour.maxDistance = maxDistance;
        behaviour.rolloffMode = rolloffMode;
    }

    // ------------------------------
    // 内部クラス: 弾の挙動管理
    // ------------------------------
    private class BallBehaviour : MonoBehaviour
    {
        public int maxBounces;
        public float lifeTime;

        public AudioClip bounceSound;
        public float volume;
        public LayerMask soundLayers;

        public float minDistance;
        public float maxDistance;
        public AudioRolloffMode rolloffMode;

        private int bounceCount = 0;
        private Collider ballCollider;

        void Start()
        {
            ballCollider = GetComponentInChildren<Collider>();
            Invoke(nameof(DisableColliderAndDestroy), lifeTime);
        }

        void OnCollisionEnter(Collision collision)
        {
            // LayerMask で判定
            if (((1 << collision.gameObject.layer) & soundLayers) != 0)
            {
                PlayBounceSound();
            }

            bounceCount++;
            if (bounceCount >= maxBounces)
            {
                DisableColliderAndDestroy();
            }
        }

        void PlayBounceSound()
        {
            if (bounceSound == null) return;

            // AudioSource を生成して 3D設定 → 自動削除
            GameObject audioObj = new GameObject("LightBallSound");
            audioObj.transform.position = transform.position;

            AudioSource source = audioObj.AddComponent<AudioSource>();
            source.clip = bounceSound;
            source.volume = volume;
            source.spatialBlend = 1f; // 3D化
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.rolloffMode = rolloffMode;
            source.Play();

            Destroy(audioObj, bounceSound.length + 0.1f);
        }

        private void DisableColliderAndDestroy()
        {
            if (ballCollider != null)
                ballCollider.enabled = false;

            Destroy(gameObject, 0.1f);
        }
    }
}
