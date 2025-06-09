using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KickerStatus", menuName = "GameDate/Enemy/KickerStatus")]
//  キッカーのステータス
public class KickerStatus : ScriptableObject
{
    //  状態とステータスをもつクラス
    [System.Serializable]
    public class StateStatPair
    {
        public EnemyStates State;    //  敵の状態(通常時と狂暴化)
        public KickerStatBlock Stats;    //  ステータスを持つクラス
    }

    [Header("Kicker Status")]
    public List<StateStatPair> StateStats = new();    //  状態とステータスを持つクラスのリスト(データ入力用)

    Dictionary<EnemyStates, KickerStatBlock> StatMap;    //  状態とステータスの辞書(処理用)

    void OnEnable()
    {
        //  スーテータスマップの初期化
        BuildStatMap();
    }
    //  ステータスマップ初期化
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
    //  状態に応じたステータスの取得
    public KickerStatBlock GetStats(EnemyStates State)
    {
        return StatMap.TryGetValue(State, out KickerStatBlock stats) ? stats : null;
    }
}
//  以下コード保存所

//[Header("Normal Time")]
//[SerializeField] float MoveSpd_Normal;    //  移動速度(平常時)
//[SerializeField] float JumpForce_Normal;    //  ジャンプ力(平常時)
//[SerializeField] float JumpFrequency_Normal;  //  ジャンプする間隔(平常時)
//[Header("Violent Time")]
//[SerializeField] float MoveSpd;    //  移動速度(狂暴化時)
//[SerializeField] float JumpForce;    //  ジャンプ力(狂暴化時)
//[SerializeField] float JumpFrequency;  //  ジャンプする間隔(狂暴化時)

//public float MoveSpdProperty => MoveSpd_Normal;
//public float JumpForceProperty => JumpForce_Normal;
//public float JumpFrequencyProperty => JumpFrequency_Normal;