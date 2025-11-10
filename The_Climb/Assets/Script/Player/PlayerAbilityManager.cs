using UnityEngine;

public class PlayerAbilityManager : MonoBehaviour
{
    public bool nakaAbi = false;
    public bool matsuAbi = false;
    public bool miyaAbi = false;
    public bool umeAbi = false;
    public bool kitaAbi = false;
    public bool nishiAbi = false;
    public bool yuoAbi = false;

    void Start()
    {
        bool[] abilityBools = {nakaAbi, matsuAbi, umeAbi, miyaAbi, kitaAbi, nishiAbi, yuoAbi};
        string[] abilityNames = { "Nakamura", "Matsuyama", "Umeda", "Miyamoto", "Kitano", "Nishiyama", "Yuoka"};

        for (int i = 0; i < abilityBools.Length; i++)
            if (PlayerPrefs.GetInt($"{abilityNames[i]}") == 1)
            {
                abilityBools[i] = true;
            }
    }

    void Update()
    {
        
    }
}
