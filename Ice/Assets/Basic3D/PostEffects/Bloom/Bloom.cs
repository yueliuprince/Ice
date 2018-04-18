using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Bloom : PostEffectBase
{
    // Blur iterations - larger number means more blur.
    [Range(0, 8)]
    public int iterations = 3;

    // Blur spread for each iteration - larger value means more blur
    [Range(0.2f, 3.0f)]
    public float blurSpread = 0.6f;

    [Range(1, 8)]
    public int downSample = 2;

    [Range(0.0f, 4.0f)]
    public float luminanceThreshold = 0.6f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_Material != null)
        {
            _Material.SetFloat("_LuminanceThreshold", luminanceThreshold);

            int rtW = src.width / downSample;
            int rtH = src.height / downSample;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(src, buffer0, _Material, 0);

            for (int i = 0; i < iterations; i++)
            {
                _Material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // Render the vertical pass
                Graphics.Blit(buffer0, buffer1, _Material, 1);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // Render the horizontal pass
                Graphics.Blit(buffer0, buffer1, _Material, 2);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }

            _Material.SetTexture("_Bloom", buffer0);
            Graphics.Blit(src, dest, _Material, 3);

            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
