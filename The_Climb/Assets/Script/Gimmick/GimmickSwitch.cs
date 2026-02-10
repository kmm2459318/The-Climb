using UnityEngine;

public class GimmickSwitch : MonoBehaviour
{
    [SerializeField] private Switch SwitchSource;
    [SerializeField] private SwitchReceiver[] Receivers;

    private bool IsActivated = false; // 押されたかどうか（内部状態）



    private void Update()
    {
        if (IsActivated) return;

        if (SwitchSource != null && SwitchSource.IsPressed)
        {
            Activate();
        }
    }

    // スイッチが押された時の処理
    private void Activate()
    {
        IsActivated = true;
        
        foreach (var r in Receivers)
        {
            if (r != null)
            {
                r.Activate();
            }
        }

        // 以後の入力処理を止める（スクリプトを無効化）
        this.enabled = false;
    }
}
