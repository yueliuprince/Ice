using System.Collections;
using UnityEngine;
using DG.Tweening;
using Basic3D;

public sealed class DoorCtrl : Signal
{
    private Transform doorL, doorR;
    private Vector3 closedPos_L, closedPos_R;
    private AudioSource m_AudioSrc;
    public bool isOpen = false;

    [SerializeField] private Vector3 openVectorLeft = Vector3.zero;
    public float duration = 2f;
    public Ease ease = Ease.InExpo;


    private void Awake() {
        m_AudioSrc = GetComponent<AudioSource>();
        doorL = transform.Find("doorL");
        doorR = transform.Find("doorR");
        closedPos_L = doorL.position;
        closedPos_R = doorR.position;
        base.signalName = GetType().ToString();
    }

    //调用此函数来开关门
    private void CtrlTheDoor() {
        doorL.DOKill();
        doorR.DOKill();

        if (!isOpen) {
            doorL.DOMove(closedPos_L + openVectorLeft, duration).SetEase<Tween>(ease);
            doorR.DOMove(closedPos_R - openVectorLeft, duration).SetEase<Tween>(ease);

            m_AudioSrc.Play();
        }
        else {
            doorL.DOMove(closedPos_L, duration).SetEase<Tween>(ease);
            doorR.DOMove(closedPos_R, duration).SetEase<Tween>(ease);
        }
        isOpen = !isOpen;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            collision.GetComponent<PlayerControl>().outSignal = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            collision.GetComponent<PlayerControl>().outSignal = null;
        }
    }

    IEnumerator GotoNextScene() {
        yield return new WaitForSeconds(duration);
        if (isOpen) P.PUBLIC.LoadSceneWithEffect(transform.name);
    }

    public override void OnTrigger() {
        CtrlTheDoor();
        StartCoroutine("GotoNextScene");
    }

}
