using UnityEngine;

public class LightDarkWorld : MonoBehaviour
{
    private PlayerState state;

    public enum brightness {Dark, Light};  //光と闇
    public brightness brightnessState = brightness.Dark;  //現在の世界の輝度

    private float lightDuration = 15f;     //光の継続時間
    private float lightTimer = 0f;         //光の世界の時間

    void Start()
    {
        state = GameObject.Find("PlayerModel").GetComponent<PlayerState>();
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
            if (state.carryingBuddy)  //Buddyおんぶしてるとき
            {
                brightnessState = brightness.Light;
                Debug.Log("■■■魔法「破壊超陽光」■■■");
                lightTimer = lightDuration;
            }
        }
        else if (brightnessState == brightness.Light && s == brightness.Dark)  //光→闇
        {
            brightnessState = brightness.Dark;
            Debug.Log("□□□鵺符「アンディファインドダークネス」□□□");
        }
    }
}
