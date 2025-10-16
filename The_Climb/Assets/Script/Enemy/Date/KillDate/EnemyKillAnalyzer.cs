using UnityEngine;
using SQLite4Unity3d;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Overlays;

public class EnemyKillAnalyzer : MonoBehaviour
{
    private EnemyDataBase dbManager;
    private SQLiteConnection Connection;

    private void Awake()
    {
        dbManager = FindFirstObjectByType<EnemyDataBase>();
        if(dbManager == null)
        {
            Debug.LogError("EnemyDataBaseが見つけられません");
            return;
        }

        Connection = dbManager._connection;
    }


    /// <summary>
    /// 敵の撃破データの割合を計算して表示
    /// </summary>
    [ContextMenu("Show Kill Ratio")]
    public void ShowKillRatio()
    {
        if(Connection == null)
        {
            Debug.Log("DB接続がありません");
            return;
        }

        string TempPath = dbManager.TempDbPath;
        string SavePath = dbManager.SaveDbPath;

        List<EnemyKillData> tempData = new List<EnemyKillData>();
        if (File.Exists(TempPath))
        {
            using (var tempConn = new SQLiteConnection(TempPath, SQLiteOpenFlags.ReadWrite))
            {
                tempData = tempConn.Table<EnemyKillData>().ToList();
            }
        }

  
        List<EnemyKillData> saveData = new List<EnemyKillData>();
        if (File.Exists(SavePath))
        {
            using (var saveConn = new SQLiteConnection(SavePath, SQLiteOpenFlags.ReadWrite))
            {
                saveData = saveConn.Table<EnemyKillData>().ToList();
            }
        }

        // 両方を統合
        var allData = tempData.Concat(saveData)
            .GroupBy(e => e.EnemyName)
            .Select(g => new EnemyKillData
            {
                EnemyName = g.Key,
                KillCount = g.Sum(x => x.KillCount)
            })
            .ToList();

        if (allData.Count == 0)
        {
            Debug.Log("データがまだありません。敵を倒してから再度お試しください。");
            return;
        }

        // 合計撃破数
        int totalKills = allData.Sum(e => e.KillCount);

        // 割合を計算してログ出力
        var result = allData
            .OrderByDescending(e => e.KillCount)
            .Select(e => new
            {
                e.EnemyName,
                e.KillCount,
                Ratio = (float)e.KillCount / totalKills * 100f
            }).ToList();

        Debug.Log($"📊 撃破データ分析結果（合計 {totalKills} 体）");
        foreach (var r in result)
        {
            Debug.Log($"・{r.EnemyName}：{r.KillCount}体 ({r.Ratio:F1}%)");
        }
    }
}