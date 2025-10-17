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
            return;
        }

        //ランダムな敵のデータと敵の選択
        EnemyStats EnemeyData = EnemyStatsList[Random.Range(0, EnemyStatsList.Count)];
        Transform Gate = EnemySpotList[Random.Range(0, EnemySpotList.Count)];

        //敵の生成
        GameObject obj = Instantiate(EnemeyData.EnemyPrefab, Gate.position, Quaternion.identity);
        obj.name = EnemeyData.EnemyName;

        //データがあるか確認EnmeyData
        Enemy EnemyBase = obj.GetComponent<Enemy>();
        if (EnemyBase != null)
        {
            EnemyBase.SetUp(EnemeyData, this);

            FieldEnemies.Add(obj);
        }
    }

        /// <summary>
        /// 敵削除時に呼ばれる
        /// </summary>
        public void RemoveEnemy(GameObject enemy)
        {
            if (FieldEnemies.Contains(enemy))
            {
                FieldEnemies.Remove(enemy);
                Debug.Log($"敵 {enemy.name} を削除しました。現在の敵数: {FieldEnemies.Count}");
            }
        }
    }

  

