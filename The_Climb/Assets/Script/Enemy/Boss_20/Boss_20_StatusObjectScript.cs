using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Boss_20_StatusObjectScript", menuName = "Scriptable Objects/Boss_20_StatusObjectScript")]
public class Boss_20_StatusObjectScript : ScriptableObject
{
    public string NAME;    //�G�̖��O
    public int HP;�@�@�@   //�G��HP
    public int Speed;      //�G�̑���
    public int Blow_away;  //������΂���
    public int Lateral;    //���ړ�
    public int Vertical;   //�c�ړ�
}
