using System;
using System.Collections;
using UnityEngine;
using Zenject;

//  ���t�E���ԊǗ��X�N���v�g
public class TimeManager : MonoBehaviour, ITimeProvider
{
    public event Action<bool> OnChangedNight;    //  ���\���C�x���g
    public Coroutine defaultTimeCount;    //  ���ԉ����֐�
    public Coroutine timeAcceleration;    //  ���ԉ����֐�

    [Inject] ITimeConfig TimeConfig;    //  ���Ԑݒ�

    public float CurrentTime;    //  ���݂̎���
    float TimeProgressValue;    //  �P�b������̎��Ԑi�s�l
    int CurrentDay;    //  ���݂̓��t
    [SerializeField] bool IsNight;    //  �邩�ǂ����̃t���O
    bool IsStopCount;    //  �^�C�}�[���~�߂邩�~�߂Ȃ���
    bool IsPlayerAttacked;    //  �v���C���[���U�����ꂽ���i���t���O�j

    //    �v���C���[���U�����ꂽ���v���p�e�B
    public bool IsPlayerAttackedProperty
    {
        get => IsPlayerAttacked;
        set => IsPlayerAttacked = value;
    }

    //  ���ݎ��Ԏ擾�v���p�e�B
    public float CurrentTimeProperty
    {
        get => CurrentTime;
        set => CurrentTime = value;
    }
    //  ���ݓ����擾�v���p�e�B
    public int CurrentDayProperty => CurrentDay;
    //  ��擾�v���p�e�B
    public bool IsNightProperty
    {
        get => IsNight;
        set
        {
            if (IsNight != value)
            {
                IsNight = value;
            }
            OnChangedNight.Invoke(IsNight);
        }
    }
    void Awake()
    {
        //  ���l������
        InitializeValue();
    }
    void Start()
    {
        CoroutineUtility.SafeStartCoroutine(this, ref defaultTimeCount, DefaultTimeCount());
    }
    //  �������֐�
    void InitializeValue()
    {
        CurrentTime = TimeConfig.InitializeTimeProperty;
        TimeProgressValue = TimeConfig.ProgressTimeProperty;
        CurrentDay = TimeConfig.InitializeDateProperty;
        IsStopCount = false;
        IsPlayerAttacked = false;
    }
    //  ���ԉ������b�p�[�֐�
    public void StartTimeAcceleration(float TargetValue, float Duration)
    {
        CoroutineUtility.SafeStopCoroutine(this, ref defaultTimeCount);
        CoroutineUtility.SafeStartCoroutine(this, ref timeAcceleration, TimeAcceleration(TargetValue, Duration));
    }
    //  �f�t�H���g���ԃJ�E���g�֐�
    IEnumerator DefaultTimeCount()
    {
        while (!IsStopCount)
        {
            CurrentTime += TimeProgressValue * Time.deltaTime;
            yield return null;
        }
    }
    //  ���ԉ����֐�
    public IEnumerator TimeAcceleration(float TargetTime, float Duration)
    {
        //float TimeDiference = Mathf.Abs(TargetTime - CurrentTime);    //  ���ݎ��ԂƖڕW���Ԃ̍�
        float WrappedTargrtTime = TargetTime > CurrentTime ? TargetTime: TargetTime + TimeConfig.OneDayTimeProperty;
        float TimeUntilTarget = WrappedTargrtTime - CurrentTime;
        float SecProgress = TimeUntilTarget / Duration;    //  1�b������̐i�s�l
        float Epsilon = 2f;

        //int Direction = (TargetTime >= CurrentTime) ? 1 : -1;    //  ���Z�����Z�̕���

        //while ((Direction == 1 && CurrentTime < TargetTime) || (Direction == -1 && CurrentTime > TargetTime))
        //{
        //    CurrentTime += Direction * SecProgress * Time.deltaTime;

        //    if((Direction == 1 && CurrentTime > TargetTime) || (Direction == -1 && CurrentTime > TargetTime))
        //    {
        //        CurrentTime = TargetTime;
        //    }

        //    yield return null;
        //}
        
        while (Mathf.Abs(WrappedTargrtTime - CurrentTime) > Epsilon)
        {
            CurrentTime += SecProgress * Time.deltaTime;
            yield return null;
        }
        Debug.Log("NightTrout");
        CurrentTime = TargetTime;
        timeAcceleration = null;
        defaultTimeCount = StartCoroutine(DefaultTimeCount());
        IsPlayerAttacked = false;
    }
}
