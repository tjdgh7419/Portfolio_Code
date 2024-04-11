using Data;
using DG.Tweening;
using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class UI_Coffin : UI_Popup
{
    private UI_CoffinItem _selectedItem;
    private UI_CoffinItem _hoveredItem;

    private Vector3 _spawnPosition;
    private List<UI_CoffinItem> coffinItems = new List<UI_CoffinItem>();
    private List<string> equipments = new List<string>();
    private Sequence _clickPanelSequence;
    private Sequence _purchaseFailSequence;
    private Sequence _openCoffinSequence;

    #region Enum

    enum GameObjects
    {
        Content,
        InfoPanel,
        ClickPanel,
        ClickPanelBG,
        Panel,
    }

    enum Texts
    {
        NameTxt,
        ExplanationTxt,
        PriceTxt,
        CheckPurchaseText,
        WarningText,
        PurchaseText,
        CancelText,
    }

    enum Buttons
    {
        CloseButton,
        PurchaseButton,
        CancelButton,
    }

    #endregion

    private void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        SequenceKill();
    }

    public override bool Init()
    {
        if (!base.Init())
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
       
        GetObject((int)GameObjects.ClickPanel).SetActive(false);
        GetObject((int)GameObjects.InfoPanel).SetActive(false);
        GetText((int)Texts.WarningText).gameObject.SetActive(false);

        GetButton((int)Buttons.CloseButton).BindEvent(OnClickedCloseButton);
        GetButton((int)Buttons.PurchaseButton).BindEvent(OnClickedPurchaseButton);
        GetButton((int)Buttons.CancelButton).BindEvent(OnClickPanelCancelButton);
        GetObject((int)GameObjects.Panel).transform.localScale = Vector3.zero;
        OpenCoffinSequence();
     
        foreach (Transform child in GetObject((int)GameObjects.Content).transform)
        {
            Managers.Resource.Destroy(child.gameObject);
            coffinItems.Clear();
        }

        foreach(string key in Managers.Data.CoffinDataDic.Keys)
        {
            if (Managers.Data.CoffinDataDic[key].type == CoffinType.Equipment)
            {
                equipments.Add(Managers.Data.CoffinDataDic[key].coffinId);
            }

            GameObject item = Managers.UI.MakeSubItem<UI_CoffinItem>
                (GetObject((int)GameObjects.Content).transform, "CoffinItem").gameObject;
            UI_CoffinItem coffinItem = item.GetOrAddComponent<UI_CoffinItem>();
            coffinItems.Add(coffinItem);

            if (coffinItem.Init())
            {
                coffinItem.SetInfo(Managers.Data.CoffinDataDic[key], SelectItem, ShowInfoPanel);        
            }
        }
        return true;
    }

    #region OnClick

    private void OnClickedCloseButton()
    {
        SequenceKill();
        Managers.Sound.PlaySound(Data.SoundType.Click);
        Time.timeScale = 1;    
        Managers.UI.ClosePopupUI(this);

        //todo remove
        gameObject.SetActive(false);
    }

    private void OnClickedPurchaseButton()
    {
        if (_selectedItem == null)
        {
            return;
        }

        switch (_selectedItem.coffinData.type)
        {
            case CoffinType.Skull:
                PurchaseSkull();
                break;
            case CoffinType.Equipment:
                PurchaseEquipment(false);
                break;
            case CoffinType.Random:
                PurchaseRandom();
                break;
        }
            
        ShowInfoPanel(_selectedItem);    
    }
    #endregion

    private void ShowInfoPanel(UI_CoffinItem data)
    {     
        if(_selectedItem != null)
        {
            return;
        }
        if(data == _hoveredItem)
        {
            return;
        }

        if (_hoveredItem != null)
        {
            _hoveredItem.TurnOffHightLightItem();
        }

        _hoveredItem = data;
        
        if (data == null)
        {
            GetObject((int)GameObjects.InfoPanel).SetActive(false);
            return;
        }

       
        _hoveredItem.TurnOnHightlightItem();
        
        GetText((int)Texts.NameTxt).text = GetString(_hoveredItem.coffinData.name);
        GetText((int)Texts.ExplanationTxt).text = GetString(_hoveredItem.coffinData.explanation);

        int price;
        if(_hoveredItem.coffinData.type == CoffinType.Skull)
        {
            price = _hoveredItem.coffinData.price * Managers.GameManager.skullPriceCoefficient;
        }
        else
        {
            price = _hoveredItem.coffinData.price * Managers.GameManager.equipmentPriceCoefficient;
        }
        GetText((int)Texts.PriceTxt).text = $"x {price}";   

        GetObject((int)GameObjects.InfoPanel).SetActive(true);
    }

    private void SelectItem(UI_CoffinItem item)
    {
        _selectedItem = item;
        ClickPanelSequence();
        ClickPanelUIUpdate();

        if (Managers.GameManager.currentSkullCount == Managers.GameManager.maxSkullCount
            && item.coffinData.type == CoffinType.Skull)
        {   
            GetText((int)Texts.CheckPurchaseText).text = string.Empty;
            WarningTextOpen(Constants.Coffin.COFFIN_WARNING_SKULL);
            GetObject((int)GameObjects.ClickPanel).SetActive(true);
            GetButton((int)Buttons.PurchaseButton).gameObject.SetActive(false);
            return;
        }
        GetObject((int)GameObjects.ClickPanel).SetActive(true);     
    }

    private void ClickPanelUIUpdate()
    {
        GetText((int)Texts.CheckPurchaseText).text = string.Empty;
        GetObject((int)GameObjects.ClickPanel).SetActive(true);
        GetButton((int)Buttons.PurchaseButton).gameObject.SetActive(true);
        GetButton((int)Buttons.CloseButton).gameObject.SetActive(true);
        SetPurchaseText();
    }


    private void PurchaseSkull()
    {
        if (Managers.Soul.CheckSoul(_selectedItem.coffinData.price * Managers.GameManager.skullPriceCoefficient))
        {        
            if (Managers.GameManager.currentSkullCount >= Managers.GameManager.maxSkullCount)
            {         
                return;
            }
            Managers.Soul.UseSoul(_selectedItem.coffinData.price * Managers.GameManager.skullPriceCoefficient);
            Managers.GameManager.skullPriceCoefficient += 1;
            Managers.GameManager.ChangeSkullCount(ref Managers.GameManager.currentSkullCount, 1);
            Managers.SlaveManager.CreateSlave(Constants.Pos.CoffinSkull + Random.insideUnitCircle.normalized);
            AchievementController.Instance.UpdateAchievementData(Managers.Data.AchievementDataDic[Constants.Achievement.TOTAL_SKULL_COUNT], 1);
            OnClickPanelPurchaseButton();
            _selectedItem = null;
            Managers.Sound.PlaySound(SoundType.Purchase);
        }
        else
        {
            WarningTextOpen(Constants.Coffin.COFFIN_WARNING_SOUL);
            PurchaseFailSequence();
            Managers.Sound.PlaySound(SoundType.PurchaseFail);
        }
    }

    private void PurchaseEquipment(bool isRandom)
    {
        if (Managers.Soul.CheckSoul(_selectedItem.coffinData.price * Managers.GameManager.equipmentPriceCoefficient))
        {
            Managers.Soul.UseSoul(_selectedItem.coffinData.price * Managers.GameManager.equipmentPriceCoefficient);
            Managers.GameManager.equipmentPriceCoefficient += 1;

            _spawnPosition = Constants.Pos.CoffinEquip + Random.insideUnitCircle.normalized * 2;

            if(isRandom)
            {
                Managers.Resource.Instantiate($"Equip/{equipments[Random.Range(0, equipments.Count)]}", _spawnPosition);
            }
            else
            {
                Managers.Resource.Instantiate($"Equip/{_selectedItem.coffinData.coffinId}", _spawnPosition);
            }
            AchievementController.Instance.UpdateAchievementData(Managers.Data.AchievementDataDic[Constants.Achievement.TOTAL_ENVIROMENT_COUNT], 1);
            OnClickPanelPurchaseButton();
            _selectedItem = null;
            Managers.Sound.PlaySound(SoundType.Purchase);
        }
        else
        {
            WarningTextOpen(Constants.Coffin.COFFIN_WARNING_SOUL);
            PurchaseFailSequence();
            Managers.Sound.PlaySound(SoundType.PurchaseFail);
        }
    }
    
    private void WarningTextOpen(string warningText)
    {
        GetText((int)Texts.WarningText).text = GetString(warningText);
        GetText((int)Texts.WarningText).gameObject.SetActive(true);
    }

    private void WarningTextOff()
    {
        GetText((int)Texts.WarningText).gameObject.SetActive(false);
    }

    private void SetPurchaseText()
    {
        GetText((int)Texts.CheckPurchaseText).text = $"{GetString(_selectedItem.coffinData.name)} {GetString(Constants.Coffin.COFFIN_CHECK_PURCHASE)}";
        GetText((int)Texts.PurchaseText).text = GetString(Constants.Coffin.COFFIN_PURCHASE);
        GetText((int)Texts.CancelText).text = GetString(Constants.Coffin.COFFIN_CANCEL);
    }

    private void PurchaseRandom()
    {
        PurchaseEquipment(true);
    }

    private void OnClickPanelCancelButton()
    {
        SequenceKill();
        Managers.Sound.PlaySound(SoundType.Click);
        _selectedItem = null;
        _hoveredItem.OnCancelHover();
        WarningTextOff();
        GetObject((int)GameObjects.ClickPanel).SetActive(false);   
    }

    private void OnClickPanelPurchaseButton()
    {
        SequenceKill();
        _selectedItem = null;
        WarningTextOff();
        GetObject((int)GameObjects.ClickPanel).SetActive(false);
    }

    private void ClickPanelSequence()
    {
        GameObject bg = GetObject((int)GameObjects.ClickPanelBG);
        _clickPanelSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                bg.transform.localScale = Vector3.zero;
            })
            .Append(bg.transform.DOScale(1, Constants.Coffin.ClickPanelAppendScaleDuration)).SetEase(Ease.Linear).SetUpdate(true);
    }

    private void PurchaseFailSequence()
    {
        GameObject panel = GetObject((int)GameObjects.ClickPanelBG);
        _purchaseFailSequence = DOTween.Sequence()
            .Append(panel.transform.DOShakePosition(Constants.Coffin.PurchaseFailAppendShakeDuration, 
            Constants.Coffin.PurchaseFailAppendShakeStrength, Constants.Coffin.PurchaseFailAppendShakeVibrato)).SetUpdate(true);
    }

    private void OpenCoffinSequence()
    {
        GameObject panel = GetObject((int)GameObjects.Panel);
        _openCoffinSequence = DOTween.Sequence()        
            .Append(panel.transform.DOScale(1, Constants.Coffin.ClickPanelAppendScaleDuration)).SetEase(Ease.Linear).SetUpdate(true);
    }

    private void SequenceKill()
    {
        _openCoffinSequence.Kill(true);
        _clickPanelSequence.Kill(true);
        _purchaseFailSequence.Kill(true);    
    }
}