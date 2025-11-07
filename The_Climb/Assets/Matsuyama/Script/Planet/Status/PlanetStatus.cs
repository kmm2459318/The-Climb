using System.Collections.Generic;
using UnityEngine;

namespace TheClimb.Astral
{
    [CreateAssetMenu(fileName = "PlanetStatus", menuName = "Planet/Status")]

    public class PlanetStatus : ScriptableObject
    {
        //  敵の状態とステータスをもつクラス
        [System.Serializable]
        public class PlanetIDStatusPair    //  ステータスブロックが増えてきたらブロックをまとめたクラス作成
        {
            public PlanetIDs CustomerName;                 //  敵の状態(通常時と狂暴化)
            public GravitationStatusBlock gravitationStatus;    //  質量などのステータスクラス
            public OrbitalStatusBlock orbitalStatus;       //  軌道のステータスクラス
        }

        [Header("PlanetStatus")]
        public List<PlanetIDStatusPair> planetIDStatus = new();    //  状態とステータスを持つクラスのリスト(データ入力用)

        Dictionary<PlanetIDs, (GravitationStatusBlock gravitationStatus, OrbitalStatusBlock orbitalStatus) > StatusMap;    //  状態とステータスの辞書(処理用)

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
                if (!StatusMap.ContainsKey(pair.CustomerName))
                {
                    StatusMap.Add(pair.CustomerName, (pair.gravitationStatus, pair.orbitalStatus));
                }
            }
        }

        //  IDに応じた万有引力ステータスの取得
        public GravitationStatusBlock GetGraviatationStatus(PlanetIDs PlanetID)
        {
            return StatusMap.TryGetValue(PlanetID, out var data) ? data.gravitationStatus : null;
        }

        //  IDに応じた軌道スタータスの取得
        public OrbitalStatusBlock GetOrbitalStatus(PlanetIDs PlanetID)
        {
            return StatusMap.TryGetValue(PlanetID, out var data) ? data.orbitalStatus : null;
        }

        //  IDに応じたすべてのステータスを保持
        public (GravitationStatusBlock gravitationStatus, OrbitalStatusBlock orbitalStatus)? GetFullStatus(PlanetIDs id)
        {
            return StatusMap.TryGetValue(id, out var data) ? data : null;
        }

    }
}