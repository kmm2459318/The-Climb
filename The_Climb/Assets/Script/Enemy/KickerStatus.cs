using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KickerStatus", menuName = "GameDate/Enemy/KickerStatus")]
//  �L�b�J�[�̃X�e�[�^�X
public class KickerStatus : ScriptableObject
{
    //  ��ԂƃX�e�[�^�X�����N���X
    [System.Serializable]
    public class StateStatPair
    {
        public EnemyStates State;    //  �G�̏��(�ʏ펞�Ƌ��\��)
        public KickerStatBlock Stats;    //  �X�e�[�^�X�����N���X
    }

    [Header("Kicker Status")]
    public List<StateStatPair> StateStats = new();    //  ��ԂƃX�e�[�^�X�����N���X�̃��X�g(�f�[�^���͗p)

    Dictionary<EnemyStates, KickerStatBlock> StatMap;    //  ��ԂƃX�e�[�^�X�̎���(�����p)

    void OnEnable()
    {
        //  �X�[�e�[�^�X�}�b�v�̏�����
        BuildStatMap();
    }
    //  �X�e�[�^�X�}�b�v������
    void BuildStatMap()
    {
        StatMap = new();
        foreach (var pair in StateStats)
        {
            if (!StatMap.ContainsKey(pair.State))
            {
                StatMap.Add(pair.State, pair.Stats);
            }
        }
    }
    //  ��Ԃɉ������X�e�[�^�X�̎擾
    public KickerStatBlock GetStats(EnemyStates State)
    {
        return StatMap.TryGetValue(State, out KickerStatBlock stats) ? stats : null;
    }
}
//  �ȉ��R�[�h�ۑ���

//[Header("Normal Time")]
//[SerializeField] float MoveSpd_Normal;    //  �ړ����x(���펞)
//[SerializeField] float JumpForce_Normal;    //  �W�����v��(���펞)
//[SerializeField] float JumpFrequency_Normal;  //  �W�����v����Ԋu(���펞)
//[Header("Violent Time")]
//[SerializeField] float MoveSpd;    //  �ړ����x(���\����)
//[SerializeField] float JumpForce;    //  �W�����v��(���\����)
//[SerializeField] float JumpFrequency;  //  �W�����v����Ԋu(���\����)

//public float MoveSpdProperty => MoveSpd_Normal;
//public float JumpForceProperty => JumpForce_Normal;
//public float JumpFrequencyProperty => JumpFrequency_Normal;