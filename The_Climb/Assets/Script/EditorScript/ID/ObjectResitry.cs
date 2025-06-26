using System.Collections.Generic;
using UnityEngine;

//  オブジェクトとIDを辞書登録
public class ObjectRegistry : MonoBehaviour
{
    private static Dictionary<string, GameObject> registry = new();    //  オブジェクトのID辞書

    //  オブジェクト登録関数
    public static void Register(string id, GameObject go)
    {
        if (!registry.ContainsKey(id))
            registry.Add(id, go);
    }
    //  IDのゲームオブジェクトを出力
    public static GameObject Get(string id)
    {
        return registry.TryGetValue(id, out var go) ? go : null;
    }
    void Awake()
    {
        IDGenerater idObject = GetComponent<IDGenerater>();    //  IDObjectのインスタンス
        if (idObject != null)
        {
            //  オブジェクト登録
            Register(idObject.IDProperty, gameObject);
        }
    }
}