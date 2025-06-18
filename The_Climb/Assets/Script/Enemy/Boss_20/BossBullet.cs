using UnityEngine;

public class BossBullet : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] m_Particles;

    public float threshold = 100f;
    public float intensity = 1f;

    public Transform target;

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
            Vector3 velocity = ps.transform.TransformDirection(m_Particles[i].velocity);
            Vector3 position = ps.transform.TransformPoint(m_Particles[i].position);
            float period = m_Particles[i].remainingLifetime * 0.9f;

            Vector3 diff = target.position - position;
            Vector3 accel = (diff - velocity * period) * 2f / (period * period);

            if (accel.magnitude > threshold)
            {
                accel = accel.normalized * threshold;
            }

            velocity += accel * Time.deltaTime * intensity;

            m_Particles[i].velocity = ps.transform.InverseTransformDirection(velocity);
        }

        ps.SetParticles(m_Particles, numParticlesAlive);
    }
}


