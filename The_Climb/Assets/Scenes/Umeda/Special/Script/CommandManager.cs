using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance;

    [Header("UI")]
    public GameObject commandPanel;
    public TMP_InputField inputField;
    public TMP_Text outputText; // ログ表示用

    [Header("ログ設定")]
    public float logDuration = 5f; // 何秒でフェードアウト
    public int maxLogLines = 21;

    bool isOpen = false;
    private Queue<string> logQueue = new Queue<string>(); // ログ履歴

    void Awake()
    {
        // シングルトン
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        commandPanel.SetActive(false);
        if (outputText != null) outputText.text = "";
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // シーン切替時にコマンドパネルを閉じる
        if (commandPanel != null)
            commandPanel.SetActive(false);

        isOpen = false;
        Time.timeScale = 1f;

        // InputField フォーカス解除
        if (inputField != null)
            inputField.DeactivateInputField();

        // EventSystem 選択状態をクリア
        EventSystem es = EventSystem.current;
        if (es != null)
            es.SetSelectedGameObject(null);
    }

    void Update()
    {
        // 閉じているときだけ / で開く
        if (!isOpen && Input.GetKeyDown(KeyCode.Slash))
        {
            OpenCommand();
            return;
        }

        if (!isOpen) return;

        // Backspaceで / を消させない
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (inputField.caretPosition <= 1)
            {
                inputField.caretPosition = 1;
                return;
            }
        }

        // Enterでコマンド実行
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteCommand(inputField.text);
            CloseCommand();
        }

        // Escで閉じる
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCommand();
        }
    }

    void OpenCommand()
    {
        isOpen = true;
        commandPanel.SetActive(true);

        inputField.text = "/";

        // カーソル位置を初期化、選択状態解除
        inputField.caretPosition = 1;
        inputField.selectionAnchorPosition = 1;
        inputField.selectionFocusPosition = 1;

        inputField.ActivateInputField();

        Time.timeScale = 0f;
    }

    void CloseCommand()
    {
        isOpen = false;
        commandPanel.SetActive(false);
        Time.timeScale = 1f;

        // フォーカス解除
        if (inputField != null)
            inputField.DeactivateInputField();

        // EventSystem 選択状態をクリア
        EventSystem es = EventSystem.current;
        if (es != null)
            es.SetSelectedGameObject(null);
    }

    void ExecuteCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            LogOutput("Unknown command: " + command);
            return;
        }

        if (!command.StartsWith("/"))
        {
            LogOutput("Unknown command: " + command);
            return;
        }

        string displayCommand = command;                   // 表示用
        string realCommand = command.Substring(1).ToLower(); // 判定用 (/ を除去)

        switch (realCommand)
        {
            case "help":
                ShowHelp();
                break;
            case "reset":
                RunReset();
                break;
            case "secret":
                RunSecret();
                break;
            default:
                LogOutput("Unknown command: " + displayCommand);
                break;
        }
    }

    void ShowHelp()
    {
        string helpText =
            "/help   - Show command list\n" +
            "/reset  - Reset the game";
        // /secret は非表示
        LogOutput(helpText);
    }

    void RunReset()
    {
        CloseCommand();
        Time.timeScale = 1f;

        EventSystem es = EventSystem.current;
        if (es != null)
            es.SetSelectedGameObject(null);

        // タイトルに戻る
        StartGame startGame = FindObjectOfType<StartGame>();
        if (startGame != null)
            startGame.EndGame();
        else
            LogOutput("StartGame スクリプトが見つかりません。");
    }

    void RunSecret()
    {
        CloseCommand();
        Time.timeScale = 1f;
        SceneManager.LoadScene("SecretScene");
    }

    void LogOutput(string text)
    {
        // 行単位で分割して追加
        string[] lines = text.Split('\n');
        foreach (var line in lines)
        {
            logQueue.Enqueue(line);

            // 最大行数チェック
            if (logQueue.Count > maxLogLines)
                logQueue.Dequeue();
        }

        UpdateOutputText();

        // フェードアウト開始
        StartCoroutine(FadeLogAfterDelay(logDuration));
    }

    void UpdateOutputText()
    {
        if (outputText != null)
            outputText.text = string.Join("\n", logQueue.ToArray());
    }

    IEnumerator FadeLogAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (logQueue.Count > 0)
        {
            logQueue.Dequeue();
            UpdateOutputText();
        }
    }
}
