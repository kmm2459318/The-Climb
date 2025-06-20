using UnityEngine;

public class BossBullet : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.Particle[] m_Particles;

    public float threshold = 100f;   // 加速度の最大値制限
    public float intensity = 1f;     // 加速の強さ
    public Transform target;         // 追尾対象

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        // 必要な分だけ一度だけ確保（性能改善）
        m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void Update()
    {
        if (target == null) return; // ターゲットがない場合はスキップ

        int numParticlesAlive = ps.GetParticles(m_Particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            // ワールド座標・ワールド速度に変換
            Vector3 velocity = ps.transform.TransformDirection(m_Particles[i].velocity);
            Vector3 position = ps.transform.TransformPoint(m_Particles[i].position);

            float period = m_Particles[i].remainingLifetime * 0.9f;

            // 追尾すべき方向
            Vector3 diff = target.position - position;

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
        }

        // 変更を反映
        ps.SetParticles(m_Particles, numParticlesAlive);
    }
}