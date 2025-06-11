using System.Collections.Generic;
using UnityEngine;

public enum GameLayers    //  �Q�[���̃��C���[�ꗗ
{
    GROUND,
}

public static class LayerNames    //  ���C���[���ꗗ
{
    public const string Ground = "Ground";
}
public static class GameLayer    //  ���C���[�f�[�^
{
    public static readonly Dictionary<GameLayers, string> LayerEnumToName = new()    //  ���C���[enum��string�̎����^
    {
        { GameLayers.GROUND, LayerNames.Ground },
    };
    public static readonly Dictionary<GameLayers, int> LayerEnumToIndex = new();    //  ���C���[enum�ƃ��C���[�C���f�b�N�X�̎����^
    public static readonly Dictionary<GameLayers, LayerMask> LayerEnumToMask = new();    //  ���C���[enum�ƃ��C���[�}�X�N�̎����^

    //  �����ݒ�
    static GameLayer()
    {
        foreach(KeyValuePair<GameLayers, string> kvp in LayerEnumToName)
        {
            int index = LayerMask.NameToLayer(kvp.Value);
            if(index == -1)
            {
#if UNITY_EDITOR
                Debug.LogError($"[GameLayer] Laeyr ' {kvp.Value}' does not exist in project settings.");
#endif
            }
            LayerEnumToIndex[kvp.Key] = index;
            LayerEnumToMask[kvp.Key] = LayerMask.GetMask(kvp.Value);
        }
    }

    public static LayerMask ToMask(GameLayers layer) => LayerEnumToMask[layer];
    public static int ToIndex(GameLayers layer) => LayerEnumToIndex[layer];
    public static string ToName(GameLayers layer) => LayerEnumToName[layer];
    //  ���C���[�C���f�b�N�X���烌�C���[�擾
    public static GameLayers? FromIndex(int index)
    {
        foreach (KeyValuePair<GameLayers, int> kvp in LayerEnumToIndex)
        {
            if (kvp.Value == index)
            {
                return kvp.Key;
            }
        }
        return null;
    }
    //  ���C���[�����烌�C���[�擾
    public static GameLayers? FromName(string layerName)
    {
        foreach(KeyValuePair<GameLayers, string> kvp in LayerEnumToName)
        {
            if(kvp.Value == layerName)
            {
                return kvp.Key;
            }
        }
        return null;
    }

}
