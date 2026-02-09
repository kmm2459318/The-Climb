using UnityEngine;

public class SwitchReceiver : MonoBehaviour
{

    [SerializeField] private GameObject InactiveObject; //スイッチ未作動時に表示するオブジェクト
    [SerializeField] private GameObject ActiveObject;   //スイッチ作動後に表示するオブジェクト

    void Awake()
    {
        if (ActiveObject != null)
        {
            // スイッチ作動後用のオブジェクトを非表示にしておく
            ActiveObject.SetActive(false);
        }
    }

    public void Activate()
    {
        if (InactiveObject != null)
        {
            InactiveObject.SetActive(true);
        }

        if (ActiveObject != null)
        {
            ActiveObject.SetActive(true);
        }
    }
}
