using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private TimeGimmickBridge Bridge;                            // 保存状態ブリッジ
    [SerializeField] private Transform DoorTransform;                             // 動くドアメッシュの Transform
    [SerializeField] private Vector3 OpenLocalPosition = new Vector3(0f, 3f, 0f); // 開いたときのローカル位置
    [SerializeField] private Collider DoorCollider;                               // ドアの当たり判定（通行可にするため無効化する）

    private bool IsOpen = false; // ドアが既に開いているか

    private void Awake()
    {
        if (Bridge        == null) Bridge = GetComponent<TimeGimmickBridge>(); // Bridge 自動取得
        if (DoorTransform == null) DoorTransform = transform;                  // 子がなければ自分を代替
        if (DoorCollider  == null) DoorCollider = GetComponent<Collider>();    // Collider 自動取得

        // Bridge のイベントに登録しておく（Awake で登録すると ApplySavedState に間に合う）
        if (Bridge != null) Bridge.OnStateApplied.AddListener(ApplyState);
    }

    void OnEnable()
    {
        // 保存値を適用（Bridge.Start() の呼び出しと重複しても安全）
        Bridge?.ApplySavedState();
    }

    // Bridge からの通知
    public void ApplyState(bool IsActive)
    {
        if (!IsActive) return;     // ボタンが押されてなければ何もしない
        if (IsOpen) return;        // 既に開いていれば何もしない

        IsOpen = true;

        // 扉を開く
        DoorTransform.localPosition += OpenLocalPosition;

        if (DoorCollider != null)
        {
            DoorCollider.enabled = false; // 当たり判定無効化で通れるように
        }
    }
}
