using UnityEngine;

[RequireComponent(typeof(UpdateBar))]
public class ProgressTimer : Timer
{
    private UpdateBar bar;
    private void Awake()
    {
        bar = GetComponent<UpdateBar>();
    }

    public void StartUp(float duration)
    {
        ColdDownTime = duration;
        ResetCd();
        bar.currSlashValue = 0f;
    }

    protected override void _AfterUpdate()
    {
        bar.PercentValue = Progress;
    }

    public void Stop()
    {
        isCd = false;
        //do effects
    }


}
