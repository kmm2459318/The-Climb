using UnityEngine;
using System.Collections.Generic;


public class EnemyGeneration : MonoBehaviour
{
    [Header("敵のデータリスト")]
    [SerializeField] private List<EnemyStats> EnemyStatsList = new List<EnemyStats>();

    [Header("敵の出現場所の設定")]
    [SerializeField] private List<Transform> EnemySpotList = new List<Transform>();

    [Header("フィールド上の敵のリスト")]
    private List<GameObject> FieldEnemies = new List<GameObject>(); 
    [Header("フィールド上の敵")]
    private List<GameObject> fieldEnemies = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateEnemy();
    }


    ///<summy>
    ///敵の生成
    ///</summy>>
    void CreateEnemy()
    {
        if (EnemyStatsList.Count == 0 || EnemySpotList.Count == 0)
        {
            Debug.Log("敵のデータとスポットが設定されていません");
        }

        //ランダムな敵のデータと敵の選択
        EnemyStats EnmeyData = EnemyStatsList[Random.Range(0, EnemyStatsList.Count)];
        Transform Gate = EnemySpotList[Random.Range(0, EnemySpotList.Count)];

        //敵の生成
        GameObject obj = Instantiate(EnmeyData.EnemyPrefab, Gate.position, Quaternion.identity);
        obj.name = EnmeyData.EnemyName;

        //データがあるか確認
        //Enemy _enemyBase = obj.GetComponent<EnemyStats>();
        //if (Enemy != null)
        //{
        //    Enemy.Setup(EnemyDataBase, this);

        //    fieldEnemies.Add(obj);
        //}
    }

        /// <summary>
        /// 敵削除時に呼ばれる
        /// </summary>
        public void RemoveEnemy(GameObject enemy)
        {
            if (fieldEnemies.Contains(enemy))
            {
                fieldEnemies.Remove(enemy);
                Debug.Log($"敵 {enemy.name} を削除しました。現在の敵数: {fieldEnemies.Count}");
            }
        }
    }

  

