﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions.Must;

public class GameManager : UnityBaseManager<GameManager>
{
    [HideInInspector] public GameObject playerGO;
    private Animator crossFadeAnim;
    private float gameTime;
    public bool TimePause { get; set; }

    public CinemachineVirtualCamera cvc { get; set; }

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        gameTime = 0;
        InputMgr.Instance.StartOrEndCheck(true);
        KeyCodeMgr.Instance.Init();
        UIMgr.Instance.ShowPanel<BasePanel>("MainStartPanel", E_UI_Layer.Bot);
    }

    private void Update()
    {
        if (!PlayerStatus.Instance.IsAlive || TimePause) return;
        gameTime += Time.deltaTime;
    }

    /// <summary>
    /// 中陷阱但未死时调用
    /// </summary>
    public void RespawnPlayer(Vector2 position)
    {
        ResMgr.Instance.LoadAsync<GameObject>("Player", (obj) =>
        {
            obj.transform.position = position;
            playerGO = obj;
            cvc = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
            cvc.m_Follow = obj.transform;
            //固定特殊技能的初始化
            SkillMgr.Instance.Init();
            //护符及护符技能的初始化
            BadgeMgr.Instance.Init();
            crossFadeAnim = GameObject.Find("CrossFade").GetComponentInChildren<Animator>();
        });
    }

    /// <summary>
    /// 真正死亡时调用
    /// </summary>
    public void RebornPlayer()
    {
        PlayerStatus.Instance.ResetPlayerStatus();
        PlayerStatus.Instance.Init();
        KeyCodeMgr.Instance.Init();
        TaskMgr.Instance.Init();
        LevelManager.Instance.Init();
        MapMgr.Instance.Init();
        //生成真正的人物角色（交给地图加载器调用）
        //初始化任务者管理器，记录并转移中转任务（交给地图加载器调用）
        //通知各位订阅者更新
        GameDataMgr.Instance.playerInfo.Notify();
        InputMgr.Instance.StartOrEndCheck(true);
    }

    public void CameraFollowPlayer(bool follow)
    {
        if (follow)
        {
            cvc.m_Follow = playerGO.transform;
        }
        else
        {
            cvc.m_Follow = null;
        }
    }

    public void FadeOut()
    {
        crossFadeAnim.SetTrigger("FadeOut");
    }

    public void FadeIn()
    {
        crossFadeAnim.Play("FadeIn");
    }

    public void UpdateGameTime()
    {
        TimePause = true;
        GameDataMgr.Instance.UpdatePlayTime(gameTime);
    }
}
