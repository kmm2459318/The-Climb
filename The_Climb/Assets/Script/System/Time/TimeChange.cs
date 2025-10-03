using UnityEngine;

public class TimeChange : MonoBehaviour
{
    [Header("ステージセレクト")]
    [SerializeField] private GameObject[] MapPrefabs;
    private GameObject[] MapInstance;

    [Header("プレイヤーの参照")]
    [SerializeField] private Transform Player;

    public KeyBind KeyBind;

    [Header("フェード制御")]
    [SerializeField] private ScreenFader fader;

    [Header("クールダウン時間(秒)")]
    [SerializeField] private float switchCooldown = 2f;

    private int CurrentActiveIndex = 0;
    private SafeSpawner spawner;


    void Start()
    {
        KeyBind = GameObject.Find("KeyManager").GetComponent<KeyBind>();
        spawner = Player.GetComponent<SafeSpawner>();

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
        MapInstance[CurrentActiveIndex].SetActive(false);

        CurrentActiveIndex++;
        if (CurrentActiveIndex >= MapInstance.Length)
            CurrentActiveIndex = 0;

        MapInstance[CurrentActiveIndex].SetActive(true);

        // 安全な位置に修正
        if (spawner != null)
        {
            Player.position = spawner.FindSafePosition(Player.position);
        }
    }
}