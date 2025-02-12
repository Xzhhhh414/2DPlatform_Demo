#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

TEXTURE2D(_BumpMaskMap);
SAMPLER(sampler_BumpMaskMap);

TEXTURE2D(_MixTexture);
SAMPLER(sampler_MixTexture);

TEXTURE2D(_LowSpecmap);
SAMPLER(sampler_LowSpecmap);

TEXTURE2D(_DissolveNoiseTexture);
SAMPLER(sampler_DissolveNoiseTexture);

TEXTURE2D(_FXSparkMask);
SAMPLER(sampler_FXSparkMask);


CBUFFER_START(UnityPerMaterial)
#include "DisneyInput_Base.hlsl"

uniform half4 _BaseColor;
uniform half4 _MainTex_ST;
uniform half4 _BumpMaskMap_ST;
uniform half4 _MixTexture_ST;
uniform half4 _LowSpecmap_ST;

uniform half4 _SparkleColor;
uniform half4 _InvincibleColor;

uniform half4 _OcclusionColor;

uniform half _OcclusionIntensity;

uniform half _AlphaClipThreshold;
uniform half _EmissionIntensity;

uniform float _SparkleFrequency;
uniform half _InvincibleScale;
uniform half _InvinciblePower;


uniform int _FXSparkInvincible_ON;

uniform half4 _IGInvincibleColor;
uniform half _IGInvincibleScale;
uniform half _IGInvinciblePower;
uniform int _UseCustomDir;
uniform half _LowRough;
uniform half _LowMetal;
uniform half _IngameAlpha;

uniform float _Postype;
uniform float _UpOrDown;
uniform float _EdgeWidth;
uniform float _DissolveValue;
uniform float _DissolvePow;
uniform float4 _DissolveColor;
uniform float4 _DissolveNoiseTexture_ST;

uniform float _UseFXSparkMask;

CBUFFER_END


uniform half4 _GlobalDirectColor;
uniform half4 _GlobalInDirectColor;