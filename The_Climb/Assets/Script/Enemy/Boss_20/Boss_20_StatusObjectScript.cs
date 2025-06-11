using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Boss_20_StatusObjectScript", menuName = "Scriptable Objects/Boss_20_StatusObjectScript")]
public class Boss_20_StatusObjectScript : ScriptableObject
{
    public string NAME;    //敵の名前
    public int HP;　　　   //敵のHP
    public int Speed;      //敵の速さ
    public int Blow_away;  //吹っ飛ばし力
    public int Lateral;    //横移動
    public int Rest;       //休憩のタイミング
    public int Vertical;   //縦移動
    public int Long_Range_Attack;   //遠距離攻撃のタイミング
}
