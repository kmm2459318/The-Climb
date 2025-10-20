using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class TimeChange : MonoBehaviour
{
    [Header("ステージセレクト")]
    [SerializeField] private GameObject[] MapPrefabs;
    private GameObject[] MapInstance;

    [Header("プレイヤーの参照")]
    [SerializeField] private Transform Player;

    [Header("フェード制御")]
    [SerializeField] private ScreenFader fader;

    [Header("クールダウン時間(秒)")]
    [SerializeField] private float switchCooldown = 2f;

    public KeyBind KeyBind;                   //プレイヤーのキーを取得

    private int CurrentActiveIndex = 0;
    //private SafeSpawner spawner;


    void Start()
    {
        KeyBind = GameObject.Find("KeyManager").GetComponent<KeyBind>();
        //spawner = Player.GetComponent<SafeSpawner>();

        MapInstance = new GameObject[MapPrefabs.Length];
        for (int i = 0; i < MapPrefabs.Length; i++)
        {
            MapInstance[i] = Instantiate(MapPrefabs[i], Vector3.zero, Quaternion.identity);
            MapInstance[i].SetActive(false);
        }

        MapInstance[CurrentActiveIndex].SetActive(true);
    }


    void Update()
    {
        if (!fader.IsFading && Input.GetKeyDown(KeyBind.timeSwitch))
        {
            fader.FadeAndDo(SwitchToNextMap);
        }
    }

    private void SwitchToNextMap()
    {
        //シーン内にいるすべての敵を消す
        EnemyGeneration[] EnemyDelete = Object.FindObjectsByType<EnemyGeneration>(FindObjectsSortMode.InstanceID);
        foreach (EnemyGeneration Generator in EnemyDelete)
        {
            Generator.ClearAllEnemy();
        }

        MapInstance[CurrentActiveIndex].SetActive(false);

        CurrentActiveIndex++;
        if (CurrentActiveIndex >= MapInstance.Length)
            CurrentActiveIndex = 0;

        MapInstance[CurrentActiveIndex].SetActive(true);

        //新しいマップの敵生成
        var Aanlyzer = FindAnyObjectByType<EnemyKillAnalyzer>();
        var killRatios = Aanlyzer.GetKillRatio();
        EnemyGeneration[] EnemyGenerate = Object.FindObjectsByType<EnemyGeneration>(FindObjectsSortMode.InstanceID);
        foreach(EnemyGeneration Generator in EnemyGenerate)
        {
            //Generator.AdjustSpawnByKillRatio(killRatios);
        }
        //// 安全な位置に修正
        //if (spawner != null)
        //{
        //    Player.position = spawner.FindSafePosition(Player.position);
        //}
    }
}