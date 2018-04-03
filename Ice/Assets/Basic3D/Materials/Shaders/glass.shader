//Enable only per-pixel lights(That means they are important)
//Write the code in [ForwardBase pass] to dispose the extra 4 per-vertex and SH light 
//[Phong] light model

Shader "xiao_D/glass" {
    Properties {
		_Diffuse("Diffuse", Color) = (0.6,0.6,0.6,1)
		_Glass("Glass", Range(0, 1)) = 0.974
        _Glossy ("Glossy", Range(4, 256)) = 130
		_Specular("Specular", Range(0, 1)) = 0.5
        _CubeMap ("CubeMap", Cube) = "_Skybox" {}       
		_Ratio("Ratio", Range(0, 1)) = 0.633
		_Alpha("Alpha", Range(0, 1)) = 1
		_FresnelExp("FresnelExp",Range(0,8)) = 5.5
		_Emission("Emission",Color)=(0.2,0.2,0.2,1)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200

		
		//Extra pass that renders to depth buffer only
		Pass {
			ZWrite On
			ColorMask 0
		}

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
			#include "AutoLight.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 3.0


            uniform float _Glossy;
            uniform samplerCUBE _CubeMap;
            uniform float4 _Diffuse;
            uniform float _Specular;
            
            uniform float _Glass;          
            uniform float _Ratio;
			uniform float _FresnelExp;
            uniform float _Alpha;
			uniform float4 _Emission;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
				float3 worldPos:TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
				float3 viewDir: TEXCOORD2;
				float3 viewReflectDir:TEXCOORD3;
				float3 viewRefractDir:TEXCOORD4;
            };

            VertexOutput vert (VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);

				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);

				o.viewReflectDir = reflect(-o.viewDir, o.worldNormal);
				o.viewRefractDir = refract(-o.viewDir, o.worldNormal, _Ratio);

                return o;
            }


			half4 frag(VertexOutput i) : SV_Target {
				i.worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);	//the vector is from point to light

				half3 refraction = texCUBE(_CubeMap, i.viewRefractDir).rgb;
				half3 reflection = texCUBE(_CubeMap, i.viewReflectDir).rgb;

				half fresnel = pow(1 - max(0, dot(i.viewDir, i.worldNormal)), _FresnelExp);

				half3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(lightDir, i.worldNormal));

				float3 specularDir = reflect(-lightDir, i.worldNormal);
				half3 specular = _LightColor0.rgb * _Specular * pow(max(0, dot(i.viewDir,specularDir)), _Glossy);

				half3 glass = lerp(refraction,reflection, fresnel*0.5);		//more refraction
				half finalAlpha = _Alpha*(1-_Glass*(0.5-fresnel));

				half3 finalColor = lerp(diffuse, glass, _Glass) + specular + UNITY_LIGHTMODEL_AMBIENT.rgb;
                return fixed4(finalColor+_Emission, saturate(finalAlpha));
            }
            ENDCG
        }

		Pass {
			Name "FORWARD_ADD"
			Tags {
                "LightMode"="ForwardAdd"
            }

			Blend One One
            ZWrite Off

			CGPROGRAM
			#pragma multi_compile_fwdadd	
			#pragma vertex vert
			#pragma fragment frag		
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			uniform float _Glossy;
            uniform float4 _Diffuse;
            uniform float _Specular;
     
            uniform float _Glass;          
            uniform float _Alpha;
			//uniform float _FresnelExp;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);			
				o.worldNormal = UnityObjectToWorldNormal(v.normal);			
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				i.worldNormal = normalize(i.worldNormal);

				#ifdef USING_DIRECTIONAL_LIGHT
					half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				#else
					half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
				#endif
			

				half3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(i.worldNormal, lightDir));			
				float3 specularDir = reflect(-lightDir, i.worldNormal);
				half3 specular = _LightColor0.rgb * _Specular * pow(max(0, dot(i.viewDir,specularDir)), _Glossy);
				
				#ifdef USING_DIRECTIONAL_LIGHT
					half atten = 1.0;
				#else
					#if defined (POINT)
				        float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
				        half atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
				    #elif defined (SPOT)
				        float4 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1));
				        half atten = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w * tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
				    #else
				        half atten = 1.0;
				    #endif
				#endif

				return fixed4((diffuse*(1-_Glass) + specular) * atten*_Alpha,1);
			}
			ENDCG
		}
    }
    FallBack "Diffuse"
}
