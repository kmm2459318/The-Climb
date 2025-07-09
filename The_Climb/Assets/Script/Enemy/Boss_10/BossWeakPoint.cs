using UnityEngine;

public class BossWeakPoint : MonoBehaviour
{
    private BossEnemy_HevvyMovement boss;

    private void Start()
    {
        boss = GetComponentInParent<BossEnemy_HevvyMovement>();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && boss.IsVulnerable())
        {
            boss.OnHit(); // ボスにダメージ
            gameObject.SetActive(false); // 一時的に無効化
        }
    }

    public void ActivateWeakPoint() => gameObject.SetActive(true);
    public void DeactivateWeakPoint() => gameObject.SetActive(false);
}