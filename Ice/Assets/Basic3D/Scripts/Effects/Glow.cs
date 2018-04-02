using UnityEngine;

public class Glow : MonoBehaviour
{

    public AnimationCurve curve;
    [Range(0, 1)] public float wave = 0f;
    private Vector3 startLocalScale;

    private void Awake()
    {
        startLocalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.LerpUnclamped(startLocalScale * curve.Evaluate(Time.time), startLocalScale, wave);
    }
}
