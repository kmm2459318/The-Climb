using System.Collections.Generic;
using UnityEngine;

namespace TheClimb.UniversalGravity
{
    public class GravitationObjectResistry    //  万有引力の影響を受けるオブジェクトのレジストリー
    {
        private static readonly List<GravitationTargetEntry> _entries = new();    //  thisとthis.GameObject.RBのクラスリスト
        public static IReadOnlyList<GravitationTargetEntry> Entries => _entries;

        public static void RegisterTarget(GravitationTargetRegister target, Rigidbody targetRB)    //  リストに渡されたtargetの情報を登録
        {
            if (_entries.Exists(e => e.target == target))
            {
                return;
            }
            _entries.Add(new GravitationTargetEntry(target, targetRB));
        }

        public static void UnregisterTarget(GravitationTargetRegister target, Rigidbody rigidbody)    //  渡されたtargetの情報をリストから登録解除
        {
            _entries.RemoveAll(e => e.target == target && e.rigidbody == rigidbody);
        }
    }
}