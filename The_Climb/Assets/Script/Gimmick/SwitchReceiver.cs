using UnityEngine;

public class SwitchReceiver : MonoBehaviour
{
    [SerializeField] private TimeGimmickBridge Bridge;  // 保存状態ブリッジ

    [SerializeField] private GameObject InactiveObject; //スイッチ未作動時に表示するオブジェクト
    [SerializeField] private GameObject ActiveObject;   //スイッチ作動後に表示するオブジェクト

    void Awake()
    {
        // Bridge 自動取得
        if (Bridge == null) Bridge = GetComponent<TimeGimmickBridge>();

        // Bridge のイベントに登録しておく
        if (Bridge != null) Bridge.OnStateApplied.AddListener(ApplyState);

        // スイッチ作動後用のオブジェクトを非表示にしておく
        ActiveObject.SetActive(false);
    }

    // 保存値を適用
    void OnEnable()
    {
        Bridge?.ApplySavedState();
    }

    // Bridge からの通知
    void ApplyState(bool IsActive)
    {
        if(!IsActive) return;

        //オブジェクトの表示を切り替える
        InactiveObject.SetActive(false);
        ActiveObject.SetActive(true);
    }
}
