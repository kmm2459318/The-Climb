using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Boss_20_StatusObjectScript", menuName = "Scriptable Objects/Boss_20_StatusObjectScript")]
public class Boss_20_StatusObjectScript : ScriptableObject
{
    [Header("Bossのステータス")]
    [Tooltip("ボスの名前")]
    public string NAME;               //敵の名前
    [Tooltip("体力")]
    public int    HP;　　　           //敵のHP
    [Tooltip("横のスピード")]
    public float  Speed;              //敵の速さ
    [Tooltip("吹っ飛ばし力")]
    public int    Blow_away;          //吹っ飛ばし力
    [Tooltip("左に移動")]
    public int    LEFT;               //左の移動
    [Tooltip("左に移動できる範囲")]
    public int    LEFT_Max;           //左の移動の動ける範囲
    [Tooltip("右に移動")]
    public int    RIGHT;              //右の移動
    [Tooltip("右に移動できる範囲")]
    public int    RIGHT_Max;          //右の移動の動ける範囲
    [Tooltip("休憩までの行動時間")]
    public int    Action_Time;        //行動時間
    [Tooltip("休憩時間")]
    public int    Rest_Time;          //休憩時間
    [Tooltip("縦の移動距離")]
    public float  Vertical;           //縦移動
    [Tooltip("攻撃のクールタイム")]
    public int    Attack;             //遠距離攻撃のタイミング
    [Tooltip("攻撃スピード")]
    public float  Attack_Speed;       //遠距離攻撃の速さ
}
