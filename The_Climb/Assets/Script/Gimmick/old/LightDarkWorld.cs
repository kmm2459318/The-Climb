#if UNITY_EDITOR
using NUnit.Framework;
#endif
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.Rendering.Universal;
#endif
using UnityEngine;
using UnityEngine.UI;

public class LightDarkWorld : MonoBehaviour
{
    private GameObject player;
    private PlayerState playerState;
    private BuddyCarry buddyCarry;
    private Light worldLight;
    private MeshRenderer backGround;
    [SerializeField] private Image uiBackground;
    [SerializeField] private TextMeshProUGUI text;

    public enum brightness {Dark, Light};  //光と闇
    public brightness brightnessState = brightness.Dark;  //現在の世界の輝度

    //private float lightDuration = 15f;     //光の継続時間
    //private float lightTimer = 0f;         //光の世界の時間private
    private float transparency = 0.3f;     //白と黒の床壁の透明度
    private List<GameObject> whiteGroup = new List<GameObject>();
    private List<GameObject> blackGroup = new List<GameObject>();

    void Awake()
    {
        player = GameObject.Find("PlayerModel");
        if(player != null)
            playerState = player.GetComponent<PlayerState>();
            
        buddyCarry = player != null ? player.GetComponent<BuddyCarry>() : null;
        
        var lightObj = GameObject.Find("Directional Light");
        if(lightObj != null)
            worldLight = lightObj.GetComponent<Light>();
            
        var bgObj = GameObject.Find("StageBackGround");
        if(bgObj != null)
            backGround = bgObj.GetComponent<MeshRenderer>();

        //白い床
        AddRange(whiteGroup, GameObject.FindGameObjectsWithTag("LightWhite"));

        //黒い床
        AddRange(blackGroup, GameObject.FindGameObjectsWithTag("DarkBlack"));
        
        // 初期状態適用（ただしStartでLayerChange(false)が呼ばれていたので、それはStartに残すか、ここで呼ぶか。
        // 元のロジックではStartで呼んでいたので、依存関係があるかもしれないが、
        // 変数初期化自体はAwakeでやるべき）
    }

    void Start()
    {
        LayerChange(false);
    }
    
    // ... (AddRange method remains same) ...

    // ... (Update and LightDarkChange methods remain same) ...

    // 他のクラスから現在の状態を適用し直すためのメソッド
    public void ApplyCurrentState(PlayerState pState = null)
    {
        if(pState != null)
        {
            this.playerState = pState;
        }
        LayerChange(brightnessState == brightness.Light);
    }

    private void AddRange(List<GameObject> list, GameObject[] objs)
    {
        foreach (var o in objs)
        {
            list.Add(o);
        }
    }

    void Update()
    {
        //光と闇切り替え
        if (Input.GetMouseButtonDown(1))
        {
            if (brightnessState == brightness.Dark)  //闇→光
            {
                LightDarkChange(brightness.Light);
            }
            else  //光→闇
            {
                LightDarkChange(brightness.Dark);
            }
        }

        //光闇世界の違い
        //if (brightnessState == brightness.Dark)  //闇の世界
        //{

        //}
        //else  //光の世界
        //{
        //    lightTimer -= Time.deltaTime;
        //    if (lightTimer <= 0)
        //    {
        //        LightDarkChange(brightness.Dark);
        //    }
        //}
    }

    private void LightDarkChange(brightness s)
    {
        if (brightnessState == brightness.Dark && s == brightness.Light)  //闇→光
        {
            //if ((playerState.carryingBuddy || playerState.nearBell || buddyCarry.nearBuddy) && !buddyCarry.buddyController.beingKidnapped)  //Buddyおんぶしてるとき
            //{
                brightnessState = brightness.Light;
                //Debug.Log("■■■魔法「破壊超陽光」■■■");
                //lightTimer = lightDuration;
                text.color = Color.black;
                LayerChange(true);
            //}
        }
        else if (brightnessState == brightness.Light && s == brightness.Dark)  //光→闇
        {
            brightnessState = brightness.Dark;
            //Debug.Log("□□□鵺符「アンディファインドダークネス」□□□");
            text.color = Color.white;
            LayerChange(false);
        }
    }

