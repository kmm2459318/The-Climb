using UnityEngine;
using System.Collections;
using System.Threading;

//  コルーチンランナー
public class CoroutineRunner : MonoBehaviour
{
    static CoroutineRunner coroutineRunner;    //  コルーチンランナー
    static readonly object lockObj = new object();    //  排他制御用変数

    //  コンストラクタ
    public static CoroutineRunner Instance
    {
        get
        {
            lock (lockObj)
            {
                if (coroutineRunner == null)
                {
                    GameObject coroutineRunnerObject = new GameObject("CoroutineRunner");    //  コルーチンランナーのゲームオブジェクト生成
                    DontDestroyOnLoad(coroutineRunnerObject);
                    coroutineRunner = coroutineRunnerObject.AddComponent<CoroutineRunner>();
                }
                return coroutineRunner;
            }
        }
    }
    // コルーチン開始
    public Coroutine Run(IEnumerator routine)
    {
        if (routine != null)
        {
            return StartCoroutine(routine);
        }
        Debug.LogError("CoroutineRunner.Run : nullのIEnumeratorが渡されました。");
        return null;
    }
    //  コルーチン停止
    public void Stop(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}