using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class LightDarkWorld : MonoBehaviour
{
    private GameObject player;
    private PlayerState playerState;
    private BuddyCarry buddyCarry;
    private Light worldLight;
    private MeshRenderer backGround;
    [SerializeField] private TextMeshProUGUI text;

    public enum brightness {Dark, Light};  //光と闇
    public brightness brightnessState = brightness.Dark;  //現在の世界の輝度

    private float lightDuration = 15f;     //光の継続時間
    private float lightTimer = 0f;         //光の世界の時間private
    private float transparency = 0.3f;     //白と黒の床壁の透明度
    private List<GameObject> whiteGroup = new List<GameObject>();
    private List<GameObject> blackGroup = new List<GameObject>();

    void Start()
    {
        player = GameObject.Find("PlayerModel");
        playerState = player.GetComponent<PlayerState>();
        buddyCarry = player.GetComponent<BuddyCarry>();
        worldLight = GameObject.Find("Directional Light").GetComponent<Light>();
        backGround = GameObject.Find("StageBackGround").GetComponent<MeshRenderer>();

        //白い床
        AddRange(whiteGroup, GameObject.FindGameObjectsWithTag("LightWhite"));

        //黒い床
        AddRange(blackGroup, GameObject.FindGameObjectsWithTag("DarkBlack"));

        ////StalkerHand
        //var stalkers = GameObject.FindGameObjectsWithTag("StalkerHand");

        //int whiteLayer = LayerMask.NameToLayer("WhiteOther");
        //int blackLayer = LayerMask.NameToLayer("BlackOther");

        //foreach (var s in stalkers)
        //{
        //    if (s.layer == whiteLayer)
        //    {
        //        whiteGroup.Add(s);
        //    }
        //    else if (s.layer == blackLayer)
        //    {
        //        blackGroup.Add(s);
        //    }
        //}
        LayerChange(false);
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
        if (Input.GetKeyDown(KeyCode.J))
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
        if (brightnessState == brightness.Dark)  //闇の世界
        {

        }
        else  //光の世界
        {
            lightTimer -= Time.deltaTime;
            if (lightTimer <= 0)
            {
                LightDarkChange(brightness.Dark);
            }
        }
    }

    private void LightDarkChange(brightness s)
    {
        if (brightnessState == brightness.Dark && s == brightness.Light)  //闇→光
        {
            if (playerState.carryingBuddy || playerState.nearBell || buddyCarry.nearBuddy)  //Buddyおんぶしてるとき
            {
                brightnessState = brightness.Light;
                //Debug.Log("■■■魔法「破壊超陽光」■■■");
                lightTimer = lightDuration;
                text.color = Color.black;
                LayerChange(true);
            }
        }
        else if (brightnessState == brightness.Light && s == brightness.Dark)  //光→闇
        {
            brightnessState = brightness.Dark;
            //Debug.Log("□□□鵺符「アンディファインドダークネス」□□□");
            text.color = Color.white;
            LayerChange(false);
        }
    }

    private void LayerChange(bool isLight)
    {
        int player = LayerMask.NameToLayer("Player");
        int buddy = LayerMask.NameToLayer("Buddy");
        int ground = LayerMask.NameToLayer("Ground");
        int whiteGround = LayerMask.NameToLayer("WhiteGround");
        int blackGround = LayerMask.NameToLayer("BlackGround");
        int whiteOther = LayerMask.NameToLayer("WhiteOther");
        int blackOther = LayerMask.NameToLayer("BlackOther");

        int[] target = { player, buddy };  //動く側のレイヤー
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
