using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoudSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    void Start()
    {
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);
        SetBGMVolume(bgmSlider.value);
        SetSEVolume(seSlider.value);
    }

    public void SetBGMVolume(float value)
    {
        if (value <= 0.0001f)
            audioMixer.SetFloat("BGM_Volume", -80f);
        else
            audioMixer.SetFloat("BGM_Volume", Mathf.Log10(value) * 20);
    }

    public void SetSEVolume(float value)
    {
        if (value <= 0.0001f)
            audioMixer.SetFloat("SE_Volume", -80f);
        else
            audioMixer.SetFloat("SE_Volume", Mathf.Log10(value) * 20);
    }
}
