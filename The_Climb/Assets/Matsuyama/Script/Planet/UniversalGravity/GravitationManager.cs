//    ※※※※　　後々使用する　　※※※※

//using System.Collections.Generic;
//using UnityEngine;

//namespace TheClimb.UniversalGravity
//{
//    public class GravitationManager : MonoBehaviour    //  
//    {
//        public static GravitationManager Instance { get; private set; }    //  シングルトンインスタンス提供

//        public List<GravitationEffecter> gravityEffecters = new List<GravitationEffecter>();

//        private void Awake()
//        {
//            if (Instance != null)
//            {
//                Destroy(gameObject);
//                return;
//            }
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        void Start()
//        {

//        }
//        void Update()
//        {

//        }
//    }
//}