﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PEManager : BaseManager<PEManager>
{
    private Dictionary<string, Queue<GameObject>> peDic = new Dictionary<string,Queue<GameObject>>();
    private Dictionary<string, Queue<GameObject>> poDic = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// 时段性粒子效果（播放到输入时间就调回对象池）
    /// </summary>
    public void GetParticleEffectDuringTime(string peName, float time, Transform parent, Vector3 pos, Vector3 localScale, Quaternion rotation)
    {
        PoolMgr.Instance.GetObj(peName, (obj) =>
        {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localScale = localScale;
            obj.transform.localRotation = rotation;
            ParticleSystem temp = obj.GetComponent<ParticleSystem>();
            temp.Play();
            if (!peDic.ContainsKey(peName))
                peDic.Add(peName, new Queue<GameObject>());
            peDic[peName].Enqueue(obj);
            MonoMgr.Instance.StartCoroutine(BackParticleEffectAfterTime(peName, time));
        });
    }
    private IEnumerator BackParticleEffectAfterTime(string peName,float time)
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        while (time > 0)
        {
            time -= 0.1f;
            yield return delay;
        }
        PoolMgr.Instance.BackObj(peName, peDic[peName].Dequeue());

    }

    /// <summary>
    /// 一次性粒子效果（播放完自动回对象池）
    /// </summary>
    public void GetParticleEffectOneOff(string peName, Transform parent, Vector3 pos, Vector3 localScale, Quaternion rotation)
    {
        PoolMgr.Instance.GetObj(peName, (obj) => 
        {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localScale = localScale;
            obj.transform.localRotation = rotation;
            ParticleSystem temp = obj.GetComponent<ParticleSystem>();
            temp.Play();
            if (!peDic.ContainsKey(peName))
                peDic.Add(peName, new Queue<GameObject>());
            peDic[peName].Enqueue(obj);
            MonoMgr.Instance.StartCoroutine(BackParticleEffectAfterPlayed(peName, temp));
        });
    }
    private  IEnumerator BackParticleEffectAfterPlayed(string peName,ParticleSystem pe)
    {
        WaitForSeconds delay = new WaitForSeconds(0.5f);
        while (!pe.isStopped)
        {
            yield return delay;
        }
        PoolMgr.Instance.BackObj(peName, peDic[peName].Dequeue());
    }

    public void GetParticleEffectByTime(string peName, Transform parent, Vector3 pos, Vector3 localScale, Quaternion rotation, float time)
    {
        MonoMgr.Instance.StartCoroutine(BeforeGetParticleEffect(peName, parent, pos, localScale, rotation, time));
    }

    private IEnumerator BeforeGetParticleEffect(string peName, Transform parent, Vector3 pos, Vector3 localScale, Quaternion rotation, float time)
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        while (time > 0)
        {
            time -= 0.1f;
            yield return delay;
        }
        PoolMgr.Instance.GetObj(peName, (obj) =>
        {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localScale = localScale;
            obj.transform.localRotation = rotation;
            ParticleSystem temp = obj.GetComponent<ParticleSystem>();
            temp.Play();
            if (!peDic.ContainsKey(peName))
                peDic.Add(peName, new Queue<GameObject>());
            peDic[peName].Enqueue(obj);
            MonoMgr.Instance.StartCoroutine(BackParticleEffectAfterPlayed(peName, temp));
        });
    }

    /// <summary>
    /// 特效粒子，待调用回池
    /// </summary>
    public void GetParticleEffect(string peName,Transform parent,Vector3 pos,Vector3 localScale,Quaternion rotation)
    {
        PoolMgr.Instance.GetObj(peName, (obj) =>
        {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localScale = localScale;
            obj.transform.localRotation = rotation;
            ParticleSystem temp = obj.GetComponent<ParticleSystem>();
            temp.Play();
            if (!peDic.ContainsKey(peName))
                peDic.Add(peName, new Queue<GameObject>());
            peDic[peName].Enqueue(obj);
        });
    }

    public void BackParticleEffect(string peName)
    {
        if (peDic[peName].Count == 0) return;
        PoolMgr.Instance.BackObj(peName, peDic[peName].Dequeue());
    }

    /// <summary>
    /// 特效物体（非粒子系统的）无时间，待待用回池
    /// </summary>
    public void GetParticleObject(string poName,Transform parent,Vector3 pos)
    {
        PoolMgr.Instance.GetObj(poName, (obj) =>
        {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            if (!poDic.ContainsKey(poName))
                poDic.Add(poName, new Queue<GameObject>());
            poDic[poName].Enqueue(obj);
        });
    }

    /// <summary>
    /// 时段性特效物体，时间到了调回池
    /// </summary>
    public void GetParticleObjectDuringTime(string poName, Transform parent, Vector3 pos, Vector3 localScale,Quaternion rotation, float time)
    {
        PoolMgr.Instance.GetObj(poName, (obj) =>
        {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localScale = localScale;
            obj.transform.localRotation = rotation;
            if (!poDic.ContainsKey(poName))
                poDic.Add(poName, new Queue<GameObject>());
            poDic[poName].Enqueue(obj);
        });
        MonoMgr.Instance.StartCoroutine(BackPaticleObjectAfterTime(poName, time));
    }
    IEnumerator BackPaticleObjectAfterTime(string poName,float time)
    {
        while (time > 0)
        {
            time -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        PoolMgr.Instance.BackObj(poName, poDic[poName].Dequeue());
    }

    /// <summary>
    /// 手动调回池
    /// </summary>
    public void BackParticleObject(string poName)
    {
        PoolMgr.Instance.BackObj(poName, poDic[poName].Dequeue());
    }

    /// <summary>
    /// 跳转场景时供总管理器调用
    /// </summary>
    public void Clear()
    {
        foreach(Queue<GameObject> gos in peDic.Values)
        {
            for(int i = 0; i < gos.Count; ++i)
            {
                GameObject go = gos.Dequeue();
                GameObject.Destroy(go);
            }
        }
        foreach (Queue<GameObject> gos in poDic.Values)
        {
            for (int i = 0; i < gos.Count; ++i)
            {
                GameObject go = gos.Dequeue();
                GameObject.Destroy(go);
            }
        }
    }
}
