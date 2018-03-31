﻿Shader "xiao_D/PostEffect/WaterWave/Single"   
{  
    Properties   
    {  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
    }  
  
    CGINCLUDE  
    #include "UnityCG.cginc"  
    uniform sampler2D _MainTex;  
    float4 _MainTex_TexelSize;  
    uniform float _distanceFactor;  
    uniform float _timeFactor;  
    uniform float _totalFactor;  
    uniform float _waveWidth;  
    uniform float _curWaveDis;  
    uniform float2 _startPos;  
  
    fixed4 frag(v2f_img i) : SV_Target  
    {  
        //DX下纹理坐标反向问题  
        #if UNITY_UV_STARTS_AT_TOP  
        if (_MainTex_TexelSize.y < 0)  
            _startPos.y = 1 - _startPos.y;  
        #endif  

        //计算uv到中间点的向量(向外扩，反过来就是向里缩)  
        float2 dv = _startPos.xy - i.uv;  
        //按照屏幕长宽比进行缩放  
        dv = dv * float2(_ScreenParams.x / _ScreenParams.y, 1);  
        //计算像素点距中点的距离  
        half dis = sqrt(dv.x * dv.x + dv.y * dv.y);  
        //用sin函数计算出波形的偏移值factor  
        //dis在这里都是小于1的，所以我们需要乘以一个比较大的数，比如60，这样就有多个波峰波谷  
        //sin函数是（-1，1）的值域，我们希望偏移值很小，所以这里我们缩小100倍，据说乘法比较快,so...  
        half sinFactor = sin(dis * _distanceFactor + _Time.y * _timeFactor) * _totalFactor * 0.01;  
        //距离当前波纹运动点的距离，如果小于waveWidth才予以保留，否则已经出了波纹范围，factor通过clamp设置为0  
        half discardFactor = clamp(_waveWidth - abs(_curWaveDis - dis), 0, 1) / _waveWidth;  

		dv=clamp(dv,0.01,1);		//过小出问题
        //像素采样时偏移offset  
        half2 uv = normalize(dv)  * sinFactor * discardFactor + i.uv;  
        return tex2D(_MainTex, uv);   
    }  
  
    ENDCG  
  
    SubShader   
    {  
        Pass  
        {  
            ZTest Always  
            Cull Back
            ZWrite Off  
            Fog { Mode off }  
  
            CGPROGRAM  
            #pragma vertex vert_img  
            #pragma fragment frag   
            ENDCG  
        }  
    }  
    Fallback off  
}  