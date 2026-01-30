using System;
using UnityEngine;
using TheClimb.Item;
using TheClimb.Astral;
using TMPro;

namespace TheClimb.Core
{
    public class ObjectLabelController : MonoBehaviour    //  オブジェクトのラベルの制御をするクラス
    {
        [Header("ラベル設定")]
        [SerializeField] private Vector3 _offset;    //  ラベルオフセット

        AttractableListenerBase itemContoroller;    //  アイテムコントローラー
        TextMeshPro label;    //  オブジェクトテキスト
        IItemLabelDef itemDefinition;    //  アイテム定義
        Transform _labelTargetTF;    //  衝撃球のトランスフォーム
        Transform _labelRootTF;    //  ラベルのRootオブジェクトのトランスフォーム
        Transform _cameraTF;    //  カメラのトランスフォーム

        //  --  Unity LifeCycle

        void LateUpdate()
        {
            LookCamera();    //  カメラ方向を向かせる
            HomingObject();    //  テキストをオブジェクトに追従


            Action action = itemDefinition.ItemKind switch    //  アイテム種類で分別する、簡易ステートパターン
            {
                ItemKind.Time_Action => () => TimeItemUpdate(),
                _ => () => Debug.LogWarning($"ItemKind value is Unacceptable")
            };

            action();
        }

        //  --  Public API

        public void Initialize(IItemLabelDef defSO, ObjectLabelContext _context)    //  表示するためのトランスフォームと、SOの参照をもらってる
        {
            itemDefinition = defSO;

            _cameraTF = _context.MainCameraTF;
            _labelTargetTF = _context.ImpactBallTF;

            _labelRootTF = this.transform;
            _labelRootTF.localPosition = _offset;

            itemContoroller = _context.ItemController;
            label = _context.ObjectLabel;
        }

        //  --  Private API

        void LookCamera()    //  ラベルをカメラに向かせる
        {
            Vector3 dir = _labelRootTF.position - _cameraTF.position;


            if (dir.sqrMagnitude < 0.0001f)
            { return; }

            _labelRootTF.position = _labelTargetTF.position + _offset;
            _labelRootTF.rotation = Quaternion.LookRotation(dir);
        }

        void HomingObject()    //  テキストをオブジェクトに追従させる
        {
            _labelRootTF.position = _labelTargetTF.position;
        }

        void TimeItemUpdate()    //  時間制限系アイテムのUpdate
        {
            label.text = itemContoroller.RemainCount.ToString();
        }
    }
}