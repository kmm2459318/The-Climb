using UnityEngine;

namespace TheClimb.Core
{
    public class ObjectLabel : MonoBehaviour    //  オブジェクトのラベル設定用クラス
    {
        [Header("ラベル設定")]
        [SerializeField] private Vector3 _offset;    //  ラベルオフセット

        Transform LabelRootTF;    //  ラベルのRootオブジェクトのトランスフォーム
        Transform _cameraTF;    //  カメラのトランスフォーム
        Transform _imapctBallTF;    //  衝撃球のトランスフォーム

        //  --  Public API

        public void Initialize(ObjectLabelContext _context)    //  Boostrapから呼ばれる初期化
        {
            _cameraTF = _context.MainCameraTF;
            _imapctBallTF = _context.ImpactBallTF;

            this.transform.localPosition = _offset;
        }

        //  --  PrivateAPI
        
        void LateUpdate()
        {
            LookCamera();    //  カメラ方向を向かせる
            HomingObject();    //  テキストをオブジェクトに追従
        }

        void LookCamera()    //  ラベルをカメラに向かせる
        {
            Vector3 dir = this.transform.position - _cameraTF.position;


            if (dir.sqrMagnitude < 0.0001f)
            { return; }

            this.transform.position = _imapctBallTF.position + _offset;
            this.transform.rotation = Quaternion.LookRotation(dir);
        }
        void HomingObject()    //  テキストをオブジェクトに追従させる
        {
            Debug.Log(254);
            this.transform.position = _imapctBallTF.position;
        }
    }
}