
Shader "CRLuo/SpritePreview"
{
    Properties
    {
		[NoScaleOffset]
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]
        _NormalMap ("Normal Map", 2D) = "bump" {}
        [NoScaleOffset] 
        _MixTex ("Mix Map", 2D) = "black" {}
        _EffectMap ("Effect Map", 2D) = "black" {}
        _RampThreshold("二值化阈值",Range(0,1)) = 0.5
        _RampSoftness("二值化平滑度",Range(0,1)) = 0.02
        _AmbientColor("Ambient Color", Color) = (0.72, 0.75, 0.77, 1.0)
        _LightColor("Light Color", Color) = (1, 0.98, 0.95, 1.0)

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Cull Off

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3x3 tangentToWorld : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            sampler2D _EffectMap;
            sampler2D _RampTex;
            sampler2D _MixTex;
			float  _X_Sum;
			float	_Y_Sum;
			float	_FrameRate;
            float _RampThreshold;
            float _RampSoftness;
            float _ShadowBrightness;
            float _LightBrightness;
            float4 _AmbientColor;
            float4 _LightColor;
            float _NoNormalMap; // 0 if no normal map, 1 if normal map exists

            uniform float3 _PreviewCharacterLightDir;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float3 normal = float3(0,0,1);
                o.worldNormal = normalize(mul((float3x3)unity_WorldToObject, v.normal));
                float3 tangent = float3(1,0,0);
                float3 binormal = float3(0,1,0);

                // 切线空间到世界空间矩阵
                o.tangentToWorld = float3x3(tangent, binormal, normal);
                return o;
            }

            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
                // Diffuse
                half4 baseColor = tex2D(_MainTex, i.uv);
                half4 mix = tex2D(_MixTex, i.uv);
                // Normal
                float3 normalTex = normalize(pow(tex2D(_NormalMap, i.uv),2.2) * 2.0 - 1.0);
                normalTex.x *= facing;
                float3 worldNormal = mul(i.tangentToWorld,normalTex);
                // Effect
                half4 effectColor = tex2D(_EffectMap, i.uv);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float halfNoL = dot(worldNormal, normalize(-_PreviewCharacterLightDir)) * 0.5 + 0.5;
                half stepNoL = smoothstep(_RampThreshold - _RampSoftness,_RampThreshold + _RampSoftness,halfNoL);
                half3 rampColor;
                if(_NoNormalMap)
                {
                    rampColor = _LightColor.xyz;
                }
                else
                {
                    rampColor = lerp(_AmbientColor.xyz,_LightColor.xyz,stepNoL);
                }


                half4 finalColor = baseColor * fixed4(rampColor, 1.0);
                finalColor.rgb *= finalColor.a;
                finalColor.rgb = lerp(finalColor.rgb,effectColor.rgb,effectColor.a);
                finalColor.a = max(finalColor.a,effectColor.a);

                half3 Emission = mix.g * 5 * baseColor.rgb;
                finalColor.rgb += Emission;

				return float4(finalColor.rgb,finalColor.a);
            }
            ENDCG
        }
    }
}