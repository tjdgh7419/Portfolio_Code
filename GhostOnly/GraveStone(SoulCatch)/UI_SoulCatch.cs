using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class UI_SoulCatch : UI_Popup
{
    private int _successReward = 0;
    private int _failReward = 0;
    private int _difficulty = 0;
    private float _starSpeed = 0;

    private bool _isCritical = false;
    private bool _isHelp;

    private Sequence _successSequence;
    private Sequence _lampSequence;
    private Sequence _openSequence;
    private Sequence _starSequence;
    private Sequence _soulSequence;
    private Sequence _releaseSoulSequence;
    private Sequence _lampFailSequence;

    #region enum
    enum Images
    {
        GraveStoneImage,
        //StarCatchBarImage,
        SoulImage,
        SoulCatchDifficultyBG,
        Soul1Image,
        Soul2Image,
        Soul3Image,
        Soul4Image,
        HelpPanel,
    }

    enum Texts
    {
        SoulText,
        HelpText,
    }

    enum Buttons
    {
        SoulCatchButton,
        HelpButton,
    }

    enum HitPoints
    {
        HitPointImage,
    }

    enum GameObjects
    {
        Reward,
        SoulCatchBG,
        SoulBG,
    }
    #endregion


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Time.timeScale = 1.0f;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        Bind<HitPoint>(typeof(HitPoints));
        GetButton((int)Buttons.SoulCatchButton).onClick.AddListener(CatchSoul);
        GetObject((int)GameObjects.Reward).SetActive(false);
        switch (_difficulty)
        {
            case 0: SetEasy(); break;
            case 1: SetNormal(); break;
            case 2: SetHard(); break;
            case 3: SetSkill(); break;
            default: break;
        }
        _init = true;
        SetOpenSequence();
        SoulSequence();
        SoulSequence();

        GetImage((int)Images.HelpPanel).gameObject.SetActive(false);
        GetText((int)Texts.HelpText).text = GetString(Constants.Setting.HELP_GRAVESTONE);
        GetButton((int)Buttons.HelpButton).BindEvent(ToggleHelp);
        _isHelp = false;

        return true;
    }

    private void CatchSoul()
    {
        HitPoint hit = Get<HitPoint>((int)HitPoints.HitPointImage);
        Animator starAnimation = GetImage((int)Images.SoulImage).gameObject.GetComponent<Animator>();
        Button clickButton = GetButton((int)Buttons.SoulCatchButton);

        ReleaseSoulSequence();
        _starSequence.Pause();
        if (hit.starInputChk)
        {
            SuccessRewardSequence();
            SuccessLampSequence();
            Managers.Soul.GetSoul(_successReward + (int)Managers.GameManager.soulYield);
            AchievementController.Instance.UpdateAchievementData(Managers.Data.AchievementDataDic[Constants.Achievement.SUCCESSED_STARCATCH_COUNT], 1);
            clickButton.interactable = false;
            GetObject((int)GameObjects.Reward).SetActive(true);
            RewardSet(_successReward + (int)Managers.GameManager.soulYield);
            DOVirtual.DelayedCall(Constants.Starcatch.ClosePopupTime, Close).SetUpdate(true);
            Managers.Sound.PlaySound(Data.SoundType.StarcatchSuccess);
        }
        else
        {
            FailRewardSequence();
            FailLampSequence();
            Managers.Soul.GetSoul(_failReward);
            clickButton.interactable = false;
            GetObject((int)GameObjects.Reward).SetActive(true);
            RewardSet(_failReward);
            DOVirtual.DelayedCall(Constants.Starcatch.ClosePopupTime, Close).SetUpdate(true);
            Managers.Sound.PlaySound(Data.SoundType.StarcatchFail);
        }
    }

    private void Close()
    {
        SequenceKill();
        Managers.UI.ClosePopupUI();
    }

    private void SetEasy()
    {
        GetImage((int)Images.Soul2Image).gameObject.SetActive(false);
        GetImage((int)Images.Soul3Image).gameObject.SetActive(false);
        GetImage((int)Images.Soul4Image).gameObject.SetActive(false);
    }

    private void SetNormal()
    {
        GetImage((int)Images.Soul3Image).gameObject.SetActive(false);
        GetImage((int)Images.Soul4Image).gameObject.SetActive(false);
        GetImage((int)Images.SoulCatchDifficultyBG).color = Constants.Starcatch.NormalStarcatchColor;
    }

    private void SetHard()
    {
        GetImage((int)Images.Soul4Image).gameObject.SetActive(false);
        GetImage((int)Images.SoulCatchDifficultyBG).color = Constants.Starcatch.HardStarcatchColor;
    }

    private void SetSkill()
    {
        GetImage((int)Images.SoulCatchDifficultyBG).color = Constants.Starcatch.SkillStarcatchColor;
    }

    private void RewardSet(int soul)
    {
        GetObject((int)GameObjects.Reward).SetActive(true);
        GetText((int)Texts.SoulText).text = $"+ {soul}";

        if (_isCritical && soul > 0)
        {
            GetText((int)Texts.SoulText).color = Color.yellow;
        }
    }

    public void Initialized(int difficulty)
    {
        _isCritical = false;

        switch (difficulty)
        {
            case 0:
                this._difficulty = difficulty;
                _starSpeed = Constants.Starcatch.EasySoulSpeed;
                _successReward = Constants.Starcatch.EasySuccessReward;
                _failReward = Constants.Starcatch.EasyFailReward;
                break;
            case 1:
                this._difficulty = difficulty;
                _starSpeed = Constants.Starcatch.NormalSoulSpeed;
                _successReward = Constants.Starcatch.NormalSuccessReward;
                _failReward = Constants.Starcatch.NormalFailReward;
                break;
            case 2:
                this._difficulty = difficulty;
                _starSpeed = Constants.Starcatch.HardSoulSpeed;
                _successReward = Constants.Starcatch.HardSuccessReward;
                _failReward = Constants.Starcatch.HardFailReward;
                break;
            case 3:
                this._difficulty = difficulty;
                _starSpeed = Constants.Starcatch.SkillStarSpeed;
                if (Random.Range(0, 100) < Constants.Starcatch.Chance)
                {
                    _successReward = Constants.Starcatch.SkillSuccessPlusReward;
                    _isCritical = true;
                }
                else
                {
                    _successReward = Constants.Starcatch.SkillSuccessReward;
                }
                _failReward = Constants.Starcatch.SkillFailReward;
                break;

            default: break;
        }
    }

    private void SuccessLampSequence()
    {
        GameObject lamp = GetButton((int)Buttons.SoulCatchButton).gameObject;

        _lampSequence = DOTween.Sequence()
        .Append(lamp.transform.DOShakePosition(0.7f, Constants.Starcatch.LampShakeStrength, Constants.Starcatch.LampShakeVivrato))
        .PrependInterval(Constants.Starcatch.SuccessLampPrependDelay)
        .SetUpdate(true);
    }

    private void FailLampSequence()
    {
        GameObject lamp = GetButton((int)Buttons.SoulCatchButton).gameObject;
        _lampFailSequence = DOTween.Sequence()

        .Append(lamp.GetComponent<CanvasGroup>().DOFade(0.6f, Constants.Starcatch.FailLampAppendFadeDuration))
        .SetUpdate(true);
    }

    private void SuccessRewardSequence()
    {
        GameObject reward = GetObject((int)GameObjects.Reward);
        _successSequence = DOTween.Sequence()
           .OnStart(() =>
           {
               reward.transform.localScale = Vector3.zero;
               reward.GetComponent<CanvasGroup>().alpha = 0;
           })
           .Prepend(reward.transform.DOScale(1, Constants.Starcatch.S_RewardPrependScaleDuration)).SetEase(Ease.Linear)
           .Join(reward.GetComponent<CanvasGroup>().DOFade(1, Constants.Starcatch.S_RewardJoinFadeDuration)).AppendInterval(Constants.Starcatch.S_RewardJoinAppendDelay)
           .Append(reward.transform.DOLocalMove(Constants.Starcatch.S_RewardAppendLocalMove, Constants.Starcatch.S_RewardAppendMoveDuration))
           .Join(reward.transform.DOScale(0, Constants.Starcatch.S_RewardJoinScaleDuration))
           .SetUpdate(true);
    }

    public void SetOpenSequence()
    {
        GameObject window = GetObject((int)GameObjects.SoulCatchBG);
        _openSequence = DOTween.Sequence()
        .OnStart(() =>
        {
            window.SetActive(true);
            window.transform.localScale = Vector3.zero;
            window.GetComponent<CanvasGroup>().alpha = 0;
        })
       .Append(window.transform.DOScale(1, Constants.Starcatch.OpenAppendScaleDuration)).SetEase(Ease.Linear)
       .Join(window.GetComponent<CanvasGroup>().DOFade(1, Constants.Starcatch.OpenJoinFadeDuration))
       .SetUpdate(true);
    }

    public void MoveSoulSequence()
    {
        GameObject star = GetImage((int)Images.SoulImage).gameObject;
        _starSequence = DOTween.Sequence()
       .Prepend(star.transform.DOLocalMoveX
       (Constants.Starcatch.StarPrependMoveX, _starSpeed).SetEase(Ease.InOutSine)).SetRelative()
       .Append(star.transform.DOLocalMoveX
       (Constants.Starcatch.StarAppendMoveX, _starSpeed).SetEase(Ease.InOutSine)).SetRelative()
       .SetLoops(-1)
       .SetUpdate(true);
    }

    public void SoulSequence()
    {
        GameObject soul1 = GetImage((int)Images.Soul1Image).gameObject;
        GameObject soul2 = GetImage((int)Images.Soul2Image).gameObject;
        GameObject soul3 = GetImage((int)Images.Soul3Image).gameObject;
        GameObject soul4 = GetImage((int)Images.Soul4Image).gameObject;

        _soulSequence = DOTween.Sequence()
            .Prepend(soul1.transform.DOLocalMoveY(Constants.Starcatch.Soul1PrependMoveY, 1f))
            .Join(soul2.transform.DOLocalMoveY(Constants.Starcatch.Soul2JoinMoveY, 1f))
            .Join(soul3.transform.DOLocalMoveY(Constants.Starcatch.Soul1PrependMoveY, 1f))
            .Join(soul4.transform.DOLocalMoveY(Constants.Starcatch.Soul2JoinMoveY, 1f))
            .Append(soul1.transform.DOLocalMoveY(Constants.Starcatch.Soul1AppendMoveY, 1f))
            .Join(soul2.transform.DOLocalMoveY(Constants.Starcatch.Soul2JoinMoveY2, 1f))
            .Join(soul3.transform.DOLocalMoveY(Constants.Starcatch.Soul1AppendMoveY, 1f))
            .Join(soul4.transform.DOLocalMoveY(Constants.Starcatch.Soul2JoinMoveY2, 1f))
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetRelative()
            .SetUpdate(true);
    }

    public void ReleaseSoulSequence()
    {
        GameObject soulBG = GetObject((int)GameObjects.SoulBG);

        _releaseSoulSequence = DOTween.Sequence()
            .Append(soulBG.GetComponent<CanvasGroup>().DOFade(0, Constants.Starcatch.ReleaseAppendFadeDuration))
            .SetUpdate(true);
    }

    public void FailRewardSequence()
    {
        GameObject reward = GetObject((int)GameObjects.Reward);
        reward.GetComponent<CanvasGroup>().DOFade(1, Constants.Starcatch.F_RewardFadeDuration)
            .SetUpdate(true);
    }

    private void SequenceKill()
    {
        _successSequence.Kill();
        _lampSequence.Kill();
        _openSequence.Kill();
        _starSequence.Kill();
        _soulSequence.Kill();
        _releaseSoulSequence.Kill();
        _lampFailSequence.Kill();
    }

    private void ToggleHelp()
    {
        _isHelp = !_isHelp;
        GetImage((int)Images.HelpPanel).gameObject.SetActive(_isHelp);

    }
}

