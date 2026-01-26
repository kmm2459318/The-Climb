using UnityEngine;

public class GimmickSwitch : MonoBehaviour
{
    private TimeGimmickBridge Bridge;        // 状態保存ブリッジ
    [SerializeField] private SwitchReceiver[] Receivers;

    private bool IsOn = false; // 押されたかどうか（内部状態）

    public Switch Switch;

    private void Awake()
    {
        // Bridge 自動取得
        if (Bridge == null) Bridge = GetComponent<TimeGimmickBridge>();
    }

    private void Update()
    {
        if (IsOn) return;

        if (Switch != null && Switch.IsPressed)
        {
            IsOn = true;
            ActivateSwitch();
        }
    }

    // スイッチが押された時の処理
    private void ActivateSwitch()
    {
        Bridge?.ReportState(true);      // 状態を保存
        
        foreach (var r in Receivers)
        {
            if (r != null)
            {
                r.ApplyCurrentState();
            }
        }

        // 以後の入力処理を止める（スクリプトを無効化）
        this.enabled = false;
    }
}
