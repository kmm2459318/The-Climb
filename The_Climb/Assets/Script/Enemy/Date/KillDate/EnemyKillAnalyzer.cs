using UnityEngine;
using SQLite4Unity3d;
using System.Linq;
using System.IO;

public class EnemyKillAnalyzer : MonoBehaviour
{
    private SQLiteConnection _connection;

    private void Awake()
    {
        // データベースの分析
        string DbPath = Path.Combine(Application.persistentDataPath, "EnemyKillData.db");
        _connection = new SQLiteConnection(DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        // テーブルがない場合は作成
        _connection.CreateTable<EnemyKillData>();
    }

    /// <summary>
    /// 敵の撃破データの割合を計算して表示
    /// </summary>
    [ContextMenu("Show Kill Ratio")]
    public void ShowKillRatio()
    {
        var allData = _connection.Table<EnemyKillData>().ToList();

        if (allData.Count == 0)
        {
            Debug.Log("⚠ データがまだありません。敵を倒してから再度お試しください。");
            return;
        }

        // ✅ ① 先に合計撃破数を求める
        int totalKills = allData.Sum(e => e.KillCount);

        // ✅ ② 敵ごとに集計
        var result = allData
            .GroupBy(e => e.EnemyName)
            .Select(g => new
            {
                EnemyName = g.Key,
                TotalKills = g.Sum(e => e.KillCount),
                Ratio = (float)g.Sum(e => e.KillCount) / totalKills * 100f
            })
            .OrderByDescending(x => x.TotalKills)
            .ToList();

        // ✅ ③ 結果をログに出力
        Debug.Log($"📊 撃破データ分析結果（合計 {totalKills} 体）");
        foreach (var r in result)
        {
            Debug.Log($"・{r.EnemyName}：{r.TotalKills}体 ({r.Ratio:F1}%)");
        }
    }
}