    // 他のクラスから現在の状態を適用し直すためのメソッド


    private void LayerChange(bool isLight)
    {
        int player = LayerMask.NameToLayer("Player");
        int buddy = LayerMask.NameToLayer("Buddy");
        int bomb = LayerMask.NameToLayer("Bomb");
        int ground = LayerMask.NameToLayer("Ground");
        int whiteGround = LayerMask.NameToLayer("WhiteGround");
        int blackGround = LayerMask.NameToLayer("BlackGround");
        int whiteOther = LayerMask.NameToLayer("WhiteOther");
        int blackOther = LayerMask.NameToLayer("BlackOther");

        int[] target = { player, buddy, bomb };  //動く側のレイヤー
        (int layer, bool whatBrightness)[] obj = {
            (whiteGround, true),
            (blackGround, false),
            (whiteOther, true),
            (blackOther, false)
        };  //白黒のレイヤーたち

        //物理的な当たり判定制御
        foreach (int t in target)
        {
            foreach (var (lay, what) in obj)
            {
                Physics.IgnoreLayerCollision(t, lay, isLight == what);
            }
        }

        //判定用LayerMaskの設定
        if (!isLight)
        {
            playerState.groundLayerMask =
                (1 << ground) | (1 << whiteGround);

            //白系黒系の透明度変化（黒を半透明に）
            ObjectTransparency(whiteGroup, 1f);
            ObjectTransparency(blackGroup, transparency);
            worldLight.color = new Color(80f / 255f, 80f / 255f, 80f / 255f, 1f);
            backGround.material.color = Color.black;

            uiBackground.color = Color.black;
        }
        else
        {
            playerState.groundLayerMask =
                (1 << ground) | (1 << blackGround);

            //白系黒系の透明度変化（白を半透明に）
            ObjectTransparency(whiteGroup, 0.1f);
            ObjectTransparency(blackGroup, 1f);
            worldLight.color = Color.white;
            backGround.material.color = new Color(195f / 255f, 195f / 255f, 190f / 255f, 1f);
            uiBackground.color = new Color(195f / 255f, 195f / 255f, 190f / 255f, 1f);
        }
    }

    //白系黒系の透明度変化
    private void ObjectTransparency(List<GameObject> objs, float tp)
    {
        foreach (GameObject obj in objs)
        {
            if (obj == null) continue;

            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            if (mr == null) continue;

            Color c = mr.material.color;
            mr.material.color = new Color(c.r, c.g, c.b, tp);
        }
    }

    //途中で追加された白黒オブジェクトの仕分け
    public void RegisterObject(GameObject obj)
    {
        int whiteLayer = LayerMask.NameToLayer("WhiteOther");
        int blackLayer = LayerMask.NameToLayer("BlackOther");
        
        //リストに追加
        if (obj.layer == whiteLayer)
        {
            whiteGroup.Add(obj);
        }
        else if (obj.layer == blackLayer)
        {
            blackGroup.Add(obj);
        }

        //今の光闇状態に合わせて透明度反映
        if (brightnessState == brightness.Light)
        {
            if (obj.layer == whiteLayer)
                ObjectTransparency(new List<GameObject>() { obj }, transparency);
            else if (obj.layer == blackLayer)
                ObjectTransparency(new List<GameObject>() { obj }, 1f);
        }
        else
        {
            if (obj.layer == whiteLayer)
                ObjectTransparency(new List<GameObject>() { obj }, 1f);
            else if (obj.layer == blackLayer)
                ObjectTransparency(new List<GameObject>() { obj }, transparency);
        }
    }

    //途中で削除された白黒オブジェクトのリストからの除外
    public void UnregisterObject(GameObject obj)
    {
        whiteGroup.Remove(obj);
        blackGroup.Remove(obj);
    }
}
