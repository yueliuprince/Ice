using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockTimer : Timer
{
    [SerializeField] private Transform startPtr;
    [SerializeField] private Transform ptr;
    [SerializeField] private Image background;
    [SerializeField] private Image fillImage;
    private Image ptrImage;
    private Vector3 startLocalScale;

    public Color normalColor = Color.white;
    public Color abnormalColor = Color.white;
    public Color normalPtrColor = Color.white;
    public Color abnormalPtrColor = Color.white;

    private void Awake()
    {
        ptrImage = ptr.GetComponent<Image>();
        startLocalScale = transform.localScale;
        Stop();
    }

    public enum AnimStyle
    {
        Normal,
        Abnormal,
    }

    public void StartUp(Vector3 worldPos, float duration, AnimStyle clockStyle, float scaleMultipier = 1)
    {
        switch (clockStyle)
        {
            case AnimStyle.Normal: background.color = normalColor; ptrImage.color = normalPtrColor; break;
            case AnimStyle.Abnormal: background.color = abnormalColor; ptrImage.color = abnormalPtrColor; break;
        }
        Vector3 pos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = pos;
        transform.localScale = startLocalScale * scaleMultipier;

        ColdDownTime = duration;
        ResetCd();
        ptr.SetPositionAndRotation(startPtr.position, startPtr.rotation);
        fillImage.fillAmount = 0;
        gameObject.SetActive(true);
        //do anim style
    }

    protected override void _AfterUpdate()
    {
        ptr.SetPositionAndRotation(startPtr.position, startPtr.rotation);
        float progress = Progress;
        ptr.RotateAround(transform.position, Vector3.back, progress * 360f);
        fillImage.fillAmount = progress;
    }

    public void Stop()
    {
        isCd = false;
        gameObject.SetActive(false);
        TimeOut = Stop;     //reset events
    }

}
