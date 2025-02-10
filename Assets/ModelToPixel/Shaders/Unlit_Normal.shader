Shader "Unlit/Unlit_Normal"
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.33
        _BumpMaskMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity("Normal Intensity", Range(0,1)) = 0
    }
    SubShader
    {
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3x3 tangentToLocal : TEXCOORD2;
                float3 localNormal : TEXCOORD8;
            };

            sampler2D _BumpMaskMap;
            float4 _BumpMaskMap_ST;
            sampler2D _MainTex;

            half _Cutoff;
            half _NormalIntensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BumpMaskMap);
                o.localNormal = v.normal;

                // 创建切线空间到本地空间矩阵
                float3 normal = normalize(v.normal);
                float3 tangent = normalize(v.tangent.xyz);
                float3 binormal = cross(normal, tangent) * v.tangent.w;

                // 切线空间到本地空间矩阵
                o.tangentToLocal = float3x3(tangent, binormal, normal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 normalMask_var = tex2D(_BumpMaskMap, i.uv);
                half2 normal_xy = UnpackNormal(normalMask_var).xy;
                half3 NormalTS = half3(normal_xy * _NormalIntensity, sqrt(1.0 - saturate(dot(normal_xy, normal_xy))));
                float3 localNormal = normalize(mul(NormalTS,i.tangentToLocal));

                float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, localNormal)) * 0.5 + 0.5;
                viewNormal.z = 1;

                half4 col = tex2D(_MainTex, i.uv);
                //clip(col.a - _Cutoff);
                
                return fixed4(viewNormal, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}