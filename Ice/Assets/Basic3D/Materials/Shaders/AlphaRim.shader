// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "xiao_D/Transparent/Alpha Rim" {
	Properties {
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Main Tex", 2D) = "white" {}
		_RimColor("rim color",Color) = (1,1,1,1)	//边缘颜色
        _RimPower ("rim power",range(1,10)) = 2		//边缘强度
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		// Extra pass that renders to depth buffer only
		Pass {
			ZWrite On
			ColorMask 0
		}
		
		Pass {
			Tags { "LightMode"="ForwardBase" }
			
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"
			
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _RimColor;
            float _RimPower;
			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
			};
			
			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				
				fixed4 texColor = tex2D(_MainTex, i.uv);
				
				fixed3 albedo = texColor.rgb * _Color.rgb;
				
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));

				//视角方向  
                float3 ViewDirection = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz); 
				//计算边缘强度  
                half Rim = 1.0 - max(0, dot(worldNormal, ViewDirection));

				half3 Emissive = _RimColor.rgb * pow(Rim,_RimPower); 

				half4 c=half4(ambient + diffuse, texColor.a * _Color.a)+half4(Emissive,_RimColor.a*Rim);
				c.a=clamp(c.a,0,1);
				return c;
			}
			
			ENDCG
		}
	} 
	FallBack "Transparent/VertexLit"
}
