using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("この敵がいるエリア名")]
    public string areaName = "草原"; 
    private EnemyStats stats;
    private EnemyGeneration Generate;
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

    public void SetUp(EnemyStats data , EnemyGeneration Spawner)
    {
        stats = data;
        Generate = Spawner;

        Debug.Log($"{stats.EnemyName}（HP:{stats.HP} 攻撃:{stats.AttackPower}）生成");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dbManager.AddOrUpdateKillData(stats, areaName);
            Generate.RemoveEnemy(gameObject); // リストから削除
            Destroy(gameObject);
        }
    }
}
