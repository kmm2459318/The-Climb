using System;
using UnityEngine;

//  着地したら着地関数を呼ぶ
[RequireComponent(typeof(CharacterGroundChecker))]
public class LandGroundNotifier : MonoBehaviour
{
    ILandingHandler LandingHandler;    // 地面に当たった際のインターフェイス
    CharacterGroundChecker characterGroundChecker;
    void Awake()
    {
        LandingHandler = GetComponent<ILandingHandler>();
        characterGroundChecker = GetComponent<CharacterGroundChecker>();
    }
    //  地面に着地したかを検知
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(TagName.Ground))
        {
            if (LandingHandler != null &&
                characterGroundChecker.CheckIsGround())
            {
             LandingHandler.OnLandStage();
            }
        }
    }
}
