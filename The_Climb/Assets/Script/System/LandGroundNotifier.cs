using System;
using UnityEngine;

//  ���n�����璅�n�֐����Ă�
[RequireComponent(typeof(CharacterGroundChecker))]
public class LandGroundNotifier : MonoBehaviour
{
    ILandingHandler LandingHandler;    // �n�ʂɓ��������ۂ̃C���^�[�t�F�C�X
    CharacterGroundChecker characterGroundChecker;
    void Awake()
    {
        LandingHandler = GetComponent<ILandingHandler>();
        characterGroundChecker = GetComponent<CharacterGroundChecker>();
    }
    //  �n�ʂɒ��n�����������m
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
