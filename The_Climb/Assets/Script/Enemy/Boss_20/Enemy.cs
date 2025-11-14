using UnityEngine;


public class Enemy : MonoBehaviour
{

    [Header("この敵がいるエリア名")]
    public string areaName = "草原"; 
    private EnemyStats stats;　　　　　　　//エネミーの基本情報
    private EnemyGeneration Generate;　　　//エネミー出現機能
    private EnemyDataBase dbManager;　　　 //エネミーのやっつけた数の判定
    public DropTable dropTable;            //ドロップアイテム
    public int e_HP = 0;                   

    private bool e_isDead = false;                 //死んだときの2重処理防止 

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
        e_HP = stats.HP;
        Generate = Spawner;

        Debug.Log($"{stats.EnemyName}（HP:{stats.HP} 攻撃:{stats.AttackPower}）生成");
    }

    public void TakeDamage(int damage)
    {
        if(e_isDead) return;
        if(e_HP > damage)
        {
            e_HP -= damage;
            Debug.Log($"{stats.EnemyName}がダメージ{damage}を受けた残りHP{e_HP}");
        }

        else if(e_HP <= damage)
        {
            Die();
        }
    }

    //死んだ際の処理
    public void Die()
    {
        if (e_isDead) return; // 二重処理防止
        e_isDead = true;

        // 撃破数を記録
        if (dbManager != null)
        {
            dbManager.AddOrUpdateKillData(stats, areaName);
        }

        // 生成元に削除を通知
        if (Generate != null)
        {
            Generate.RemoveEnemy(gameObject);
        }

        // ドロップ判定
        if (stats != null && stats.Period == "過去")
        {
            DropItem();
        }

        Debug.Log($"{stats?.EnemyName ?? "不明な敵"}が倒されました");
        Destroy(gameObject);
        e_isDead = false;
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
