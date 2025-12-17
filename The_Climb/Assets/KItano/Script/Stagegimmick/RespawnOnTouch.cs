using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerRespawnUmeda respawn = collision.collider.GetComponent<PlayerRespawnUmeda>();
           
            if (respawn != null)
            {
                respawn.Respawn();
            }
        }
    }
}
