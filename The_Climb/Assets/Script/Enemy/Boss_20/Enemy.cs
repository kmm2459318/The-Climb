using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("敵の基本情報")]
    public EnemyStats stats;

    [Header("この敵がいるエリア名")]
    public string areaName = "草原";

    private EnemyDataBase dbManager;

    private void Start()
    {
        // 同じシーン内にある EnemyDataBase を探して取得
        dbManager = FindFirstObjectByType<EnemyDataBase>();

        if (dbManager == null)
        {
            Debug.LogError("⚠️ EnemyDataBase がシーン内に見つかりません！");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dbManager.AddOrUpdateKillData(stats, areaName);
            Destroy(gameObject);
        }
    }
}
