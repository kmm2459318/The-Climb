using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMind : MonoBehaviour
{
    private PlayerState state;
    private LightDarkWorld lightDarkWorld;

    private int sanityMax = 100;             //正気度の最大値
    private float SANDecreaseCoolTime = 0f;  //正気度減少のクールタイム
    private float SANDecreaseDuration = 5f;  //正気度減少の間隔
    private bool inFog = false;              //霧の中

    void Start()
    {
        state = GetComponent<PlayerState>();
        if (GameObject.Find("LightDarkWorld") != null)
        {
            lightDarkWorld = GameObject.Find("LightDarkWorld").GetComponent<LightDarkWorld>();
        }
    }

    void Update()
    {
        //正気度１００超えたら１００にする
        if (state.sanityLevel > sanityMax)
        {
            state.sanityLevel = sanityMax;
        }
        //侵蝕度０下回ったら０にする
        if (state.erosionLevel < 0)
        {
            state.erosionLevel = 0;
        }

        //暗い闇の中で侵蝕度増加
        if (lightDarkWorld != null)
        {
            if (lightDarkWorld.brightnessState == LightDarkWorld.brightness.Dark)
            {
                ErosionIncrease();
            }
            else if (!inFog)  //光かつNot霧の中で侵蝕度リセット
            {
                ErosionReset();
            }
        }

        //侵蝕があったら正気度減少
        if (state.erosionLevel > 0)
        {
            SanityDecrease();
        }
    }

    //侵蝕度増加
    private void ErosionIncrease()
    {
        state.erosionLevel += Time.deltaTime;
    }

    //侵蝕度リセット
    private void ErosionReset()
    {
        state.erosionLevel = 0;
        SANDecreaseCoolTime = 0f;
    }

    //正気度減少
    private void SanityDecrease()
    {
        //クールタイム
        SANDecreaseCoolTime += Time.deltaTime;
        if (SANDecreaseCoolTime >= SANDecreaseDuration)
        {
            //減少
            state.sanityLevel--;
            SANDecreaseCoolTime = 0f;
        }
    }

    //エリア切り替え時の正気度最大値減少
    public void SanityMaxDecrease()
    {
        sanityMax -= 5;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SanityHealItem"))
        {
            state.sanityLevel += 5;
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
