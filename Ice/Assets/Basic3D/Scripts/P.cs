using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 全局单例类，包括静态全局变量和静态函数。
/// </summary>
[DefaultExecutionOrder(-200)]
public class P : MonoBehaviour
{
    public enum FramePerSecond
    {
        fantastic = 60,
        good = 45,
        based = 30,
    }

    public static P PUBLIC;

    public bool displayFps = true;
    public Text fpsText;

    [SerializeField] private bool openStartEffect = true;
    public float startEffectDuration = 2.5f;
    private string currentScene;
    public FramePerSecond fps = FramePerSecond.good;

    // Use this for initialization
    void Awake()
    {
        PUBLIC = this;

        Application.targetFrameRate = (int)fps;
        currentScene = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        if (displayFps)
        {
            fpsText = GameObject.Find("Canvas/DebugText").GetComponent<Text>();
            StartCoroutine(DisPlayFPS());
        }

        if (openStartEffect)
        {
            ImageEffects.PUBLIC.PlayEffect(ImageEffects.EffectType.Brighten, startEffectDuration);
        }
    }


    public GameObject MakeEffect(GameObject effect, Vector3 pos, float destroyDelay = 1f)
    {
        GameObject newEffect = Instantiate(effect);
        newEffect.transform.position = pos;
        Destroy(newEffect, destroyDelay);
        return newEffect;
    }






    #region AudioPlayer
    private Stack<AudioSource> bufferPool = new Stack<AudioSource>();
    public void PlaySound(AudioClip clip, float vol = 1f)
    {
        if (clip == null) return;
        AudioSource ac;
        if (bufferPool.Count > 0) ac = bufferPool.Pop();
        else
        {
            GameObject obj = new GameObject("sound");
            obj.transform.parent = this.transform;
            ac = obj.AddComponent<AudioSource>();
        }
        ac.clip = clip;
        ac.volume = vol;
        ac.Play();
        StartCoroutine(PushAudioToBuffer(ac));
    }

    private IEnumerator PushAudioToBuffer(AudioSource ac)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (ac.isPlaying == false)
            {
                bufferPool.Push(ac);
                break;
            }
        }
    }
    #endregion

    #region SceneHelper
    IEnumerator DisPlayFPS()
    {
        while (true)
        {
            float dt = 0;
            for (int i = 0; i < 10; i++)
            {
                dt += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            fpsText.text = "FPS:" + (10.0f / dt).ToString(".00");
        }
    }

    private IEnumerator _LoadSceneWithEffect(string name, float dur)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false;

        yield return new WaitForSecondsRealtime(dur);
        async.allowSceneActivation = true;

        //yield return new WaitForSecondsRealtime(dur);
        //SceneManager.LoadSceneAsync(name);
    }



    public void LoadSceneWithEffect(string name, float dur = 1.5f)
    {
        StartCoroutine(_LoadSceneWithEffect(name, dur));
    }

    public void ReLoadCurrentScene(float dur = 1.5f)
    {
        ImageEffects.PUBLIC.PlayEffect(ImageEffects.EffectType.Darken, dur);
        Invoke("_ReLoadCurrentScene", dur);
    }

    private void _ReLoadCurrentScene()
    {
        SceneManager.LoadScene(currentScene);
    }
    #endregion

}
