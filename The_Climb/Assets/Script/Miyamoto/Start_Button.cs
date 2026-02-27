using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Button : MonoBehaviour
{

    [SerializeField] private CustomButton button;

    void Start()
    {
        button.onClickCallBack = () =>
        {
            SceneManager.LoadScene("Title");
        };
    }

}
