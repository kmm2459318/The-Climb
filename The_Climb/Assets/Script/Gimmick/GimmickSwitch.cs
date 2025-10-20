using UnityEngine;

public class GimmickSwitch : MonoBehaviour
{
    [SerializeField] private TimeGimmickBridge Bridge;        // 状態保存ブリッジ
    [SerializeField] private KeyCode ActivateKey = KeyCode.E; // インタラクトキー
    [SerializeField] private float InteractionRange = 2f;     // インタラクト距離
    [SerializeField] private Transform PlayerTransform;       // プレイヤーTransform
    [SerializeField] private GameObject VisualRoot;           // 表示用子オブジェクト
    [SerializeField] private Collider SwitchCollider;         // スイッチの当たり判定

    private bool IsOn = false; // 押されたかどうか（内部状態）

    private void Awake()
    {
        if (Bridge     == null) Bridge = GetComponent<TimeGimmickBridge>(); // Bridge 自動取得
        if (VisualRoot == null) VisualRoot = this.gameObject; // VisualRoot が無ければ自分を代替

        if (PlayerTransform == null)
        {
            var playerObj = GameObject.FindWithTag("Player"); // Player タグで検索
            if (playerObj != null) PlayerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (IsOn) return; // 既に押されていたら何もしない
        if (PlayerTransform == null) return;

        float Dist = Vector3.Distance(PlayerTransform.position, transform.position); // 距離計算
        if (Dist <= InteractionRange && Input.GetKeyDown(ActivateKey))
        {
            ActivateSwitch();
        }
    }

    // スイッチが押された時の処理
    private void ActivateSwitch()
    {
        IsOn = true;                    // 押された状態にする
        Bridge?.ReportState(true);      // 状態を保存

        // 見た目を消す（オブジェクト自体は有効のまま）
        if (VisualRoot != null && VisualRoot != this.gameObject)
        {
            VisualRoot.SetActive(false);
        }
        else
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers) r.enabled = false;
        }

        SwitchCollider.enabled = false; 　//当たり判定を消す

        // 以後の入力処理を止める（スクリプトを無効化）
        this.enabled = false;
    }
}
