using UnityEngine;

public class GimmickSwitch : MonoBehaviour
{
    private TimeGimmickBridge Bridge;        // 状態保存ブリッジ
    [SerializeField] private SwitchReceiver[] Receivers;
    [SerializeField] private KeyCode ActivateKey = KeyCode.E; // インタラクトキー

    [Header("必要なアイテム情報")]
    [SerializeField] private int RequiredCore = 0; // 必要なコアの数
    private string ItemName = "コア";              // 必要なアイテム名
    private ItemDataBase ItemDB;

    [Header("表示用オブジェクト")]
    [SerializeField] private GameObject SwitchOff;  // スイッチ未作動時に表示するオブジェクト
    [SerializeField] private GameObject SwitchOn;   // スイッチ作動後に表示するオブジェクト
    [SerializeField] private GameObject a;  

    private bool IsOn = false; // 押されたかどうか（内部状態）
    private bool CanInteract = false;

    private void Awake()
    {
        // Bridge 自動取得
        if (Bridge   == null) Bridge = GetComponent<TimeGimmickBridge>();

        // データベース自動取得
        ItemDB = FindObjectOfType<ItemDataBase>();

        if (ItemDB == null)
        {
            Debug.LogError("ItemDataBaseがシーン内に見つかりません");
        }

        SwitchOn.SetActive(false);
        a.SetActive(false);
    }

    private void Update()
    {
        if (IsOn) return; // 既に押されていたら何もしない

        if (CanInteract && Input.GetKeyDown(ActivateKey))
        {
            a.SetActive(false);
            TryActivateSwitch();
            // 見た目を切り替える
            SwitchOff.SetActive(false);
            SwitchOn.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("当たった");
            if (IsOn) return;   //既に押されていたら何もしない
            CanInteract = true;
            a.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("離れた");
            CanInteract = false;
            a.SetActive(false);
        }
    }

    // 起動条件を確認して、足りていれば起動
    private void TryActivateSwitch()
    {
        if (ItemDB == null)
        {
            Debug.LogError("アイテムデータベースが見つかりません");
            return;
        }

        int CurrentCore = ItemDB.GetItemCount(ItemName);
        if (CurrentCore >= RequiredCore)
        {
            ItemDB.ConsumeItem(ItemName, RequiredCore);
            ActivateSwitch();
        }
        else
        {
            Debug.Log("コアが足りない");
        }
    }

    // スイッチが押された時の処理
    private void ActivateSwitch()
    {
        IsOn = true;                    // 押された状態にする
        Bridge?.ReportState(true);      // 状態を保存
        
        //スイッチの見た目を切り替える
        if (SwitchOff != null && SwitchOn != null)
        {
            SwitchOff.SetActive(false);
            SwitchOn.SetActive(true);
        }

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
