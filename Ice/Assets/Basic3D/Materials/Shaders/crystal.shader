Shader "xiao_D/Transparent/crystal" {
    Properties {
        _Diffuse ("Diffuse", Color) = (0.5735294,0.5735294,0.5735294,1)
        _Emission ("Emission", Color) = (0.8233265,0.8088235,1,1)
		_FresnelExp("FresnelExp",Range(0,8)) = 5.5
        _Alpha_add ("alpha_add", Range(0, 1)) = 0.2611585
    }
    SubShader {
        Tags {
            "IgnoreProjector"="False"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

		//Extra pass that renders to depth buffer only
		Pass {
			ZWrite On
			ColorMask 0
		}

		LOD 400

        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Diffuse;
            uniform float4 _Emission;
            uniform float _Alpha_add;
			uniform float _FresnelExp;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
				float3 viewDir: TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);

				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);
                return o;
            }
            fixed4 frag(VertexOutput i) : SV_Target {
                i.worldNormal = normalize(i.worldNormal);

				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);	//the vector is from point to light
				half fresnel = pow(1 - max(0, dot(i.viewDir, i.worldNormal)), _FresnelExp);

				half3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(lightDir, i.worldNormal));

				half alpha=fresnel + _Alpha_add;
				half4 finalColor = half4(diffuse,fresnel) + half4(_Emission.rgb*alpha,0);
				finalColor.a=clamp(finalColor.a,0,1);

				return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
