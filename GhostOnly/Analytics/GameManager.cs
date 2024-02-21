using Analytics;
using Util;
using Manager;
using System;
using Unity.Services.Analytics;

public class GameManager
{
    public enum EGameSystemValue
    {
        HpRegen,
        HpRegenTime,
        HpDecrease,
        HpDecreaseTime,
        LegendMasteryPercentage,
        EpicMasteryPercentage,
        RareMasteryPercentage,
    }

    //=== Enhanced Stats ==//
    public float recoverySpeed = 0f;
    public float attckPowerCoefficient = 0f;
    public float skullMaxHpCoefficient = 0f;
    public float skullMoveSpeedCoefficient = 0f;
    public float ghostMoveSpeedCoefficient = 0f;
    public float gravestoneRespawnTimeCoefficient = 0f;
    public float realTime = 0f;
    public float soulYield = 0f;


    //=== Game States ===//
    public int collectedSoul => Managers.Soul.EarnedSoul;
    public int currentDay = 0;
    public int currentAlter = 25;
    public int currentSkullCount = 2;
    public int maxSkullCount = 5;
    public int skullPriceCoefficient = 1;
    public int equipmentPriceCoefficient = 1;
    public int heroDeathCount = 0;
    public bool isGameClear = false;

    //=== GameSetting Values ===// 
    public float TimePerHeal { get; private set; } = Constants.Time.SkullBaseHealTime;
    public float TimePerDamage { get; private set; } = Constants.Time.SkullDamageTime;
    public float TimePerAuraTick { get; private set; } = Constants.Time.TimePerAuraTick;
    public float BaseHealAmount { get; private set; } = Constants.GameSystem.HealAmount;
    public float BaseDecreaseAmount { get; private set; } = Constants.GameSystem.DecreaseHealthAmount;
    public float RarePercent { get; private set; } = Constants.Probability.MasteryRare;
    public float EpicPercent { get; private set; } = Constants.Probability.MasteryEpic;
    public float LegendPercent { get; private set; } = Constants.Probability.MasteryLegend;

    public event Action OnChangeSkullCount;
    public event Action<AppSetting> OnAppSettingChanged;

    public PlayerController Player;

    public GameManager()
    {
        PreferencesManager.SetOnAppSettingChangeListener(CallAppSettingChanged);
    }

    private void Init()
    {
        recoverySpeed = 0f;
        attckPowerCoefficient = 0f;
        skullMaxHpCoefficient = 0f;
        skullMoveSpeedCoefficient = 0f;
        ghostMoveSpeedCoefficient = 0f;
        gravestoneRespawnTimeCoefficient = 0f;
        realTime = 0f;
        soulYield = 0f;
        currentDay = 0;
        currentAlter = 25;
        currentSkullCount = 2;
        maxSkullCount = 5;

        skullPriceCoefficient = 1;
        equipmentPriceCoefficient = 1;

        heroDeathCount = 0;

        isGameClear = false;
    }

    public void StartNewGame()
    {
        Init();
        while (true)
        {
            if (Managers.Data.IsLoaded)
            {
                Managers.SlaveManager.Clear();
                Managers.SlaveManager.CreateSlave(Constants.Pos.FirstSkull);
                Managers.SlaveManager.CreateSlave(Constants.Pos.SecondSkull);
                SetSystemValues();
            }
            else
            {
                Managers.Data.Init();
                continue;
            }

            break;
        }

        Managers.SpellBook.Init();
        Managers.Soul.Init();
        SpawnManager.Instance.Init();
        ObjectPoolManager.Instance.Init();
    }

    private void SetSystemValues()
    {
        BaseHealAmount = Managers.Data.SystemValueDic[EGameSystemValue.HpRegen.ToString()].Value;
        BaseDecreaseAmount = Managers.Data.SystemValueDic[EGameSystemValue.HpDecrease.ToString()].Value;
        TimePerHeal = Managers.Data.SystemValueDic[EGameSystemValue.HpRegenTime.ToString()].Value;
        TimePerDamage = Managers.Data.SystemValueDic[EGameSystemValue.HpDecreaseTime.ToString()].Value;
    }

    public void ReadyForNextGame()
    {
        Managers.Target.Init();
    }

    public void GameClear()
    {
        // For test scene
        isGameClear = true;
        SendGameDataToAnalytics();
        Managers.Scene.ChangeScene(Define.Scene.GameClearScene);
    }

    public void GameOver()
    {
        isGameClear = false;
        SendGameDataToAnalytics();
        AchievementController.Instance.UpdateAchievementData(Managers.Data.AchievementDataDic[Constants.Achievement.TOTAL_GAMEOVER_COUNT], 1);
        Managers.Scene.ChangeScene(Define.Scene.GameOverScene);
    }

    private void SendGameDataToAnalytics()
    {
#if DEBUG
        return;
#endif
        GameStateData data = new GameStateData()
        {
            AliveDay = currentDay,
            CollectedSoul = collectedSoul,
            CurSkullCount = currentSkullCount,
            IsGameClear = isGameClear,
            MaxSkullCount = maxSkullCount,
            PlayTime = DayManager.Instance.RealTime
        };
        AnalyticsService.Instance.CustomData(
            Constants.AnalyticsData.GameEndEvent.EventName,
            Constants.AnalyticsData.GameEndEvent.ToParams(data)
        );
    }

    private void CallAppSettingChanged()
    {
        AppSetting setting = new AppSetting()
        {
            MasterVolume = PreferencesManager.GetMasterVolume(),
            BGMVolume = PreferencesManager.GetBgmVolume(),
            EffectVolume = PreferencesManager.GetEffectVolume(),
            IsMinimapLeft = PreferencesManager.IsMinimapLeft(),
            IsMasterVolumeOn = PreferencesManager.IsMasterVolumeOn(),
            IsBgmVolumeOn = PreferencesManager.IsBgmVolumeOn(),
            IsEffectVolumeOn = PreferencesManager.IsEffectVolumeOn(),
            Language = PreferencesManager.GetLanguage(),
        };
        OnAppSettingChanged?.Invoke(setting);
    }

    public void ChangeSkullCount(ref int key, int value)
    {
        key += value;
        OnChangeSkullCount?.Invoke();
    }
}