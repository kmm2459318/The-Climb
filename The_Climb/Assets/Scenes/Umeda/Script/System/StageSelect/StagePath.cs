using UnityEngine;

public class StagePath : MonoBehaviour
{
    [Header("接続ステージ番号")]
    public int fromStage;
    public int toStage;

    [Header("道の見た目（任意）")]
    public GameObject pathObject;

    private void Awake()
    {
        if (pathObject == null)
            pathObject = this.gameObject;
    }

    public void ShowPath()
    {
        if (pathObject != null)
            pathObject.SetActive(true);
    }

    public void HidePath()
    {
        if (pathObject != null)
            pathObject.SetActive(false);
    }
}
