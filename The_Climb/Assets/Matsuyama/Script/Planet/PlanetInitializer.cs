using TheClimb.Astral;
using TheClimb.Player;
using UnityEngine;

public class PlanetInitializer : MonoBehaviour    //  天体の初期化実行係
{
    PlanetController _planetController;         //  天体の包括的コントローラー

    IPlanetDataProvider _PlanetDataProvider;      //  天体のデータプロバイダー
    IPlayerDataProvider _PlayerDataProvider;      //  プレイヤーのデータプロバイダー
    

    private void Start()
    {
        _planetController = PlanetContext.Instance._PlanetController;
        _PlanetDataProvider = PlanetContext.Instance._PlanetDataProvider;
        _PlayerDataProvider = PlayerContext.Instance._PlayerDataProvider; ;

        Debug.Log(_planetController);
        Debug.Log(_PlanetDataProvider);
        Debug.Log(_PlayerDataProvider);
        _planetController.Initialize(_PlanetDataProvider, _PlayerDataProvider);
    }
}
