using UnityEngine;

public class KeyFragment : MonoBehaviour
{
    [Header("この欠片が属する鍵グループID")]
    public string keyID = "KeyA";  // 例："KeyA", "KeyB" など

    private void OnTriggerEnter(Collider other)
    {
        var collector = other.GetComponent<KeyCollector>();
        if (collector != null)
        {
            collector.CollectFragment(keyID);
            Destroy(gameObject);
        }
    }
}
