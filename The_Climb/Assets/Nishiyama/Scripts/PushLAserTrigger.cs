using UnityEngine;

public class PushLaserTrigger : MonoBehaviour
{
    public LaserKill laser;

    private bool used = false; // 一度使ったかどうか

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
            used = true;
            Debug.Log("【LaserSpawnTrigger】ポイント通過");

            laser.AppearAndStartPush();
        }
    }

    public void ResetTrigger()
    {
        used = false;
    }
}
