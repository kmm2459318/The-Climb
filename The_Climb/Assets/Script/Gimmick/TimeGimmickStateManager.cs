using UnityEngine;
using System.Collections.Generic;

public static class TimeGimmickStateManager
{
    private static Dictionary<string, bool> GimmickStates = new Dictionary<string, bool>();

    //ギミック状態を保存する
    public static void SetState(string GimmickId, bool IsActive)
    {
        if (GimmickId == null) return;
        GimmickStates[GimmickId] = IsActive;
        Debug.Log(GimmickId);
        Debug.Log(IsActive);
    }

    //保存された状態を取得する
    public static bool TryGetState(string GimmickId, out bool IsActive)
    {
        if (GimmickId == null)
        {
            IsActive = false;
            return false;
        }
        return GimmickStates.TryGetValue(GimmickId, out IsActive);
    }

    //public static void ClearAll()
    //{
    //    GimmickStates.Clear();
    //}
}