using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EffectDisplayer : MonoBehaviour
{
    public static EffectDisplayer PUBLIC;
    private void Awake()
    {
        if (PUBLIC == null) PUBLIC = this;
    }

    private Animator animator;
    public Animator My_Animator {
        get {
            if (animator == null) animator = GetComponent<Animator>();
            return animator;
        }
    }

    private void Play(string animation, float destroyDelay)
    {
        My_Animator.Play(animation);
        Destroy(gameObject, destroyDelay);
    }

    public GameObject CreateNewAndPlay(Vector3 pos, string anim, float destroyDelay = 3f)
    {
        if (anim == "") return null;
        GameObject obj = Instantiate(gameObject);
        obj.transform.position = pos;
        obj.GetComponent<EffectDisplayer>().Play(anim, destroyDelay);
        return obj;
    }

}
