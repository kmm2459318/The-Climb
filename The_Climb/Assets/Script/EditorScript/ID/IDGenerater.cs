using UnityEngine;

[RequireComponent(typeof(ObjectRegistry))]
//  オブジェクトID自動生成
[ExecuteInEditMode]
public class IDGenerater : MonoBehaviour
{
    [SerializeField] private IDCategory Category = IDCategory.UNDEFINED;   // 種類例：Player, Enemy
    [SerializeField] private IDLabel Label = IDLabel.UNDNAMED;        // 意味例：Main, Boss
    [SerializeField] private string ID = "";            // 自動生成されるID
    public string CategoryProperty => IDEnumToString.ToString(Category);
    public string LabelProperty => IDEnumToString.ToString(Label);
    public string IDProperty => ID;
    private void OnValidate()
    {
            ID = $"{CategoryProperty}_{LabelProperty}_{System.Guid.NewGuid().ToString("N").Substring(0, 8)}";
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
//  以下コード保存所  //
//if (string.IsNullOrEmpty(ID))    IDがなかったら実行
//{
//    ID = $"{CategoryProperty}_{LabelProperty}_{System.Guid.NewGuid().ToString("N").Substring(0, 8)}";
//#if UNITY_EDITOR
//    UnityEditor.EditorUtility.SetDirty(this);
//#endif
//}