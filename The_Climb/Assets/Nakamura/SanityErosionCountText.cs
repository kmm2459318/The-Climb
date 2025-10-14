using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SanityErosionCountText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private PlayerState playerState;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        playerState = GameObject.Find("PlayerModel").GetComponent<PlayerState>();
    }

    void Update()
    {
        text.text = "SAN : " + playerState.sanityLevel + "ERO : " + playerState.erosionLevel;
    }
}
