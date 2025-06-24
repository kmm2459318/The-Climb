using UnityEditor;
using UnityEngine;

public class EdgeDarkEditorWindowCreater : EditorWindow
{
    [SerializeField] EdgeDarkCanvasSetting edgeDarkCanvasSetting;    //  設定値のスクリプタブルオブジェクト
    [SerializeField] EdgeDarkImgSetting edgeDarkImgSetting;    //  設定値のスクリプタブルオブジェクト
    [MenuItem("Tools/Canvas Scaler Tool")]
    //  エディタウィンドウを表示
    public static void ShowWindow()
    {
        GetWindow<EdgeDarkEditorWindowCreater>("Canvas Scaler Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Canvas Scaler Utility", EditorStyles.boldLabel);

        if (GUILayout.Button("Set EdgeDarkCanvas Scaling in Scene and Asset."))
        {
            try
            {
                EdgeDarkCanvasSetter.ApplyToAllEdgeDarkCanvas(edgeDarkCanvasSetting);    //  四隅からくる闇のキャンバスの設定をシーンとアセット内に適用
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ EdgeDarkCanvas設定適用時に例外発生: {ex.Message}\n{ex.StackTrace}");
            }
        }
        if (GUILayout.Button("Set EdgeDarkImage Setting in Scene and Asset."))
        {
            try
            {
                EdgeDarkImgSetter.ApplyToAllEdgeDarkImg(edgeDarkImgSetting);    //  四隅からくる闇のイメージを設定をシーンとアセット内に適用
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ EdgeDarkImage設定適用時に例外発生: {ex.Message}\n{ex.StackTrace}");
            }
        }
        GUILayout.Label("設定情報", EditorStyles.helpBox);
        EditorGUILayout.LabelField("参照解像度", $"{AppDefault.DefaultResolution.Width} × {AppDefault.DefaultResolution.Height}");
        EditorGUILayout.LabelField("マッチ", $"{edgeDarkCanvasSetting.MatchWidthOrHeight}");
    }
}
