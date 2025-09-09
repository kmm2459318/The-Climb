using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class KeyBind : MonoBehaviour
{
    //ーーーーーーーー仮途中ーーーーーーーーーー
    [SerializeField] private InputActionReference jumpAction;

    public void StartRebind()
    {
        // 古いバインディングをクリア
        jumpAction.action.RemoveAllBindingOverrides();

        // 次に押されたキー/ボタンをJumpに割り当て
        jumpAction.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse") // 例: マウスは除外
            .OnComplete(operation =>
            {
                Debug.Log("Rebind complete: " + jumpAction.action.bindings[0].effectivePath);
                operation.Dispose();

                // 保存
                Save();
            })
            .Start();
    }

    public void Save()
    {
        string json = jumpAction.action.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(jumpAction.action.name, json);
    }

    public void Load()
    {
        string json = PlayerPrefs.GetString(jumpAction.action.name, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            jumpAction.action.LoadBindingOverridesFromJson(json);
        }
    }

    //public KeyCode playerLMove = KeyCode.A;
    //public KeyCode playerRMove = KeyCode.D;
    //public KeyCode playerJump = KeyCode.Space;
    //public KeyCode highJump = KeyCode.W;
    //public KeyCode meteorDrop = KeyCode.S;

    //public void Save()
    //{
    //    PlayerPrefs.SetString("moveLeft", playerLMove.ToString());
    //    PlayerPrefs.SetString("moveRight", playerRMove.ToString());
    //    PlayerPrefs.SetString("playerJump", playerJump.ToString());
    //    PlayerPrefs.SetString("highJump", highJump.ToString());
    //    PlayerPrefs.SetString("meteorDrop", meteorDrop.ToString());
    //}

    //public void Load()
    //{
    //    playerLMove = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveLeft", playerLMove.ToString()));
    //    playerRMove = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveRight", playerRMove.ToString()));
    //    playerJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("playerJump", playerJump.ToString()));
    //    highJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("highJump", highJump.ToString()));
    //    meteorDrop = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("meteorDrop", meteorDrop.ToString()));
    //}
}
