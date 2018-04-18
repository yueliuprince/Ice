Shader "xiao_D/2D/sp_Anti-Aliasing"
{
	Properties
	{
		[PerRendererData]_MainTex ("Texture", 2D) = "white" {}
		_AlphaThreshold("Alpha Threshold",Range(0,1))=0.64
		_RendererColor("_Color", Color) = (1,1,1,1)
	}
	SubShader
	{
        Cull Off
        Lighting Off
        ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Tags{
			"Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
			"CanUseSpriteAtlas"="True"
			"PreviewType"="Plane"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
					
			#include "UnityCG.cginc"

			float _AlphaThreshold;
			half2 _MainTex_TexelSize;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			half4 _RendererColor;

			half4 frag (v2f i) : SV_Target
			{		
				half4 col = tex2D(_MainTex, i.uv);

				if(col.a>_AlphaThreshold) return col;
				
				half2 texel=_MainTex_TexelSize;
				col+=tex2D(_MainTex,half2(i.uv.x-texel.x,i.uv.y+texel.y));			//左上
				col+=tex2D(_MainTex,half2(i.uv.x,i.uv.y+texel.y));					//中上
				col+=tex2D(_MainTex,half2(i.uv.x+texel.x,i.uv.y+texel.y));			//右上

				col+=tex2D(_MainTex,half2(i.uv.x-texel.x,i.uv.y));					//左一
				col+=tex2D(_MainTex,half2(i.uv.x+texel.x,i.uv.y));					//右一

				col+=tex2D(_MainTex,half2(i.uv.x-texel.x,i.uv.y-texel.y));			//左下
				col+=tex2D(_MainTex,half2(i.uv.x,i.uv.y-texel.y));					//中下
				col+=tex2D(_MainTex,half2(i.uv.x+texel.x,i.uv.y-texel.y));			//右下

				col/=9;
				return col*_RendererColor;
			}
			ENDCG
		}
	}
	FallBack off
}
