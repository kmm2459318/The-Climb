using UnityEngine;

public class BossBullet : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.Particle[] m_Particles;

    public float threshold = 100f;   // 加速度の最大値制限
    public float intensity = 1f;     // 加速の強さ
    private Transform playerTransform;


    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }
    private void Start()
    {
        // プレイヤーのTransformを取得
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Playerが見つかりません。タグが正しいか確認してください。");
        }
    }
    void Update()
    {
        ForceUpdateParticles();
    }

    public void ForceUpdateParticles()
    {
        if (playerTransform == null)
        {
            Debug.Log("ターゲットが見つかりません");
            return;
        }

        int numParticlesAlive = ps.GetParticles(m_Particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {

            
            // ワールド座標・ワールド速度に変換
            Vector3 velocity = ps.transform.TransformDirection(m_Particles[i].velocity);
            Vector3 position = ps.transform.TransformPoint(m_Particles[i].position);

            float period = m_Particles[i].remainingLifetime * 0.9f;

            // 追尾すべき方向
            Vector3 diff = playerTransform.position - position;

            // 加速度を計算（等加速度運動の式）
            Vector3 accel = (diff - velocity * period) * 2f / (period * period);
         
            // 加速度が大きすぎる場合、最大値で制限
            if (accel.magnitude > threshold)
            {
                accel = accel.normalized * threshold;
              
            }

            // 加速度に基づいて速度を更新
            velocity += accel * Time.deltaTime * intensity;

            // ローカル座標系に戻して保存
            m_Particles[i].velocity = ps.transform.InverseTransformDirection(velocity);
            Debug.Log("生きているパーティクル数: " + numParticlesAlive);
        }

        // 変更を反映
        ps.SetParticles(m_Particles, numParticlesAlive);
    }
}