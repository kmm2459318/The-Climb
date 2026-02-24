using UnityEngine;

namespace TheClimb.Core
{
    public class InputSystemBootstrap : MonoBehaviour    //  インプットシステムの初期化用クラス
    {
        InputSystem_Actions inputAction_System;

        InputHandleBase inputReceiver;    //  入力を受け付けるクラス

        void Awake()
        {
            //    inputAction_System = new InputSystem_Actions();
            //    inputReceiver = new PlayerInputHandle(inputAction_System); 
        }
    }
}