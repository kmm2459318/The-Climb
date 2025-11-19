using UnityEngine;

public class PlayerAbilityManager : MonoBehaviour
{
    
   
    public GameObject umeAbi;
    public GameObject kitaAbi;
    public GameObject nakaAbi;
    public GameObject nishiAbi;
    public GameObject matsuAbi;
    public GameObject miyamotoyuoAbi;

    [Header("デバッグ用")]
    public bool AbilityOn;

    int NowAbilityNo = 0;

    void Awake()
    {
        GameObject[] abilityBools = {nakaAbi, matsuAbi, umeAbi, miyamotoyuoAbi, kitaAbi, nishiAbi};
        string[] abilityNames = { "Nakamura", "Matsuyama", "Umeda", "Miyamoto", "Kitano", "Nisiyama", "Yuoka"};

        // デバッグ用で全アビリティオンオフ切り替えれるようにしてる
        for(int i = 0; i < abilityBools.Length; i++)
        {
            if(AbilityOn == true)
            {
                PlayerPrefs.SetInt($"{abilityNames[i]}", 1);
            }
            else
            {
                PlayerPrefs.SetInt($"{abilityNames[i]}", 0);
            }
        }

        for (int i = 0; i < abilityBools.Length; i++)
            if (PlayerPrefs.GetInt($"{abilityNames[i]}") == 0)
            {
                abilityBools[i].active = false;
            }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            switch (NowAbilityNo)
            {
                case 0:
                    NowAbilityNo++;
                    if (PlayerPrefs.GetInt("Umeda") == 1)
                    {
                        PlayerPrefs.SetInt("UmedaAbi", 1);
                        PlayerPrefs.SetInt("KitanoAbi", 0);
                        PlayerPrefs.SetInt("NisiyamaAbi", 0);
                        Debug.Log("梅田君起動");
                    }
                    break;
                case 1:
                    NowAbilityNo++;
                    if (PlayerPrefs.GetInt("Kitano") == 1)
                    {
                        PlayerPrefs.SetInt("UmedaAbi", 0);
                        PlayerPrefs.SetInt("KitanoAbi", 1);
                        PlayerPrefs.SetInt("NisiyamaAbi", 0);
                        Debug.Log("北野君起動");
                    }
                    break;
                case 2:
                    NowAbilityNo = 0;
                    if (PlayerPrefs.GetInt("Nisiyama") == 1)
                    {
                        PlayerPrefs.SetInt("UmedaAbi", 0);
                        PlayerPrefs.SetInt("KitanoAbi", 0);
                        PlayerPrefs.SetInt("NisiyamaAbi", 1);
                        Debug.Log("西山君起動");
                    }
                    break;

            }
        }
    }
}
