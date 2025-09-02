using System.Collections;
using UnityEngine;

//  コルーチンランナーUtility
public interface ICoroutineRunner
{
    //  
    Coroutine Run(IEnumerator routine);
    void Stop(Coroutine coroutine);
}
