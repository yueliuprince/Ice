using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    protected float restTime = 0f;
    protected float coldDownTime = 5f;

    public float ColdDownTime {
        get { return coldDownTime; }
        set {
            coldDownTime = value;
            if (restTime > coldDownTime) restTime = coldDownTime;
        }
    }

    public float Progress { get { return 1 - (restTime / coldDownTime); } }

    public bool isCd = false;

    protected Action TimeOut;
    protected virtual void _AfterUpdate() { }

    public void OnTimeOut(Action callBack) { TimeOut += callBack; }

    void LateUpdate()
    {
        if (!isCd) return;

        if (restTime > 0f)
        {
            restTime -= Time.deltaTime;
            isCd = true;
        }
        else
        {
            isCd = false;
            if (TimeOut != null) TimeOut();
        }
        _AfterUpdate();
    }


    /// <summary>
    /// 重置冷却时间
    /// </summary>
    public void ResetCd()
    {
        isCd = true;
        restTime = coldDownTime;
    }



}
