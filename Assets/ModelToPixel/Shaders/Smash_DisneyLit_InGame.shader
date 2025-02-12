Shader "Smash/SH_DisneyLit_InGame_Simplified"
{
    Properties
    {
        [HideInInspector] _SurfaceType("__SurfaceType", Float) = 0
        [HideInInspector][Enum(UnityEngine.Rendering.CullMode)][HideInInspector] _RenderFace("__RenderFace", Float) = 2
        [HideInInspector][Enum(Off, 0, On, 1)] _ZWrite("__ZWrite", Float) = 1
        [HideInInspector] _BlendType("__BlendType", Float) = 0
        [HideInInspector] _AlphaClipThreshold("__AlphaClip", Range(0 , 1)) = 0
        [HideInInspector] _RenderOrder("__RenderOrder", Range(-50,50)) = 0
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("__SrcBlend", Float) = 1
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("__DstBlend", Float) = 0

        [Header(ALBEDO)]
        _MainTex("Albedo Map", 2D) = "white" {}
        [hdr] _BaseColor("Color", Color) = (1,1,1,0)

        [Header(NORMAL And MASK)]
        _BumpMaskMap("Normal(RG) Sheen(B) Clearcoat(A)", 2D) = "bump" {}

        [Header(PBR MISC)]
        _MixTexture("PBR MAP：R:Metal G:Rough B:AO A:Emission", 2D) = "gray" {}
        _OcclusionIntensity("AO Intensity", Range(0 , 1)) = 0
        _OcclusionColor("AO Color",Color) = (0, 0, 0, 0)
        _EmissionIntensity("Emission Intensity", Range(0,10)) = 0
        _IngameAlpha("IngameAlpha", Range(0,1)) = 1

        [Header(SHEEN)]
        _Sheen("Sheen(Cloth)", Range(0 , 1)) = 0
        _SheenTint("Sheen weight", Range(0 , 1)) = 0.5

        [Header(CUSTOM)]
        _DirectLightIntensity("Direct Light Intensity", Range(0,3)) = 1
        _GIIntensity("GI Intensity", Range(0,3)) = 1
        _ShadowIntensity("Shadow Intensity", Range(0 , 5)) = 1
        _CustomShadowBias("Custom Shadow Bias", Vector) = (0,0,0,0)
        _GIfresnel("GI Fresnel", Range(0,1)) = 1
        _EnableHightLightNormal("Enable HightLight & Normal Enhance", int) = 0
        _CustomLightSpecIntensity("HightLight Intensity", float) = 0
        _LightmapNormEnhance("Normal Enhance", Range(0,2)) = 0
        _LightmapNormEnhanceBrighter("Normal Enhance Lerp", Range(0,2)) = 0

        [Header(FX)]
        [Header(FX_ SPARKLE AND INVICIBLE)]
        _FXSparkInvincible_ON("Enable Sparkle and Invincible", int) = 1
        _FXSparkMask("FXSpark Mask", 2D) = "white" {}
        [Toggle] _UseFXSparkMask("Use FXSparkMask", float) = 0.0
        [HDR] _SparkleColor("Spark FX", Color) = (0,0,0,0)
        _SparkleFrequency("Spark Frequency", Float) = 10
        [HDR] _InvincibleColor("Invincible Color", Color) = (0,0,0,0)
        _InvincibleScale("Invincible Scale", Range(0 , 6)) = 4.5
        _InvinciblePower("Invincible Power", Range(0 , 5)) = 2.34

        [Header(Camp)]
        _IGInvincibleColor("IGInvincible Color", Color) = (0,0,0,0)
        _IGInvincibleScale("IGInvincible Scale", Range(0 , 6)) = 4.5
        _IGInvinciblePower("IGInvincible Power", Range(0 , 5)) = 2.34
        [HideInInspector] _UseCustomDir("_UseCustomDir", int) = 0

        [Header(Dissolve)]
        [Toggle(_DISSOLVE_INGAME_ON)] _Dissolve_InGame_ON("Dissolve", float) = 0
        [HideInInspector] _Postype("_Postype", Float) = 0
        _UpOrDown("UpOrDown", Range(0,1)) = 0
        _DissolveNoiseTexture("Dissolve Noise Texture", 2D) = "white" {}
        _EdgeWidth("EdgeWidth", Range(0,1)) = 0
        _DissolveValue("Dissolve Value", float) = 0
        _DissolvePow("_DissolvePow", float) = 1
        [HDR] _DissolveColor("Dissolve Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _MixTexture;
            float4 _MainTex_ST;
            half _EmissionIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Sample the base color
                half4 col = tex2D(_MainTex, i.texcoord);

                // Sample the emission mask from the alpha channel of _MixTexture
                half emissionMask = tex2D(_MixTexture, i.texcoord).a;

                // Calculate emission color based on intensity and mask
                half3 Emission = col.xyz * max(0, pow(2, _EmissionIntensity) - 1) * emissionMask;

                // Output the color (base color + emission)
                return half4(col.rgb + Emission, col.a);
            }
            ENDCG
        }
    }
}