using UnityEngine;

public class PlayerAbilityManager : MonoBehaviour
{
    
   
    public GameObject umeAbi;
    public GameObject kitaAbi;
    public GameObject nakaAbi;
    public GameObject nishiAbi;
    public GameObject matsuAbi;
    public GameObject miyamotoyuoAbi;

    void Start()
    {
        GameObject[] abilityBools = {nakaAbi, matsuAbi, umeAbi, miyamotoyuoAbi, kitaAbi, nishiAbi};
        string[] abilityNames = { "Nakamura", "Matsuyama", "Umeda", "Miyamoto", "Kitano", "Nishiyama", "Yuoka"};

        for (int i = 0; i < abilityBools.Length; i++)
            if (PlayerPrefs.GetInt($"{abilityNames[i]}") == 1)
            {
                abilityBools[i].active = true;
            }
    }

    void Update()
    {
        
    }
}
