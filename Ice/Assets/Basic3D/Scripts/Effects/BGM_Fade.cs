using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class BGM_Fade : MonoBehaviour
{
    public float fadeTime = 2f;
    private AudioSource ac;
    [Range(0, 1)] public float vol = 1f;
    public bool loop = true;
    public float interval = 1f;

    private void Awake()
    {
        ac = GetComponent<AudioSource>();
        PlayWithFade();

    }

    private void PlayWithFade()
    {
        if (loop) Invoke("PlayWithFade", ac.clip.length / ac.pitch + interval);
        ac.volume = 0;
        DOTween.To(() => ac.volume, x => ac.volume = x, vol, fadeTime).SetEase(Ease.InQuad);
        ac.Play();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}
