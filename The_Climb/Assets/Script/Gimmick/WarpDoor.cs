using UnityEngine;

public class WarpDoor : MonoBehaviour
{
    [SerializeField] private bool canGoBack = false;
    [Header("canGoBackがtrueであればgoToDoorを" +
        "falseならgoToWhereを指定しろ")]
    [SerializeField] private GameObject goToDoor;
    [SerializeField] private GameObject goToWhere;

    void Start()
    {
        if ((canGoBack && goToDoor != null) || (!canGoBack && goToWhere != null))
        {
            Debug.LogError(canGoBack ? "goToDoor" : "goToWhere" + "を指定してください。");
        }
    }

    void Update()
    {
        
    }
}
