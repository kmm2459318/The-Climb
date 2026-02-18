using UnityEngine;
using System.Collections.Generic;

namespace TheClimb.Core
{
    public class EffectSystem : MonoBehaviour, IEffectSystem    //  エフェクト処理システム
    {
        [Header("エフェクトカタログ")]
        [SerializeField] private EffectCatalog catalog;

        private readonly Dictionary<EffectKey, GameObject> playingEffects = new();

        private void Awake()
        {
            ServiceLocator.Register<IEffectSystem>(this);    // ServiceLocator に自分を登録
        }

        private void OnDestroy()
        {
            //ServiceLocator.Unregister<IEffectSystem>();    // シーン破棄時に解除（任意だが安全）
        }

        //  --  Public API

        public void Play(EffectKey key, Vector3 position)    //  エフェクト再生
        {
            // 定義解決
            var definition = catalog.Get(key);
            if (definition == null)
            {
                Debug.LogWarning($"EffectDefinition not found : {key}");
                return;
            }

            Stop(key);    //  エフェクトを一回再生終了する

            var instance = Instantiate(definition.prefab, position, definition.prefab.transform.rotation);

            playingEffects[key] = instance;
        }

        public void Stop(EffectKey key)    //  エフェクト停止
        {
            if (!playingEffects.TryGetValue(key, out var instance))
                return;

            Destroy(instance);
            playingEffects.Remove(key);
        }

        public void ActiveEffect(EffectKey key, GameObject effectRoot)    //  渡されたEffectをアクティブにする
        {
            if(!effectRoot.activeSelf)
            {
                effectRoot.SetActive(true);
            }
            playingEffects[key] = effectRoot;
        }
    }
}