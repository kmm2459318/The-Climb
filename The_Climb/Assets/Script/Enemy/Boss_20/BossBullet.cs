using UnityEngine;

public class BossBullet : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] m_Particles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
        int numParticlesAlive = ps.GetParticles(m_Particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            // ここでパーティクル毎に計算結果を適用する
            //m_Particles[i].velocity ;
        }
        ps.SetParticles(m_Particles, numParticlesAlive);
    }
}


