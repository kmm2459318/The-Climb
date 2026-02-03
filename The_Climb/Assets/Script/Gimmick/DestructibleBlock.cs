using Hanzzz.MeshDemolisher;
using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    // ブロックが存在する「時間軸(過去・現代)」を表すenum
    public enum TimeEra
    {
        Past,    // 過去
        Present, // 現代
    }

    [SerializeField] private TimeGimmickBridge Bridge;  // 状態保存ブリッジ
    [SerializeField] private GameObject BlockObject;    // ブロック本体(ON/OFFを切り替える対象)
    [SerializeField] private MeshDemolisherExample demolisher; //足場を壊す演出

    [SerializeField] private GameObject dark; // 暗闇
    [SerializeField] TimeEra Era = TimeEra.Past;// このブロックが属する時代(過去/現代)

    private bool IsVisible = true;  //現在ブロックが表示されているか

    private void Awake()
    {
        // Bridge を取得
        if (Bridge == null) Bridge = GetComponent<TimeGimmickBridge>();

        // 状態が適用された時に呼ばれるイベントを登録
        if (Bridge != null)
        {
            Bridge.OnStateApplied.AddListener(ApplyState);
        }

        if (BlockObject == null)
        {
            BlockObject = this.gameObject;
        }
        Debug.Log("イベントが呼ばれました");
    }

    void OnEnable()
    {
        // 保存された状態が存在する時だけ適用
        if(Bridge != null && Bridge.HasSavedState)
        {
            //マップ切り替え後の再表示に最新状態を反映
            Bridge?.ApplySavedState();
        }
    }

    // ブロックの表示を切り替える(爆発時に呼ばれる)
    public void BreakBlock()
    {
        // 現代ではブロックは破壊できない
        if (Era == TimeEra.Present) return;

        IsVisible = false;　　　　　　　 // 表示状態を false(非表示)に変更
        ApplyState(IsVisible);           // 状態を反映
        Bridge?.ReportState(IsVisible);  // 状態を保存
        Bridge?.ApplySavedState();
    }

    // Bridgeから受け取った状態を適用する処理
    public void ApplyState(bool IsActive)
    {
        // 初回ロードではデフォルト状態を維持
        if (Bridge != null && !Bridge.HasSavedState) return;

        IsVisible = IsActive;

 
        // 壊れる時だけ演出
        if (!IsActive && demolisher != null)
        {
            demolisher.RequestDemolish();
            Debug.Log("破壊します");
        }

        if (!IsActive && dark != null)
        {
            dark.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        //イベント登録を解除
        if (Bridge != null)
        {
            Bridge.OnStateApplied.RemoveListener(ApplyState);
        }
    }
}