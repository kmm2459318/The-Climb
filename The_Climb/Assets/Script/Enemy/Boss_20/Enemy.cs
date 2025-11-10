using UnityEngine;


public class Enemy : MonoBehaviour
{

    [Header("この敵がいるエリア名")]
    public string areaName = "草原"; 
    private EnemyStats stats;　　　　　　　//エネミーの基本情報
    private EnemyGeneration Generate;　　　//エネミー出現機能
    private EnemyDataBase dbManager;　　　 //エネミーのやっつけた数の判定
    public DropTable dropTable;            //ドロップアイテム

    private void Start()
    {
        // 同じシーン内にある EnemyDataBase を探して取得
        dbManager = FindFirstObjectByType<EnemyDataBase>();

        if (dbManager == null)
        {
            Debug.LogError("⚠️ EnemyDataBase がシーン内に見つかりません！");
        }
    }

    //出現した時に出すダイアログ
    public void SetUp(EnemyStats data , EnemyGeneration Spawner)
    {
        stats = data;
        Generate = Spawner;

        Debug.Log($"{stats.EnemyName}（HP:{stats.HP} 攻撃:{stats.AttackPower}）生成");
    }

    //プレイヤーと接触した際の処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
      
            dbManager.AddOrUpdateKillData(stats, areaName);

            Generate.RemoveEnemy(gameObject);
            Destroy(gameObject);
            if (stats.Period == "過去")
            {
                DropItem();
            }
        }
    }

    void DropItem()
    {
        foreach (var item in dropTable.PossibleItems)
        {
            float roll = Random.value;
            if (roll < item.DropRate)
            {
                Instantiate(item.DropPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
