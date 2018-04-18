using UnityEngine;
using System.Collections;

public class BrightnessSaturationAndContrast : PostEffectBase {
	[Range(0.0f, 3.0f)]
	public float brightness = 1.0f;

	[Range(0.0f, 3.0f)]
	public float saturation = 1.0f;

	[Range(0.0f, 3.0f)]
	public float contrast = 1.0f;

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (_Material != null) {
			_Material.SetFloat("_Brightness", brightness);
			_Material.SetFloat("_Saturation", saturation);
			_Material.SetFloat("_Contrast", contrast);

			Graphics.Blit(src, dest, _Material);
		} else {
			Graphics.Blit(src, dest);
		}
	}
}
