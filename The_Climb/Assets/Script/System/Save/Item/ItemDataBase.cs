using System.IO;
using System.Linq;
using System.Collections.Generic;
using SQLite4Unity3d;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    private SQLiteConnection Connection;

    private string SaveDbPath;


    private void Awake()
    {
        //保存する場所の指定(ビルド前用)
        SaveDbPath = Path.Combine(Application.dataPath, "ItemDataBase.db");
        //ユーザーのセーブデータに保存
        SaveDbPath = Path.Combine(Application.persistentDataPath, "ItemDataBase.db");

        //ItemDataBaseの編集
        Connection = new SQLiteConnection(SaveDbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        //データベースの接続
        Connection.CreateTable<ItemCollectData>();
    }


    /// <summary>
    /// アイテム取得時の記録処理
    /// </summary>
    public void AddOrUpdateItem(ItemData itemData)
    {
        // 既にあるか確認
        var existing = Connection.Table<ItemCollectData>().FirstOrDefault(x => x.ItemName == itemData.Name);
        string now = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        if (existing != null)
        {
            // カウント増加・最終取得時間更新
            existing.Count += 1;
            existing.LastCollectTime = now;
            Connection.Update(existing);
        }
        else
        {
            // 保存したことがないアイテムの場合
            var newItem = new ItemCollectData(itemData, 1, now);
            Connection.Insert(newItem);
        }

        Debug.Log($"アイテムデータ保存: {itemData.Name}");
    }

    /// <summary>
    /// 全アイテムの記録を取得
    /// </summary>
    public List<ItemCollectData> GetAllItems()
    {
        return Connection.Table<ItemCollectData>().ToList();
    }

    /// <summary>
    /// 特定アイテムの取得数を取得
    /// </summary>
    public int GetItemCount(string itemName)
    {
        var data = Connection.Table<ItemCollectData>().FirstOrDefault(x => x.ItemName == itemName);
        return data != null ? data.Count : 0;
    }

}
