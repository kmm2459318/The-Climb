using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    public int damage = 1; // 即死させたいなら maxHP 以上

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}
