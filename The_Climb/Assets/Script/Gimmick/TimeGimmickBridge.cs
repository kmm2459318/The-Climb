using UnityEngine;
using UnityEngine.Events;

public class TimeGimmickBridge : MonoBehaviour
{
    [Header("ギミック識別子")]
    [SerializeField] private string GimmickId;  //ギミックのID
    [Header("復元時のデフォルト状態")]
    [SerializeField] private bool DefaultState = false;  //保存がなかった場合に使うデフォルトの状態

    //ギミック本体へ状態を通知するためのUnityEvent(インスペクターで受け取り側の関数をアサインできる)
    public UnityEventBool OnStateApplied = new UnityEventBool();  //状態が適用されたときに呼ばれる

    //UnityEventの bool版(インスペクターで boolを渡せるようにする)
    [System.Serializable]
    public class UnityEventBool : UnityEngine.Events.UnityEvent<bool> { }

    private void Start()
    {
        ApplySavedState();
    }

    //ギミック本体が状態を変化させたときに呼ぶ
    public void ReportState(bool IsActive)
    {
        if (string.IsNullOrEmpty(GimmickId)) return;
        TimeGimmickStateManager.SetState(GimmickId, IsActive);
    }

    //保存されている状態を取得してローカルに適用する
    public void ApplySavedState()
    {
        bool State;
        if (TimeGimmickStateManager.TryGetState(GimmickId, out State))
        {
            ApplyLocalState(State);
        }
        else
        {
            ApplyLocalState(DefaultState);
        }
    }

    //実際にギミック本体へ状態を渡す処理
    private void ApplyLocalState(bool IsActive)
    {
        //UnityEventに通知
        OnStateApplied.Invoke(IsActive);
    }
}