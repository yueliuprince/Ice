using UnityEngine;

public class Glow : MonoBehaviour
{

    public AnimationCurve curve;
    [Range(0, 1)] public float wave = 0f;
    private Material mat;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float scale = Mathf.LerpUnclamped(curve.Evaluate(Time.time), 1, wave);
        mat.SetFloat("_VertexScale", scale);
    }
}
