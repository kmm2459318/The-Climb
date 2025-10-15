using System.Collections.Generic;
using UnityEngine;

namespace TheClimb.Astral
{
    [CreateAssetMenu(fileName = "PlanetStatus", menuName = "Planet/Status")]

    public class PlanetStatus : ScriptableObject
    {
        //  敵の状態とステータスをもつクラス
        [System.Serializable]
        public class PlanetIDStatusPair
        {
            public PlanetIDs customerName;    //  敵の状態(通常時と狂暴化)
            public PlanetStatusBlock Stat;    //  ステータスを持つクラス
        }

        [Header("PlanetStatus")]
        public List<PlanetIDStatusPair> planetIDStatus = new();    //  状態とステータスを持つクラスのリスト(データ入力用)

        Dictionary<PlanetIDs, PlanetStatusBlock> StatusMap;    //  状態とステータスの辞書(処理用)

        void OnEnable()
        {
            //  スーテータスマップの初期化
            BuildStatMap();
        }

        //  ステータスマップ初期化
        void BuildStatMap()
        {
            StatusMap = new();
            foreach (var pair in planetIDStatus)
            {
                if (!StatusMap.ContainsKey(pair.customerName))
                {
                    StatusMap.Add(pair.customerName, pair.Stat);
                }
            }
        }

        //  状態に応じたステータスの取得
        public PlanetStatusBlock GetStats(PlanetIDs PlanetID)
        {
            return StatusMap.TryGetValue(PlanetID, out PlanetStatusBlock stats) ? stats : null;
        }
    }
}