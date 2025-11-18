using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private void Awake()
    {
        PlayerPrefs.SetInt("Matsuyama", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
