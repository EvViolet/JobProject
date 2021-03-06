﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "New Event", menuName = "Dialogue/Event")]
public class EventBehaviourForDialogSystem : ScriptableObject
{
    public void OpenShopBuyPanel()
    {
        Debug.Log("打开买商店");
        //如果没东西可卖了，输出对话。
        if (GameDataMgr.Instance.playerInfo.shopList.Count == 0)
        {
            DialogBase db = ResMgr.Instance.Load<DialogBase>("ShopSellOut");
            DialogMgr.Instance.EnqueueDialog(db);
        }
        else
            UIMgr.Instance.ShowPanel<BasePanel>("ShopBuyPanel", E_UI_Layer.Mid);
    }

    public void OpenShopSellPanel()
    {
        Debug.Log("打开卖商店");
        int temp = 0;
        foreach(ItemInfo info in GameDataMgr.Instance.playerInfo.numItem)
        {
            if (info.num == 0)
                temp++;
        }
        //证明背包没东西卖了
        if(temp == GameDataMgr.Instance.playerInfo.numItem.Count)
        {
            DialogBase db = ResMgr.Instance.Load<DialogBase>("PlayerSellOut");
            DialogMgr.Instance.EnqueueDialog(db);
        }
        else
            UIMgr.Instance.ShowPanel<BasePanel>("ShopSellPanel", E_UI_Layer.Mid);
    }

    public void ShowNextDialog(DialogBase nextDB)
    {
        if (nextDB != null) DialogMgr.Instance.EnqueueDialog(nextDB);
        PlayerStatus.Instance.IsForzen = false;
    }

    public void OpenLevel(string targetID)
    {
        if (!LevelManager.Instance.hasOpenDungeon)
        {
            if (200 <= GameDataMgr.Instance.playerInfo.Money)
            {
                LevelManager.Instance.hasOpenDungeon = true;
                //扣钱
                MoneyDetails md = new MoneyDetails();
                md.moneyAmount = -200;
                PlayerStatus.Instance.ChangeMoney(md);
                //开门
                GameObject gate = GameObject.Find("Gate");
                GameObject whiteGate = GameObject.Find("WhiteGate");
                gate.GetComponent<Animator>().SetTrigger("Open");
                whiteGate.GetComponent<Animator>().SetTrigger("Open");
                MusicMgr.Instance.PlaySound("ColLevelTriggerAudio", false);
                LevelManager.Instance.SetDungeonID(targetID, targetID);
            }
            else
            {
                DialogBase db = ResMgr.Instance.Load<DialogBase>("DonotHaveEnoughMoneyToOpen");
                if (db != null) DialogMgr.Instance.EnqueueDialog(db);
            }
        }
        else
        {
            DialogBase db = ResMgr.Instance.Load<DialogBase>("HasOpenDungeon");
            if (db != null) DialogMgr.Instance.EnqueueDialog(db);
        }
        PlayerStatus.Instance.IsForzen = false;
    }
}
