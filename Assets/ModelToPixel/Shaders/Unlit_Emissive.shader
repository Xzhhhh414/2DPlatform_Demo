Shader "Unlit/Unlit_Emissive"
{
    Properties
    {
        _MixTexture("PBR MAP：R:Metal G:Ro, B:Sheen A:Clearcoat", 2D) = "black" {}
        _EmissionIntensity("Emission Intensity",Range(0,10)) = 0
        // 其他属性...
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MixTexture; // 使用 MixTexture
            float _EmissionIntensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 获取混合图
                fixed4 mixColor = tex2D(_MixTexture, i.uv);
                float intensity = _EmissionIntensity > 0.1 ? 1.0 : 0.0;
                intensity *= _EmissionIntensity * 0.2;
                float emissive = intensity * mixColor.a;

                // 输出自发光的 alpha
                return fixed4(emissive, emissive, emissive, 1); // 将 alpha 通道用于 mask
            }
            ENDCG
        }
    }
}