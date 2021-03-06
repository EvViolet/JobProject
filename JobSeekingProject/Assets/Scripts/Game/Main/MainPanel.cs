﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainPanel : BasePanel,IObserver
{
    public GameObject[] panelList;
    private int currentPanelID;
    private bool bpOpen;
    private bool menuOpen;
    public GameObject hpHeader;
    private int bloodSlotNum;
    private int lastHp;
    private int currentHp;
    public static bool changinePanel;
    private bool hadListener;

    /// <summary>
    /// 显示界面时调用，先于Start运行
    /// </summary>
    public override void ShowMe()
    {
        GetControl<Text>("Moneytxt").text = GameDataMgr.Instance.playerInfo.Money.ToString();
        lastHp = GameDataMgr.Instance.playerInfo.HP;
        UpdateData(GameDataMgr.Instance.playerInfo);
        StartCoroutine(UpdateHpCell(lastHp));
        InputMgr.Instance.StartOrEndCheck(true);
        hadListener = false;
        if (!hadListener)
        {
            hadListener = true;
            EventCenter.Instance.AddEventListener<KeyCode>("xPress", CheckInputDown);
        }
        EventCenter.Instance.AddEventListener<int>("PanelChange", ShowPanel);
        GameDataMgr.Instance.AttachPlayerData(this);
        bpOpen = false;
        menuOpen = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    /// <summary>
    /// 面板生成（被背包面板调用）
    /// </summary>
    void ShowPanel(int dir)
    {
        currentPanelID += dir;
        if (currentPanelID < 0)
            currentPanelID = 2;
        if (currentPanelID > 2)
            currentPanelID = 0;
        UIMgr.Instance.ShowPanel<BasePanel>(panelList[currentPanelID].name, E_UI_Layer.Mid,(obj)=> {
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920 * dir, 0);
            changinePanel = true;
        });
    }

    /// <summary>
    /// 检测输入按钮
    /// </summary>
    private void CheckInputDown(KeyCode key)
    {
        if (changinePanel) return;
        if (!bpOpen)
        {
            if (key == KeyCodeMgr.Instance.Bag.CurrentKey && !PlayerStatus.Instance.IsForzen)
            {
                bpOpen = true;
                UIMgr.Instance.ShowPanel<BadgePanel>("BadgePanel", E_UI_Layer.Mid);
                GameManager.Instance.playerGO.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                currentPanelID = 1;
            }
            if (key == KeyCodeMgr.Instance.Menu.CurrentKey && !PlayerStatus.Instance.IsForzen && !menuOpen)
            {
                menuOpen = true;
                UIMgr.Instance.ShowPanel<BasePanel>("PausePanel", E_UI_Layer.top);
                GameManager.Instance.playerGO.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Cursor.visible = true;
            }
            else if (key == KeyCodeMgr.Instance.Menu.CurrentKey && menuOpen)
            {
                UIMgr.Instance.PopPanel();
                menuOpen = false;
                PlayerStatus.Instance.InputEnable = !bpOpen;
                PlayerStatus.Instance.IsForzen = bpOpen;
                GameManager.Instance.TimePause = false;
            }
        }
        else
        {
            if (key == KeyCodeMgr.Instance.Bag.CurrentKey || key == KeyCodeMgr.Instance.Menu.CurrentKey)
            {
                UIMgr.Instance.PopPanel();
                bpOpen = false;
                PlayerStatus.Instance.InputEnable = !bpOpen;
                PlayerStatus.Instance.IsForzen = bpOpen;
                BadgeMgr.Instance.PassBadgeSkillData();
            }
        }
    }

    /// <summary>
    /// 订阅者交给观察者的订阅更新函数
    /// </summary>
    /// <param name="sub"></param>
    public void UpdateData(ISubject sub)
    {
        if (!gameObject.activeSelf) return;
        Player temp = sub as Player;
        GetControl<Image>("MPBall").material.DOFloat(temp.SP / 25.0f - 2.0f, "MP", 0.5f);
        ShowMoneyAdd(GetControl<Text>("Moneytxt"), temp.Money, 1.5f, GetControl<Text>("MoneyAddtxt"));
        if (bloodSlotNum != temp.MaxHp + PlayerStatus.Instance.tempMaxHp)
        {
            bloodSlotNum = temp.MaxHp + PlayerStatus.Instance.tempMaxHp;
            UIMgr.Instance.CreatChildren("HPCell", hpHeader, bloodSlotNum);
            StartCoroutine(UpdateHpCell(temp.HP + PlayerStatus.Instance.tempMaxHp));
            lastHp = PlayerStatus.Instance.CurrentHealth;
            return;
        }
        currentHp = PlayerStatus.Instance.CurrentHealth;
        if (lastHp != currentHp)
        {
            //说明是治疗
            if (lastHp < currentHp)
            {
                for (int i = 0; i < currentHp; ++i)
                {
                    hpHeader.transform.GetChild(i).GetComponent<Animator>().SetTrigger("Heal");
                    hpHeader.transform.GetChild(i).GetComponent<Animator>().SetBool("Empty", false);
                }
            }
            //说明是扣血
            else
            {
                for(int i = currentHp; i < lastHp; ++i)
                {
                    hpHeader.transform.GetChild(i).GetComponent<Animator>().SetTrigger("Damage");
                    hpHeader.transform.GetChild(i).GetComponent<Animator>().SetBool("Empty", true);
                }
            }
            lastHp = currentHp;
        }
        else
        {
            for(int i = 0; i < hpHeader.transform.childCount; ++i)
            {
                hpHeader.transform.GetChild(i).GetComponent<Animator>().SetBool("Empty", i < currentHp ? false : true);
            }
        }
    }

    /// <summary>
    /// 金钱数字增减滚动效果
    /// </summary>
    public void ShowMoneyAdd(Text coinText, float coinValue, float lerpTime, Text moneyChange)
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
            StartCoroutine(ShowMoneyAddAnim(coinText, coinValue, lerpTime, moneyChange));
    }
    IEnumerator ShowMoneyAddAnim(Text coinText, float desiredNumber, float lerpTime, Text moneyChange)
    {
        float temp = GameDataMgr.Instance.moneyDelta;
        moneyChange.color = Color.white;
        if (temp == 0)
            moneyChange.color = Color.clear;
        moneyChange.text = temp > 0 ? "+" + temp.ToString() : (temp).ToString();
        float time = 0.5f;
        yield return new WaitForSeconds(time);
        GameDataMgr.Instance.moneyDelta = 0;
        float initialNumber = 0;
        if (!string.IsNullOrEmpty(coinText.text))
        {
            try
            {
                initialNumber = System.Convert.ToInt32(coinText.text);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }
        float dis = desiredNumber - initialNumber;


        float currentNumber = initialNumber;
        while (currentNumber != desiredNumber)
        {
            currentNumber += (lerpTime * Time.deltaTime) * dis;
            if (dis > 0 && (currentNumber >= desiredNumber))
            {
                currentNumber = desiredNumber;
            }
            else if (dis < 0 && (currentNumber <= desiredNumber))
            {
                currentNumber = desiredNumber;
            }
            coinText.text = currentNumber.ToString("0");
            yield return new WaitForSeconds(0.01f);
        }

        coinText.text = desiredNumber.ToString();
        moneyChange.color = Color.clear;
    }

    /// <summary>
    /// 显示对话或者商店面板时界面暂停
    /// </summary>
    public override void OnPause()
    {
        PlayerStatus.Instance.InputEnable = false;
        PlayerStatus.Instance.IsForzen = true;
        gameObject.SetActive(false);
        if (!bpOpen && !menuOpen && !ScenesMgr.Instance.goingScene && PlayerStatus.Instance.IsAlive && hadListener)
        {
            hadListener = false;
            EventCenter.Instance.RemoveEventListener<KeyCode>("xPress", CheckInputDown);
        }
    }

    /// <summary>
    /// 恢复界面并更新变化
    /// </summary>
    public override void OnResume()
    {
        gameObject.SetActive(true);
        PlayerStatus.Instance.InputEnable = true;
        UpdateData(GameDataMgr.Instance.playerInfo);
        if (!bpOpen && !menuOpen && !hadListener)
        {
            hadListener = true;
            EventCenter.Instance.AddEventListener<KeyCode>("xPress", CheckInputDown);
        }
        menuOpen = false;
    }

    private IEnumerator UpdateHpCell(int hp)
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i <= hp - 1; i++)
        {
            hpHeader.transform.GetChild(i).GetComponent<Animator>().SetBool("Empty", false);
        }
        for(int i = hp; i < hpHeader.transform.childCount; ++i)
        {
            hpHeader.transform.GetChild(i).GetComponent<Animator>().SetBool("Empty", true);
        }
    }
}
