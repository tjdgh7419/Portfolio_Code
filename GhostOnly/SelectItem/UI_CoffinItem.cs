using Data;
using DG.Tweening;
using System;
using UnityEngine;
using Util;

public class UI_CoffinItem : UI_Base
{
    public CoffinData coffinData;
    private Action<UI_CoffinItem> _onClick;
    private Action<UI_CoffinItem> _onHover;
    public UI_Coffin uiCoffinData;
    private Sequence _itemHoverSequence;

    enum Images
    {
        HighlightImg,
        CoffinImg,
        HoverImage,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));

        GetImage((int)Images.HoverImage).gameObject.BindEvent(OnClickedItem);
        GetImage((int)Images.HoverImage).gameObject.BindEvent(OnCancelHover, Define.UIEvent.PointerExit);
        GetImage((int)Images.HoverImage).gameObject.BindEvent(OnHover, Define.UIEvent.PointerEnter);
        return true;
    }
 

    public void SetInfo(CoffinData coffinData, Action<UI_CoffinItem> onClick, Action<UI_CoffinItem> onHover)
    {
        this.coffinData = coffinData;

        _onClick = onClick;
        _onHover = onHover;

        GetImage((int)Images.CoffinImg).sprite = Resources.Load<Sprite>($"Sprites/Coffin/{this.coffinData.coffinId}"); 
        GetImage((int)Images.HighlightImg).gameObject.SetActive(false);
    }


    public void OnClickedItem()
    {
        Managers.Sound.PlaySound(SoundType.Interaction);
        _onClick?.Invoke(this);
    }

    public void OnCancelHover()
    {
        _onHover?.Invoke(null);
        _itemHoverSequence.Kill(true);
        GetImage((int)Images.CoffinImg).transform.position = GetImage((int)Images.HoverImage).transform.position;   
    }

    public void OnHover()
    {
        ItemHoverSequence();
        _onHover?.Invoke(this);
    }

    public void TurnOnHightlightItem()
    {
        GetImage((int)Images.HighlightImg).gameObject.SetActive(true);      
    }

    public void TurnOffHightLightItem()
    {
        GetImage((int)Images.HighlightImg).gameObject.SetActive(false);
    }

    public void ClickedSkull()
    {
        GetImage((int)Images.CoffinImg).sprite = Resources.Load<Sprite>($"Sprites/Coffin/{this.coffinData.coffinId}_1");
    }

    public void ClickedEquipment()
    {
        GetImage((int)Images.CoffinImg).sprite = Resources.Load<Sprite>($"Sprites/Coffin/{this.coffinData.coffinId}");   
    }

    public void ItemHoverSequence()
    {      
        GameObject item = GetImage((int)Images.CoffinImg).gameObject;
        _itemHoverSequence = DOTween.Sequence()
       .Prepend(item.transform.DOLocalMoveY(Constants.Coffin.ItemHoverPrependMoveY, Constants.Coffin.ItemHoverPrependMoveDuration)).SetRelative().SetUpdate(true); 
    }

    private void OnDisable()
    {
        _itemHoverSequence.Kill();
    }
}