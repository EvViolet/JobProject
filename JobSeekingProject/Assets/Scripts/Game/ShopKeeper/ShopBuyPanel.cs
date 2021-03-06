﻿using UnityEngine;
using UnityEngine.UI;

public class ShopBuyPanel : BasePanel
{
    public RectTransform content;

    private Mugen<ShopCellInfo, ShopCell> sv;

    private GameObject seleObj;

    private int seleIndex;
    private bool waitToBuy;
    private bool quit;
    private bool hasListener;

    public override void ShowMe()
    {
        quit = false;
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 0);
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 800);

        sv = new Mugen<ShopCellInfo, ShopCell>();
        sv.InitCotentAndSVH(content, 580);
        sv.InitItemSizeAndCol(300, 100, 20, 1);
        sv.InitItemResName("ShopCell");
        sv.InitInfos(GameDataMgr.Instance.playerInfo.shopList);

        sv.CheckShowOrHide();

        //选择框
        seleObj = GetControl<Image>("imgSele").gameObject;
        seleIndex = sv.seleIndex;

        //禁用人物移动
        PlayerStatus.Instance.IsForzen = true;

        //告知位置
        Invoke("CheckSeleObjPos", 0.1f);

        //添加选择框监听事件
        AddInputListener();
    }

    private void CheckInput(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                if (seleIndex == 0) return;
                content.anchoredPosition += new Vector2(0, -120);
                sv.CheckShowOrHide();
                seleIndex = sv.seleIndex;
                CheckSeleObjPos();
                break;
            case KeyCode.S:
                if (seleIndex == GameDataMgr.Instance.playerInfo.shopList.Count - 1) return;
                content.anchoredPosition += new Vector2(0, 120);
                sv.CheckShowOrHide();
                seleIndex = sv.seleIndex;
                CheckSeleObjPos();
                break;
            case KeyCode.Escape:
                //退出+防止粘键
                if (quit) return;
                quit = true;
                Debug.Log("quit");
                UIMgr.Instance.PopPanel();
                return;
            case KeyCode.Space:
                //购买+防止粘键
                if (waitToBuy) return;
                waitToBuy = true;
                Debug.Log("buy");
                ConfirmItem();
                return;
        }
    }

    private void Refresh()
    {
        if (GameDataMgr.Instance.playerInfo.shopList.Count == 0)
        {
            UIMgr.Instance.PopPanel();
            DialogBase db = ResMgr.Instance.Load<DialogBase>("PlayerSellOut");
            DialogMgr.Instance.EnqueueDialog(db);
            return;
        }
        for (int i = sv.oldMinIndex; i <= sv.oldMaxIndex; ++i)
        {
            if (sv.nowShowItems[i] != null)
                PoolMgr.Instance.BackObj("ShopCell", sv.nowShowItems[i]);
        }
        sv.nowShowItems.Clear();
        sv.CheckShowOrHide();
        seleIndex = sv.seleIndex;
        CheckSeleObjPos();
    }

    private void CheckSeleObjPos()
    {
        seleObj.transform.SetParent(sv.nowShowItems[seleIndex].transform);
        seleObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        //告知TipsPanel现在是什么物体
        EventCenter.Instance.EventTrigger<object>("CurrentPosShop", sv.nowShowItems[seleIndex].GetComponent<ShopCell>().GetShopCellInfo());
    }

    private void ConfirmItem()
    {
        ShopCell itemWTB = seleObj.GetComponentInParent<ShopCell>();
        if (itemWTB.ifCanBuy())
        {
            string itemName = GameDataMgr.Instance.GetItemInfo(itemWTB.GetShopCellInfo().itemInfo.id).name;
            //点击购买后，弹出确认提示面板
            UIMgr.Instance.ShowConfirmPanel("是否确认购买" + itemName, ConfirmType.TwoBtn, () => 
            { 
                itemWTB.BuyItem(); 
                waitToBuy = false; 
            });
        }
    }
    
    public override void HideMe()
    {
        RemoveInputListener();
        PlayerStatus.Instance.IsForzen = false;
    }

    public override void OnPause()
    {
        RemoveInputListener();
        transform.gameObject.SetActive(false);
    }

    public override void OnResume()
    {
        transform.gameObject.SetActive(true);
        //无论是否买了都要刷新列表
        Refresh();
        Invoke("AddInputListener", 0.1f);
        PlayerStatus.Instance.IsForzen = true;
        PlayerStatus.Instance.InputEnable = false;
    }

    private void RemoveInputListener()
    {
        if (hasListener)
        {
            hasListener = false;
            EventCenter.Instance.RemoveEventListener<KeyCode>("xPress", CheckInput);
        }
    }

    private void AddInputListener()
    {
        if (!hasListener)
        {
            hasListener = true;
            EventCenter.Instance.AddEventListener<KeyCode>("xPress", CheckInput);
        }
    }

}
