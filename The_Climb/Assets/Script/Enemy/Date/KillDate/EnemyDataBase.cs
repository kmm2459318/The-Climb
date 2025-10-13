using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;
using System;
public class EnemyDataBase : MonoBehaviour
{
   private SQLiteConnection _connection;

    /// <summary>
    /// データベースの保存場所
    /// </summary>
    private void Awake()
    {
        string DbPath = Path.Combine(Application.persistentDataPath, "EnemyKillData.db");
        _connection = new SQLiteConnection(DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        //テーブルが無ければ作成
        _connection.CreateTable<EnemyKillData>();
        
    }
    
    ///<summray>
    ///敵撃破データを追加又は更新
    ///</summray>
    public void AddOrUpdateKillData(EnemyStats stats, string areaName)
    {
        //既存データ検索
        var existingDate = _connection.Table<EnemyKillData>()
        .FirstOrDefault(e => e.EnemyID == stats.ID && e.AreaName == areaName);

        string now = DateTime.Now.ToString("yyy/MM/dd HH:mm:ss");

        if(existingDate != null)
        {
            existingDate.KillCount += 1;
            existingDate.LastKillTime = now;
            _connection.Update(existingDate);
            Debug.Log($"【撃破記録】{existingDate.EnemyName} を倒した！ " +
                  $"場所: {existingDate.AreaName} / 累計: {existingDate.KillCount} 回 / 最終: {existingDate.LastKillTime}");
        }

        else
        {
            var newDate = new EnemyKillData(stats, areaName, 1, now);
            _connection.Insert(newDate);
            Debug.Log($"【撃破記録】{newDate.EnemyName} を倒した！ " +
                 $"場所: {newDate.AreaName} / 累計: {newDate.KillCount} 回 / 最終: {newDate.LastKillTime}");
        }
    }
}
