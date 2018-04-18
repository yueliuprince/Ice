using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager PUBLIC;
    private void Awake()
    {
        PUBLIC = this;
    }

    public List<SoundGroup> groups = new List<SoundGroup>();
    public AudioClip placeEffect;

    [System.Serializable]
    public struct SoundGroup
    {
        public List<AudioClip> clips;
        public AudioClip RandomClip()
        {
            return clips[Random.Range(0, clips.Count - 1)];
        }
    }

}
