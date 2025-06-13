using UnityEngine;

public class KeyBind : MonoBehaviour
{
    public KeyCode playerMoveLeft = KeyCode.A;
    public KeyCode playerMoveRight = KeyCode.D;
    public KeyCode playerJump = KeyCode.Space;
    public KeyCode highJump = KeyCode.W;

    public void Save()
    {
        PlayerPrefs.SetString("moveLeft", playerMoveLeft.ToString());
        PlayerPrefs.SetString("moveRight", playerMoveRight.ToString());
        PlayerPrefs.SetString("playerJump", playerJump.ToString());
        PlayerPrefs.SetString("highJump", highJump.ToString());
    }

    public void Load()
    {
        playerMoveLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveLeft", playerMoveLeft.ToString()));
        playerMoveRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveRight", playerMoveRight.ToString()));
        playerJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("playerJump", playerJump.ToString()));
        highJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("highJump", highJump.ToString()));
    }
}
