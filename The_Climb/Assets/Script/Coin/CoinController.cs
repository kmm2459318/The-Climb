using UnityEngine;
using TMPro;

public class CoinController : MonoBehaviour
{
    [Header("回転設定")]
    public float rotationSpeed = 90f;

    [Header("コイン取得設定")]
    public AudioClip pickupSound;
    public float respawnTime = -1f;

    [Header("UI設定（任意）")]
    public TextMeshProUGUI coinText;

    [Header("距離判定")]
    public float pickupRadius = 1.5f;

    private static int totalCoins = 0;

    private AudioSource audioSource;
    private Renderer[] myRenderers;
    private Transform player;
    private bool isCollected = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        myRenderers = GetComponentsInChildren<Renderer>();

        // プレイヤーをタグで検索
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Playerタグの付いたオブジェクトが見つかりません");

        UpdateUIText();
    }

    void Update()
    {
        // 回転
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        if (!isCollected && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= pickupRadius)
            {
                Debug.Log("半径0.5m以内にプレイヤーが入りました");
                CollectCoin();
            }
        }
    }

    void CollectCoin()
    {
        isCollected = true;

        if (pickupSound != null)
            audioSource.PlayOneShot(pickupSound);

        totalCoins++;
        Debug.Log("コイン獲得！合計: " + totalCoins);
        UpdateUIText();

        foreach (var r in myRenderers)
            r.enabled = false;

        if (respawnTime > 0)
        {
            Invoke(nameof(Respawn), respawnTime);
        }
        else
        {
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
    }

    void Respawn()
    {
        foreach (var r in myRenderers)
            r.enabled = true;

        isCollected = false;
    }

    void UpdateUIText()
    {
        if (coinText != null)
            coinText.text = "x " + totalCoins;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

}
