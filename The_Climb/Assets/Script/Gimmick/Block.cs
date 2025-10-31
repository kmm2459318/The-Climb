using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private TimeGimmickBridge Bridge; //同じGameObjectにアタッチして紐づける
    [SerializeField] private GameObject VisualRoot;    //表示用の子オブジェクト
    [SerializeField] private Collider[] Colliders;
    [SerializeField] private KeyCode ToggleKey = KeyCode.E; //切り替えキー

    private bool IsVisible = true;  //現在ブロックが表示されているか

    private void Awake()
    {
        if (Bridge == null) Bridge = GetComponent<TimeGimmickBridge>();

        if (Bridge != null)
        {
            Bridge.OnStateApplied.AddListener(ApplyState);
        }

        if (VisualRoot == null)
        {
            VisualRoot = this.gameObject;
        }

        if (Colliders == null || Colliders.Length == 0)
        {
            var c = GetComponents<Collider>();
            Colliders = c != null ? c : new Collider[0];
        }
    }

    void OnEnable()
    {
        //マップ切り替え後の再表示に最新状態を反映
        Bridge?.ApplySavedState();
    }

    private void Update()
    {
        //指定キーで表示・非表示を切り替える
        if (Input.GetKeyDown(ToggleKey))
        {
            ToggleVisibility();
        }
    }

    private void ToggleVisibility()
    {
        IsVisible = !IsVisible;
        ApplyState(IsVisible);
        Bridge?.ReportState(IsVisible);  //状態を保存
    }

    public void ApplyState(bool IsActive)
    {
        IsVisible = IsActive;

        if (VisualRoot != null && VisualRoot != this.gameObject)
        {
            VisualRoot.SetActive(IsVisible);
        }
        else
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
            {
                r.enabled = IsVisible;
            }
        }

        if (Colliders != null)
        {
            foreach (var col in Colliders)
            {
                if (col != null) col.enabled = IsVisible;
            }
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