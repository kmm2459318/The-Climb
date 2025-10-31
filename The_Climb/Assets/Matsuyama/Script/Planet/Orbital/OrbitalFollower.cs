using System.Collections;
using UnityEngine;
using TheClimb.Core;
using TheClimb.Logging;

namespace TheClimb.Astral
{
    public class OrbitalFollower : PlanetCommadBase    //  軌道追従クラス
    {
        OrbitalContext _context;        //  コンテキスト
        Coroutine orbitalFollowLoop;    //  天体が軌道上を動く

        ICorutineRunner _CoroutineRunner;    //  コルーチンランナー

        bool IsRunning;    //  Followコルーチンが走っているかどうか

        public OrbitalFollower(OrbitalContext orbitalCtx)    //  コンストラクタ
        {
            _context = orbitalCtx;
            _CoroutineRunner = orbitalCtx._corutineRunner;
        }

        public void Initialize()    //  初期化
        {  /*  後記可能性のために定義  */  }

        public override void Execute()    //  軌道追従実行
        {
            if (IsRunning) { return; }

            orbitalFollowLoop = _CoroutineRunner.StartCoroutine(OrbitalFollowLoop());    //  マウス追従ループ開始
        }

        IEnumerator OrbitalFollowLoop()    //  マウス位置に応じて円軌道を追従させるコルーチンループ
        {
            LogUtility.Log(LogPrefix.orbitalFollower, "天体円軌道追従開始", LogLevel.Debug);
            while (true)
            {
                yield return MoveAlongCircleByAngle(
                    _context._planetTransform,
                    _context._playerTransform,
                    _context._orbitalStatusBlock.OrbitRadius,
                    _context._orbitalStatusBlock.Duration
                    );
            }
        }

        IEnumerator MoveAlongCircleByAngle(Transform obj, Transform centerTF, float radius, float duration)    //  天体移動
        {
            Plane plane = new Plane(Vector3.forward, centerTF.position);    //  Ray検知用Plane
            
            float elapsed = 0f;    //  経過時間
            float startAngle = Mathf.Atan2(obj.position.y - centerTF.position.y, obj.position.x - centerTF.position.x) * Mathf.Rad2Deg;    //  開始アングル

            while (elapsed < duration)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 mousePos = ray.GetPoint(distance);
                    float endAngle = Mathf.Atan2(mousePos.y - centerTF.position.y, mousePos.x - centerTF.position.x) * Mathf.Rad2Deg;
                    if (endAngle < 0) endAngle += 360f;

                    float t = elapsed / duration;
                    float angle = Mathf.LerpAngle(startAngle, endAngle, t) * Mathf.Deg2Rad;

                    obj.position = centerTF.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        public void Stop()    //  マウス追従停止
        {
            if (orbitalFollowLoop != null)
            {
                _CoroutineRunner.StopCoroutine(orbitalFollowLoop);
                orbitalFollowLoop = null;
            }
            IsRunning = false;
        }
    }
}