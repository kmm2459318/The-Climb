using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    PlayerBarrier playerBarrier;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerBarrier = player.GetComponentInChildren<PlayerBarrier>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (!playerBarrier.TryBlockAttack())
            {
                PlayerRespawnUmeda respawn = collision.collider.GetComponent<PlayerRespawnUmeda>();

                if (respawn != null)
                {
                    respawn.Respawn();
                }
            }
        }
    }
}
