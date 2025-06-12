using UnityEngine;

public class KeyBind : MonoBehaviour
{
    public KeyCode playerMoveLeft = KeyCode.A;
    public KeyCode playerMoveRight = KeyCode.D;
    public KeyCode playerJump = KeyCode.Space;

    public void Save()
    {
        PlayerPrefs.SetString("jump", playerJump.ToString());
        PlayerPrefs.SetString("moveLeft", playerMoveLeft.ToString());
        PlayerPrefs.SetString("moveRight", playerMoveRight.ToString());
    }

    public void Load()
    {
        playerJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jump", playerJump.ToString()));
        playerMoveLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveLeft", playerMoveLeft.ToString()));
        playerMoveRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveRight", playerMoveRight.ToString()));
    }
}
