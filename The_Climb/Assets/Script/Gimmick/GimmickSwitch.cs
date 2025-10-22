using UnityEngine;

public class GimmickSwitch : MonoBehaviour
{
    [SerializeField] private TimeGimmickBridge Bridge;        // 状態保存ブリッジ
    [SerializeField] private KeyCode ActivateKey = KeyCode.E; // インタラクトキー

    [Header("表示用オブジェクト")]
    [SerializeField] private GameObject SwitchOff;  //スイッチ未作動時に表示するオブジェクト
    [SerializeField] private GameObject SwitchOn;   //スイッチ作動後に表示するオブジェクト

    private bool IsOn = false; // 押されたかどうか（内部状態）
    private bool CanInteract = false;

    private void Awake()
    {
        // Bridge 自動取得
        if (Bridge == null) Bridge = GetComponent<TimeGimmickBridge>(); 

        SwitchOn.SetActive(false);
    }

    private void Update()
    {
        if (IsOn) return; // 既に押されていたら何もしない

        if (CanInteract && Input.GetKeyDown(ActivateKey))
        {
            ActivateSwitch();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("当たった");
            if (IsOn) return;   //既に押されていたら何もしない
            CanInteract = true;
            //ActivateSwitch();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("離れた");
            CanInteract = false;
        }
    }

    // スイッチが押された時の処理
    private void ActivateSwitch()
    {
        IsOn = true;                    // 押された状態にする
        Bridge?.ReportState(true);      // 状態を保存

        // 見た目を切り替える
        if (SwitchOff != null && SwitchOn != null)
        {
            SwitchOff.SetActive(false);
            SwitchOn.SetActive(true);
        }

        // 以後の入力処理を止める（スクリプトを無効化）
        this.enabled = false;
    }
}
