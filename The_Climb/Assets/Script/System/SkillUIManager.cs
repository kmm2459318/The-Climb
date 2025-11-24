
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルUIを管理するマネージャークラス
/// 現在アクティブなスキルの画像表示と、各スキルのクールタイムゲージを管理
/// [Recompiled]
/// </summary>
public class SkillUIManager : MonoBehaviour
{
    [Header("スキル状態画像")]
    [SerializeField] private Image skillIconImage; // 現在のスキルアイコンを表示する画像
    [SerializeField] private Sprite umedaSkillSprite; // Umedaスキル用の画像
    [SerializeField] private Sprite kitanoSkillSprite; // Kitanoスキル用の画像
    [SerializeField] private Sprite nisiyamaSkillSprite; // Nisiyamaスキル用の画像

    [Header("クールタイムゲージ(上から下に減少)")]
    [SerializeField] private Image umedaCooldownGauge; // Umedaスキルのクールタイムゲージ
    [SerializeField] private Image kitanoCooldownGauge; // Kitanoスキルのクールタイムゲージ
    [SerializeField] private Image nisiyamaCooldownGauge; // Nisiyamaスキルのクールタイムゲージ

    [Header("参照")]
    [SerializeField] private PlayerAbilityManager abilityManager; // スキル管理マネージャー
    [SerializeField] private SkillPlatformSpawner skillPlatformSpawner; // Umedaスキル
    [SerializeField] private GravityFlipManager gravityFlipManager; // Nisiyamaスキル
    [SerializeField] private TempDisableColliders tempDisableColliders; // Kitanoスキル

    private int previousSkillNumber = -1; // 前回のスキル番号(変更検知用)

    void Start()
    {
        // 初期化: すべてのゲージを満タンに設定
        if (umedaCooldownGauge != null)
        {
            umedaCooldownGauge.fillAmount = 0f; // 初期状態は0
            umedaCooldownGauge.type = Image.Type.Filled;
            umedaCooldownGauge.fillMethod = Image.FillMethod.Vertical;
            umedaCooldownGauge.fillOrigin = (int)Image.OriginVertical.Bottom; // 下を基準にする(上から減っていくように見せるため)
        }

        if (kitanoCooldownGauge != null)
        {
            kitanoCooldownGauge.fillAmount = 0f;
            kitanoCooldownGauge.type = Image.Type.Filled;
            kitanoCooldownGauge.fillMethod = Image.FillMethod.Vertical;
            kitanoCooldownGauge.fillOrigin = (int)Image.OriginVertical.Bottom;
        }

        if (nisiyamaCooldownGauge != null)
        {
            nisiyamaCooldownGauge.fillAmount = 0f;
            nisiyamaCooldownGauge.type = Image.Type.Filled;
            nisiyamaCooldownGauge.fillMethod = Image.FillMethod.Vertical;
            nisiyamaCooldownGauge.fillOrigin = (int)Image.OriginVertical.Bottom;
        }

        // 初期スキル画像を設定
        UpdateSkillIcon();
    }

    void Update()
    {
        // スキル切り替えを検知して画像を更新
        if (abilityManager != null)
        {
            int currentSkill = abilityManager.CurrentSkillNumber;
            if (currentSkill != previousSkillNumber)
            {
                UpdateSkillIcon();
                previousSkillNumber = currentSkill;
            }
        }

        // 各スキルのクールタイムゲージを更新
        UpdateCooldownGauges();
    }

    /// <summary>
    /// 現在アクティブなスキルに応じてアイコン画像を切り替える
    /// </summary>
    private void UpdateSkillIcon()
    {
        if (skillIconImage == null || abilityManager == null) return;

        // 全てのアビリティがOFFかチェック
        bool isUmedaActive = PlayerPrefs.GetInt("UmedaAbi") == 1;
        bool isKitanoActive = PlayerPrefs.GetInt("KitanoAbi") == 1;
        bool isNisiyamaActive = PlayerPrefs.GetInt("NisiyamaAbi") == 1;

        if (!isUmedaActive && !isKitanoActive && !isNisiyamaActive)
        {
            // 全てOFFなら非表示
            skillIconImage.gameObject.SetActive(false);
            SetGaugeActive(umedaCooldownGauge, false);
            SetGaugeActive(kitanoCooldownGauge, false);
            SetGaugeActive(nisiyamaCooldownGauge, false);
            return;
        }

        // いずれかがONなら表示
        skillIconImage.gameObject.SetActive(true);

        int currentSkill = abilityManager.CurrentSkillNumber;

        switch (currentSkill)
        {
            case 1: // Umedaスキル
                if (umedaSkillSprite != null) skillIconImage.sprite = umedaSkillSprite;
                SetGaugeActive(umedaCooldownGauge, true);
                SetGaugeActive(kitanoCooldownGauge, false);
                SetGaugeActive(nisiyamaCooldownGauge, false);
                break;
            case 2: // Kitanoスキル
                if (kitanoSkillSprite != null) skillIconImage.sprite = kitanoSkillSprite;
                SetGaugeActive(umedaCooldownGauge, false);
                SetGaugeActive(kitanoCooldownGauge, true);
                SetGaugeActive(nisiyamaCooldownGauge, false);
                break;
            case 0: // Nisiyamaスキル
                if (nisiyamaSkillSprite != null) skillIconImage.sprite = nisiyamaSkillSprite;
                SetGaugeActive(umedaCooldownGauge, false);
                SetGaugeActive(kitanoCooldownGauge, false);
                SetGaugeActive(nisiyamaCooldownGauge, true);
                break;
        }
    }

    private void SetGaugeActive(Image gauge, bool isActive)
    {
        if (gauge != null)
        {
            gauge.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// 各スキルのクールタイムゲージを更新
    /// </summary>
    private void UpdateCooldownGauges()
    {
        // Umedaスキルのクールタイムゲージ更新
        if (umedaCooldownGauge != null && skillPlatformSpawner != null)
        {
            float progress = skillPlatformSpawner.GetCooldownProgress();
            umedaCooldownGauge.fillAmount = progress;
        }

        // Kitanoスキルのクールタイムゲージ更新
        if (kitanoCooldownGauge != null && tempDisableColliders != null)
        {
            float progress = tempDisableColliders.GetCooldownProgress();
            kitanoCooldownGauge.fillAmount = progress;
        }

        // Nisiyamaスキルのクールタイムゲージ更新
        if (nisiyamaCooldownGauge != null && gravityFlipManager != null)
        {
            // GravityFlipManagerから直接クールタイム情報を取得
            float progress = gravityFlipManager.GetCooldownProgress();
            nisiyamaCooldownGauge.fillAmount = progress;
        }
    }
}
