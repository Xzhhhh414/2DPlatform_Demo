Shader "Custom/EffectTonemapping"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _PostExposure ("Post Exposure", Float) = 1.0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _PostExposure;

            // Tonemapping functions
            float3 T3NeutralCurve(float3 x, float T3_a, float T3_b, float T3_c, float T3_d, float T3_e, float T3_f)
            {
                return ((x * (T3_a * x + T3_c * T3_b) + T3_d * T3_e) / (x * (T3_a * x + T3_b) + T3_d * T3_f)) - T3_e / T3_f;
            }

            float3 TonemapT3ACES(float3 x)
            {
                float T3_a = 0.588;
                float T3_b = 0.34;
                float T3_c = 0.261;
                float T3_d = 0.462;
                float T3_e = 0.1;
                float T3_f = 0.55;
                float T3_whiteLevel = 2.3;
                float T3_whiteClip = 1.0;

                float3 whiteScale = (1.0).xxx / T3NeutralCurve(T3_whiteLevel, T3_a, T3_b, T3_c, T3_d, T3_e, T3_f);
                x = T3NeutralCurve(x * whiteScale, T3_a, T3_b, T3_c, T3_d, T3_e, T3_f);
                x *= whiteScale;
                x /= T3_whiteClip.xxx;

                return saturate(x);
            }


            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                //color.rgb = saturate(pow(color.rgb,2.2));

                // Apply tonemapping
                color.rgb = TonemapT3ACES(color.rgb);
                color.rgb = pow(color.rgb,1/2.2); 
                
                return color;
            }

            ENDCG
        }
    }
}