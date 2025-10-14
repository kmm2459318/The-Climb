using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMind : MonoBehaviour
{
    private PlayerState playerState;
    private LightDarkWorld lightDarkWorld;

    private int sanityMax = 100;             //正気度の最大値
    private float SANDecreaseCoolTime = 0f;  //正気度減少のクールタイム
    private float SANDecreaseDuration = 5f;  //正気度減少の間隔
    private bool inFog = false;              //霧の中

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        lightDarkWorld = GameObject.Find("LightDarkWorld").GetComponent<LightDarkWorld>();
    }

    void Update()
    {
        //正気度１００超えたら１００にする
        if (playerState.sanityLevel > sanityMax)
        {
            playerState.sanityLevel = sanityMax;
        }
        //侵蝕度０下回ったら０にする
        if (playerState.erosionLevel < 0)
        {
            playerState.erosionLevel = 0;
        }

        //暗い闇の中で侵蝕度増加
        if (lightDarkWorld.brightnessState == LightDarkWorld.brightness.Dark)
        {
            ErosionIncrease();
        }
        else if (!inFog)  //光かつNot霧の中で侵蝕度リセット
        {
            ErosionReset();
        }

        //侵蝕があったら正気度減少
        if (playerState.erosionLevel > 0)
        {
            sanityDecrease();
        }
    }

    //侵蝕度増加
    private void ErosionIncrease()
    {
        playerState.erosionLevel += Time.deltaTime;
    }

    //侵蝕度リセット
    private void ErosionReset()
    {
        playerState.erosionLevel = 0;
        SANDecreaseCoolTime = 0f;
    }

    //正気度減少
    private void sanityDecrease()
    {
        //クールタイム
        SANDecreaseCoolTime += Time.deltaTime;
        if (SANDecreaseCoolTime >= SANDecreaseDuration)
        {
            //減少
            playerState.sanityLevel--;
            SANDecreaseCoolTime = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SanityHealItem"))
        {
            playerState.sanityLevel += 5;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Fog"))  //霧の中
        {
            inFog = true;
            ErosionIncrease();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Fog"))  //霧から出た
        {
            inFog = false;
        }
    }
}
