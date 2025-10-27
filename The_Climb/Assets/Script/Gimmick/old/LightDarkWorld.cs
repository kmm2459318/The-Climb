using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class LightDarkWorld : MonoBehaviour
{
    private GameObject player;
    private PlayerState playerState;
    private BuddyCarry buddyCarry;

    public enum brightness {Dark, Light};  //光と闇
    public brightness brightnessState = brightness.Dark;  //現在の世界の輝度

    private float lightDuration = 15f;     //光の継続時間
    private float lightTimer = 0f;         //光の世界の時間private
    private float transparency = 0.3f;     //白と黒の床壁の透明度
    private GameObject[] lightWhiteObj;    //白系のオブジェクト
    private GameObject[] darkBlackObj;     //黒系のオブジェクト

    void Start()
    {
        player = GameObject.Find("PlayerModel");
        playerState = player.GetComponent<PlayerState>();
        buddyCarry = player.GetComponent<BuddyCarry>();
        lightWhiteObj = GameObject.FindGameObjectsWithTag("LightWhite");
        darkBlackObj = GameObject.FindGameObjectsWithTag("DarkBlack");
        LayerChange(false);
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
                Debug.Log("■■■魔法「破壊超陽光」■■■");
                lightTimer = lightDuration;
                LayerChange(true);
            }
        }
        else if (brightnessState == brightness.Light && s == brightness.Dark)  //光→闇
        {
            brightnessState = brightness.Dark;
            Debug.Log("□□□鵺符「アンディファインドダークネス」□□□");
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

            ObjectTransparency(lightWhiteObj, 1f);
            ObjectTransparency(darkBlackObj, transparency);
        }
        else
        {
            playerState.groundLayerMask =
                (1 << ground) | (1 << blackGround);

            ObjectTransparency(lightWhiteObj, transparency);
            ObjectTransparency(darkBlackObj, 1f);
        }
    }

    private void ObjectTransparency(GameObject[] @object, float tp)
    {
        foreach (GameObject obj in @object)
        {
            MeshRenderer mr;
            mr = obj.GetComponent<MeshRenderer>();

            Color currentColor = mr.material.color;
            mr.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, tp);
        }
    }
}
