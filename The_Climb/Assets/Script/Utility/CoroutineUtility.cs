using System;
using System.Collections;
using UnityEngine;

//  コルーチンユーティリティ
public static class CoroutineUtility
{
    //  コルーチンを開始
    public static void SafeStartCoroutine(MonoBehaviour mono, ref Coroutine coroutineRef, IEnumerator Routine)
    {
        if(mono == null || Routine == null)
        {
            return;
        }

        if(coroutineRef != null)
        {
            mono.StopCoroutine(coroutineRef);
        }

        coroutineRef = mono.StartCoroutine(Routine);
    }

    //  コルーチンを停止
    public static void SafeStopCoroutine(MonoBehaviour mono, ref Coroutine coroutineRef)
    {
        if (mono == null)
        {
            return;
        }

        if(coroutineRef != null)
        {
            mono.StopCoroutine(coroutineRef);
            coroutineRef = null;
        }
    }
    //  一定時間待機してから指定のアクションを開始
    public static Coroutine Delay(MonoBehaviour mono, float delaySeconds, System.Action action)
    {
        if(mono == null || delaySeconds < 0f)
        {
            return null;
        }
        return mono.StartCoroutine(DelayRoutine(delaySeconds, action));
    }
    //  指定時間待機関数
    private static IEnumerator DelayRoutine(float delaySecondes, System.Action action)
    {

        yield return new WaitForSeconds(delaySecondes);
        try
        {
            action?.Invoke();
        }
        catch(Exception ex)
        {
            Debug.LogError($"[CoroutineUtility] Exception in delayed action: {ex}");
        }
        
    }
}
