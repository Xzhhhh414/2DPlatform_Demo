#include "CustomLighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "DisneyLighting.hlsl"
#include "Fog/HeightFogUE4Function.hlsl"

struct VertexInput
{
	float4 vertex : POSITION;
	half3 normal : NORMAL;
	half4 tangent : TANGENT;
	float4 uv : TEXCOORD0;
	half4 uv2 : TEXCOORD1;
	half4 uv3:TEXCOORD2;
	half4 color :COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput
{
	float4 clipPos : SV_POSITION;
	half4 lightmapUVOrVertexSH : TEXCOORD0;
	half4 fogFactorAndVertexLight : TEXCOORD1;
	float4 shadowCoord : TEXCOORD2;
	half4 tSpace0 : TEXCOORD3;
	half4 tSpace1 : TEXCOORD4;
	half4 tSpace2 : TEXCOORD5;
	float4 posOS : TEXCOORD6;
	float4 uv : TEXCOORD7;
	float4 fogColorAndOpacity : TEXCOORD9;
	float tangentw : TEXCOORD10;

	#if defined(_USE_SPARKLE)
	float4 randomColor : TEXCOORD11;
	#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};


VertexOutput vert(VertexInput v)
{
	VertexOutput o = (VertexOutput)0;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	half3 lwWNormal = TransformObjectToWorldNormal(v.normal);
	float3 lwWorldPos = TransformObjectToWorld(v.vertex.xyz);
	half3 lwWTangent = TransformObjectToWorldDir(v.tangent.xyz);
	half3 lwWBinormal = Unity_SafeNormalize(cross(lwWNormal, lwWTangent) * v.tangent.w * GetOddNegativeScale());

	float3 positionWS = TransformObjectToWorld(v.vertex.xyz);

	o.clipPos = TransformWorldToHClip(positionWS);

	#ifdef _SMASH_DISNEY_TOON
		if(_Outline > 0.0001)
		{
			half2 offset = TransformWorldToHClipDir(lwWNormal).xy;
			half outlineWidth = _Outline * lerp(1, v.color.a, _OutlineWidthA);
			o.clipPos.xy += offset * o.clipPos.w * outlineWidth * 0.003;
		}
	#endif

	OUTPUT_LIGHTMAP_UV(v.uv2, unity_LightmapST, o.lightmapUVOrVertexSH.xy);
	OUTPUT_SH(lwWNormal, o.lightmapUVOrVertexSH.xyz);
	
	#ifndef USE_UNIQUE_SHADOW
		#ifdef _MAIN_LIGHT_SHADOWS
		o.shadowCoord = TransformWorldToShadowCoord(positionWS);
		#endif
	#else
		//X2M_UNIQUE_TRANSFER_SHADOW(o, positionWS);
		o.shadowCoord = mul(u_UniqueShadowMatrix, float4(positionWS.xyz, 1.f));
	#endif

	o.tSpace0 = half4(lwWTangent.x, lwWBinormal.x, lwWNormal.x, lwWorldPos.x);
	o.tSpace1 = half4(lwWTangent.y, lwWBinormal.y, lwWNormal.y, lwWorldPos.y);
	o.tSpace2 = half4(lwWTangent.z, lwWBinormal.z, lwWNormal.z, lwWorldPos.z);
	o.posOS = v.vertex;
	o.uv.xy = v.uv.xy; o.uv.zw = v.uv2.xy;
	o.tangentw =  v.tangent.w;

	half fogFactor = 0;
	#ifdef _SMASH_DISNEY_USEADVFOG
		// #ifndef _ADVANCE_UE4HEIGHTFOG_PIXEL
		if(_FogType == 1)
		{
			Light mainLight = GetMainLight();
			half3 mainLightDir = _CustomLight == 1 ? _CustomLightDir.xyz : mainLight.direction;
			o.fogColorAndOpacity = ComputeHeightFogColor(positionWS, float4(mainLightDir, 1));
		}
		// #endif
	#endif

	#if defined(_USE_SPARKLE)

		o.randomColor.xyz = v.vertex.xyz;

	#endif

	return o;
}


void InitMetalRoughAOEmiss(float2 uv, half3 Albedo, out half Metallic, out half Roughness, out half AO, out float3 Emission) {
	half4 pbrMap_var = SAMPLE_TEXTURE2D(_MixTexture, sampler_MixTexture, uv * _MainTex_ST/*_MixTexture_ST*/.xy + _MainTex_ST/*_MixTexture_ST*/.zw);

	Metallic = pbrMap_var.r;
	Roughness = pbrMap_var.g;
	AO = pbrMap_var.b;
	Emission = Albedo * pbrMap_var.a * max(0, pow(2, _EmissionIntensity) - 1);
#ifdef _SMASH_EMISSIVENOISE
	float4 emNoise = SAMPLE_TEXTURE2D(_EmissionNoiseTex, sampler_EmissionNoiseTex, uv * _EmissionNoiseTex_ST.xy + _EmissionNoiseTex_ST.zw * _Time.y);
#ifdef _ADDBLOOM
	Emission *= saturate(emNoise.r)* lerp(_EmColor.xyz,_GamaEmColor.xyz,_AddBloom);
#else
	Emission *= saturate(emNoise.r) * _EmColor.xyz;
#endif
#endif
	
}

//if normal map is no SafeNormalize, when x^2+y^2>1, then give default value(0,0,1)
void InitNormalMask(half2 uv, out half3 NormalTS, out half SheenMask, out half ClearCoatMask) {
	half4 normalMask_var = SAMPLE_TEXTURE2D(_BumpMaskMap, sampler_BumpMaskMap, uv * _MainTex_ST.xy + _MainTex_ST.zw);
	half2 normal_xy = normalMask_var.rg * 2 - 1;
	
	// half z2 = max(0.99 - dot(normal_xy, normal_xy), 0);
	// NormalTS = lerp(half3(0, 0, 1), half3(normal_xy, sqrt(z2)), ceil(z2));

	// NormalTS = z2 > 1 ? half3(0, 0, 1) : half3(normal_xy, sqrt(z2));

	NormalTS = half3(normal_xy, sqrt(1.0 - saturate(dot(normal_xy,normal_xy))));
	SheenMask = normalMask_var.b;
	ClearCoatMask = normalMask_var.a;
}

void InitSubsurface(half2 uv, half3 Albedo, out SubsurfaceData sssData) {
	sssData = (SubsurfaceData)0;
	#ifdef _SMASH_DISNEY_SUBSURFACE_ON
		half4 subtranslucency_var = SAMPLE_TEXTURE2D(_SubsurfaceMap, sampler_SubsurfaceMap, uv * _MainTex_ST.xy + _MainTex_ST.zw);
		sssData.translucency = subtranslucency_var.r * _TransStrength;
		_ShadowIntensity *= subtranslucency_var.a;
	#ifdef _SMASH_DDLOBBY_JUNE
	if(_Lobby_june == 1)
	{
		sssData.translucency *= 0.65;
	}
	#endif
		sssData.transPower = _TransPower;
		sssData.color = _SubsurfaceCol.rgb * Albedo;
	#else
		sssData.translucency = 1;
		sssData.color = 1;
	#endif
}

void InitAnistropic(half2 uv, half3 normalWS, half3 WorldSpaceTangent, half3 WorldSpaceBiTangent, out AnistroData anistroData, half tangentw) {
	anistroData = (AnistroData)0;
	#ifdef _SMASH_DISNEY_ANISOTROPIC_ON 
		#ifdef _SMASH_DISNEY_USEKAJIYAANISTROPIC
			half4 flowSample = SAMPLE_TEXTURE2D(_FlowMap, sampler_FlowMap, uv.xy);
			half4 kajiyaNoise = SAMPLE_TEXTURE2D(_KajiyaNoise, sampler_KajiyaNoise, uv.xy * _KajiyaNoise_ST.xy + _KajiyaNoise_ST.zw);
			half3 flowDir = 2 * flowSample.xyz -1;
			flowDir = flowDir.x * WorldSpaceTangent +
			flowDir.y * WorldSpaceBiTangent +
			flowDir.z * normalWS;
			flowDir = normalize(flowDir);
			half specularShift;
			half specularShift2;
			specularShift = kajiyaNoise.r *_SpecularShift + _SpecularPos ;
			specularShift2 = kajiyaNoise.r * _SpecularShift2 + _SpecularPos2;
			#ifdef _SMASH_NO_FLOWDIR
				flowDir = Unity_SafeNormalize(WorldSpaceBiTangent);
				float shifttex = lerp((0.5 - _Jitter * 0.5), (_Jitter * 0.5 + 0.5),
					kajiyaNoise.b);
				specularShift = shifttex + _HighSpecularShift;
				specularShift2 = shifttex + _HighSpecularShift2;
			#endif		
			anistroData.T1 = ShiftTangentX(flowDir, normalWS, specularShift);
			anistroData.T2 = ShiftTangentX(flowDir, normalWS, specularShift2);
			anistroData.perceptualRoughness = 1-_Smoothness;
		    anistroData.perceptualRoughness2 = 1-_Smoothness2;
			anistroData.anisotropicColor = _AnisotropicColor.xyz;
			anistroData.anisotropicColor2 = _AnisotropicColor2.xyz;
			anistroData.anisotropicIntensity = _AnisotropicIntensity;
			anistroData.anisotropicMask = flowSample.w;
		#endif
	#endif
}

void InitClearcoat(half3 WorldSpaceNormal, half ClearcoatMask, out ClearcoatData clearcoatData) {
	clearcoatData = (ClearcoatData)0;
	#ifdef _SMASH_DISNEY_CLEARCOAT_ON
		clearcoatData = (ClearcoatData)0;
		clearcoatData.normal = WorldSpaceNormal;
		clearcoatData.clearcoat = _Clearcoat * ClearcoatMask;
		clearcoatData.clearcoatGloss = _ClearcoatGloss;
	#endif
}

//Detail normal_metal_roughness map
void InitDetailNormalMetalRough(half2 uv, half SheenMask, inout half3 Normal, inout half Metallic, inout half Roughness) {
	#ifdef _DetailBump_Metallic_Roughness_ON
		half4 detail_nbrVar = SAMPLE_TEXTURE2D(_DetailBMRMap, sampler_DetailBMRMap, uv * _DetailBMRMap_ST.xy + _DetailBMRMap_ST.zw);
		half2 detail_nxy = detail_nbrVar.rg * 2.0 - 1.0;
		half3 detail_normal = half3(detail_nxy, sqrt(1 - dot(detail_nxy, detail_nxy)));
		Normal = BlendNormalRNM(Normal, lerp(half3(0, 0, 1), detail_normal, _DetailNormalIntensity * SheenMask));
		Metallic = saturate(overlayLerpBlend(Metallic, detail_nbrVar.b, _DetailMetallicIntensity * SheenMask));
		Roughness = saturate(overlayLerpBlend(Roughness, detail_nbrVar.a, _DetailRoughnessIntensity * SheenMask));
	#endif
}

half3 NormalTSToWS(VertexOutput input, half3 Normal, half3 WorldSpaceNormal, out half3 WorldSpaceTangent, out half3 WorldSpaceBiTangent) {
	WorldSpaceTangent = half3(input.tSpace0.x, input.tSpace1.x, input.tSpace2.x);
	WorldSpaceBiTangent = half3(input.tSpace0.y, input.tSpace1.y, input.tSpace2.y);
	return Unity_SafeNormalize(TransformTangentToWorld(Normal, half3x3(WorldSpaceTangent, WorldSpaceBiTangent, WorldSpaceNormal)));
}

void FX_Invincible(half NdotV, inout half3 Emission,float maskValue = 1.0) {
	#ifdef _SMASH_DISNEY_FX_ON
		if (_FXSparkInvincible_ON == 1) {
			float sinValue = sin(fmod(_Time.y,10) * _SparkleFrequency ) * 0.5 + 0.5 ;
			//sinValue = saturate(sinValue);

			Emission += (sinValue * _SparkleColor.rgb +
			_InvincibleColor.rgb * _InvincibleScale * pow(max(0.01,saturate(1.0 - NdotV)), _InvinciblePower) * maskValue).rgb ;
			// Emission += (saturate(sin(fmod(_Time.y, 1.0) * _SparkleFrequency) * 0.5) * _SparkleColor.rgb +
			// _InvincibleColor.rgb * _InvincibleScale * pow(max(0.01,saturate(1.0 - NdotV)), _InvinciblePower)).rgb;
		}
	#endif
}

void Lobby_Adv(half NdotL, half NdotV, half3 Albedo, half FMask, inout half3 Emission) {
	#ifdef _SMASH_DISNEY_LOBBYADV
		// #ifndef _SMASH_DDLOBBY_JUNE
	if(_Lobby_june != 1)
	{
		if (_Lobby_on == 1) {
			Emission +=  FMask * lerp(half3(1.0h,1.0h,1.0h),Albedo,_AlbedeoIntensity) * NdotL * _FresnelColor.xyz * _FresnelScale * smoothstep(_FresnelLow,_FresnelHigh,pow(max(0.01,saturate(1.0 - NdotV)), _FresnelPower));
		}
	}
		// #endif
	#endif
}

void IngameCamp(half NdotV,inout half3 Emission)
{
	#ifdef _SMASH_DISNEY_IGCAMP
		Emission += _IGInvincibleColor.rgb * _IGInvincibleScale * pow(max(0.01,saturate(1.0 - NdotV)), _IGInvinciblePower);
	#endif
}


float3 RotateAboutAxis(float3 In, float3 Axis, float Rotation)
{
	float s = sin(Rotation);
	float c = cos(Rotation);
	float one_minus_c = 1.0 - c;

	Axis = normalize(Axis);
	float3x3 rot_mat = 
	{   one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s, one_minus_c * Axis.z * Axis.x + Axis.y * s,
		one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c, one_minus_c * Axis.y * Axis.z - Axis.x * s,
		one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s, one_minus_c * Axis.z * Axis.z + c
	};
	return mul(rot_mat,  In);
}


#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP
	void InitCustomlightmap(out CustomlightmapData customlightmapData){
		customlightmapData = (CustomlightmapData)0;
		customlightmapData.lightmapFactor = _LightmapFactor;
		customlightmapData.lightmaplerp = _LightmapLerp;
		customlightmapData.lightmaptransLow = _LightmaptransLow;
		customlightmapData.lightmaptransHigh = _LightmaptransHigh;
		customlightmapData.lightmapChrominance = _LightmapChrominance;
		customlightmapData.lightmapChroma = _LightmapChroma;
		customlightmapData.lightmapshadowcolorup = _Lightmapshadowcolorup;
		customlightmapData.lightmapshadowcolorleft = _Lightmapshadowcolorleft;
		customlightmapData.lightmapshadowcolorbuttom = _Lightmapshadowcolorbuttom;
		customlightmapData.L = _Lightmapsmooth;
		customlightmapData.lightmapssscolor = _LightmapsssColor;
		customlightmapData.lightmapDirintensity = _LightmapDirintensity;
	}
#endif

half4 fragOutput(VertexOutput input) {	
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	
	InputData inputData;
	inputData.positionWS = half3(input.tSpace0.w, input.tSpace1.w, input.tSpace2.w);
	inputData.viewDirectionWS = Unity_SafeNormalize(_WorldSpaceCameraPos.xyz - inputData.positionWS);
	inputData.shadowCoord = input.shadowCoord;
	half4 mainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw) * _BaseColor;
	half3 Albedo = mainTex_var.rgb;
#ifdef _INGAMEALPHA
	half Alpha = mainTex_var.a*_IngameAlpha;
#else
	half Alpha = mainTex_var.a ;
#endif

	#ifdef _SMASH_DISNEY_TOON
		half2 toonTex_var = SAMPLE_TEXTURE2D(_ToonTex, sampler_ToonTex, input.uv.zw).rg;
	#endif

#ifdef  _RADIAL
	
	float2 Center = (float2(-0.5, -0.5) + input.uv.xy);
	float Radial = frac((atan2((Center).x, (Center).y) / TWO_PI));
	 Radial = ceil(Radial - _AngleBegin) - ceil(Radial - (_Angle + _AngleBegin));
	 Radial = saturate(Radial);
	 Alpha *= Radial;

	/* float Ra = (_AngleBegin + _Angle * 0.5) * TWO_PI;
	 float w = -_Width * (1.0 / sin(_Angle * 0.5 * TWO_PI)) ;
	 float2 Roffset = w*0.015 * float2(sin(Ra), cos(Ra));
	 Roffset = clamp(Roffset, -6.28, 6.28);
	 float2 mCenter = (float2(-0.5, -0.5) + input.uv.xy + Roffset);
	 float  Radialmini = frac(atan2(mCenter.x, mCenter.y) / TWO_PI);
	 Radialmini  = ceil(Radialmini - _AngleBegin) - ceil(Radialmini - (_Angle + _AngleBegin));
	 Radialmini = saturate(Radialmini);*/

	 float2 LineCenter = float2(0.5, 0.5);
	 float LA = -_AngleBegin*TWO_PI;
	 float2 RotatoA = mul(input.uv.xy - LineCenter, float2x2(cos(LA), -sin(LA), sin(LA), cos(LA))) + LineCenter;
	 float LB = (-_AngleBegin - _Angle) * TWO_PI;
	 float2 RotatoB = mul(input.uv.xy - LineCenter, float2x2(cos(LB), -sin(LB), sin(LB), cos(LB))) + LineCenter;
	 float4 Atex = SAMPLE_TEXTURE2D(_GachaTex, sampler_GachaTex, RotatoA);
	 float4 Btex = SAMPLE_TEXTURE2D(_GachaTex, sampler_GachaTex, RotatoB);
	 float4 Lt = (Atex + Btex) * _LineColor * _LineColorIntensity;
	 //Radialmini *= _LineColor.rgb* _LineColor.a;
	 //float3 LColor = lerp(Albedo, _LineColor.rgb, _LineColor.a);
	// Albedo = lerp(Albedo, _LineColor.rgb * _LineColor.a, Radial - Radialmini);
	 Albedo = lerp(Albedo, Lt.rgb,saturate(pow( Lt.a, _Width)));
#endif
	#ifdef _ALPHATEST_ON
		clip(Alpha - _AlphaClipThreshold);
	#endif
	half3 WorldSpaceNormal = Unity_SafeNormalize(half3(input.tSpace0.z, input.tSpace1.z, input.tSpace2.z));
	#ifdef _SMASH_DISNEY_SPARKS
		float4 SnowSpaeksMaskTex_Var = SAMPLE_TEXTURE2D(_SnowSparksMask, sampler_SnowSparksMask, input.uv.xy * _SnowSparksMask_ST.xy + _SnowSparksMask_ST.zw);
		float4 SnowSparksTex_var = SAMPLE_TEXTURE2D(_SnowSparks, sampler_SnowSparks, input.uv.xy * _SnowSparks_ST.xy + _SnowSparks_ST.zw);
		half SnowNov = dot(WorldSpaceNormal, inputData.viewDirectionWS);
		half snov = saturate(((SnowNov - (1.0 - _SparksSize)) * _SparksSoft));
		half4 SnowSparks = pow(abs(SnowSparksTex_var.r * SnowSparksTex_var.g), _SparksPower) * snov * _SnowSparksColor* SnowSpaeksMaskTex_Var.r;
	
		Albedo += SnowSparks.xyz;

	#ifdef _SMASH_DDLOBBY_JUNE
	if(_Lobby_june == 1)
	{
		Light mainlight = GetMainLight();
		Albedo.xyz = lerp(Albedo.xyz , Albedo.xyz* saturate(mainlight.color),0.75);
	}
	#endif

	#endif
	#ifdef _SMASH_DISNEY_INITLIGHTING
		Light MainLight =  GetMainLight();
		half3 customlightdir = (_UseCustomDir == 1 )?  MainLight.direction : half3(-1,0,0);
		half NdotBackL = max(0,dot(WorldSpaceNormal, customlightdir));
		half NdotV = dot(WorldSpaceNormal, inputData.viewDirectionWS);
	#endif
	#ifdef _SMASH_DISNEY_SIMPLIFY
		half3 WorldSpaceTangent = half3(1.0, 0.0, 0.0);
		half3 WorldSpaceBiTangent = half3(0.0, 1.0, 0.0);
		half Metallic = _MixSimMetal;
		half Roughness = _MixSimRough;
		half AO = 1;
		half3 Emission = Albedo * max(0, pow(2, _EmissionIntensity) - 1);
		half Sheen = 0;
		half ClearCoatMask = 1;
		half subThickness = 1;
		half4 subsurfaceCol = half4(1.0h, 1.0h, 1.0h, 1.0h);
		inputData.normalWS = WorldSpaceNormal;
	#else
		half Metallic, Roughness, AO;
		half3 Emission;
		InitMetalRoughAOEmiss(input.uv.xy, Albedo, Metallic, Roughness, AO, Emission);
		#ifdef _SMASH_NONORMAL
			inputData.normalWS = WorldSpaceNormal;
			half3 WorldSpaceTangent = half3(1.0, 0.0, 0.0);
			half3 WorldSpaceBiTangent = half3(0.0, 1.0, 0.0);
			half Sheen = 0;
			half ClearCoatMask = 1;
			half subThickness = 1;
			half4 subsurfaceCol = half4(1.0h, 1.0h, 1.0h, 1.0h);
		#else
			half3 Normal;
			half SheenMask, ClearCoatMask;
			InitNormalMask(input.uv.xy, Normal, SheenMask, ClearCoatMask);
			#ifdef _SMASH_DISNEY_SHEEN
				half Sheen = _Sheen * SheenMask;
				#ifdef _SMASH_DDLOBBY_JUNE
					if(_Lobby_june == 1)
					{
						Sheen = 0;
					}
				#endif
			#else
				half Sheen = 0;
			#endif
			#ifdef _DetailAlbedo_ON
				InitDetailNormalMetalRough(input.uv.xy, SheenMask, Normal, Metallic, Roughness);
			#endif
			//Detail albedo map
			#ifdef _DetailAlbedo_ON 
				half4 detail_albedoVar = SAMPLE_TEXTURE2D(_DetailAlbedoTex, sampler_DetailAlbedoTex, input.uv.xy * _DetailAlbedoTex_ST.xy + _DetailAlbedoTex_ST.zw);
				Albedo *= detail_albedoVar; 
				Alpha *= detail_albedoVar.a;
			#endif
			#ifdef _SMASH_SNOW_ON
				float4 SnowTex = SAMPLE_TEXTURE2D(_SnowTex, sampler_SnowTex, inputData.positionWS.xz*0.1 * _SnowTiling)*_SnowColor;
				float3 SnowNormal = UnpackNormalScale(SAMPLE_TEXTURE2D(_SnowNormal, sampler_SnowNormal, inputData.positionWS.xz*0.1 * _SnowTiling), _SnowNormalScale);
				float4 SnowMaskTex = SAMPLE_TEXTURE2D(_SnowMask, sampler_SnowMask, inputData.positionWS.xz*0.1 * _SnowMask_ST.xy + _SnowMask_ST.zw);
				float dotNormalWS = saturate(dot(WorldSpaceNormal, float3(0, 1, 0)));
				float Mask = saturate(pow(dotNormalWS, _SnowSharpness)) * saturate(SnowMaskTex.r*_SnowMaskScale);
				Albedo = lerp(Albedo, SnowTex.rgb, Mask);
				Normal = lerp(Normal, SnowNormal, Mask);
			#endif
				half3 WorldSpaceTangent, WorldSpaceBiTangent;
				inputData.normalWS = NormalTSToWS(input, Normal, WorldSpaceNormal, WorldSpaceTangent, WorldSpaceBiTangent);
		#endif
		#ifdef _SMASH_DISNEY_INITLIGHTING
			#ifdef _SMASH_DISNEY_IGCAMP
				IngameCamp(NdotV,Emission);
			#endif
			#ifdef _SMASH_DISNEY_LOBBYADV
				// #ifndef _SMASH_DDLOBBY_JUNE
					if(_Lobby_june != 1)
					{
						half FMask = SAMPLE_TEXTURE2D(_LobbyMaskTex, sampler_LobbyMaskTex, input.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw).x;
						Lobby_Adv(NdotBackL,NdotV,Albedo, FMask,Emission);
					}
				// #endif	
			#endif
			#ifdef _SMASH_DISNEY_FX_ON
				float maskValue = 1.0;
				#ifdef _SMASH_DISNEY_FX_MASK_ON
					if(_UseFXSparkMask > 0.9)
					{
						maskValue = SAMPLE_TEXTURE2D(_FXSparkMask, sampler_FXSparkMask, input.uv.xy).r;
					}
				#endif
				FX_Invincible(NdotV, Emission,maskValue);
			#endif
		#endif
	#endif
	//Anistropic
	AnistroData anistroData;
	InitAnistropic(input.uv.xy, inputData.normalWS, WorldSpaceTangent, WorldSpaceBiTangent, anistroData,input.tangentw);
	//clearcoat
	ClearcoatData clearcoatData;
	InitClearcoat(WorldSpaceNormal, ClearCoatMask, clearcoatData);
	//subsurface
	SubsurfaceData sssData;
	InitSubsurface(input.uv.xy, Albedo, sssData);
	inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.lightmapUVOrVertexSH.xyz, inputData.normalWS);
	#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP_ENHANCENORM		
	if (_EnableHightLightNormal == 1) {
		half fac = saturate(dot(inputData.normalWS, normalize(_CustomLightmapdir)));
		half norBoost = (fac * fac + _LightmapNormEnhanceBrighter) * _LightmapNormEnhance + (1 - _LightmapNormEnhance);
		inputData.bakedGI *= norBoost;
	}
	#endif
	#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP
	if(_EnableOldCutsomLightmap == 1)
	{
		CustomlightmapData customlightmapData;
		InitCustomlightmap(customlightmapData);
		half3 CustomLightmapDir = normalize(_CustomLightmapdir.xyz);
		half NDL = dot(CustomLightmapDir,inputData.normalWS);
		inputData.bakedGI = _EnbaleCustomlightmap == 1 ? Customlightmapping(customlightmapData, inputData.bakedGI, NDL,inputData.normalWS) : inputData.bakedGI;
		#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP_MATCAP
			half2 matcapuvnormal;
			matcapuvnormal.x = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), inputData.normalWS);
			matcapuvnormal.y = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), inputData.normalWS);
			matcapuvnormal = matcapuvnormal * 0.5 + 0.5;
			half2 matcapuv = matcapuvnormal;
			half3 matCapColor = SAMPLE_TEXTURE2D(_MatcapTexture, sampler_MatcapTexture, matcapuv).xyz;
			inputData.bakedGI += _EnbaleMatCap == 1 ? matCapColor * _Matcap_Intensity *(1-Roughness) : 0;
		#endif
	}
	#endif
	#ifdef _SMASH_DISNEY_CUSTOMLIGHTING
		half shadowIntensity = _ShadowIntensity;
		half directLightIntensity = _DirectLightIntensity;
		half giIntensity = _GIIntensity;
		half giFresnel = _GIfresnel;
	#else
		half shadowIntensity = 1;
		half directLightIntensity = 1;
		half giIntensity = 1;
		half giFresnel = 1;
	#endif
	half SheenRange = 0;
	#ifdef _SMASH_DISNEY_SHEEN
		SheenRange = _SheenRange;
	#endif
	half4 color = DisneyBRDF( 
	inputData,
	Albedo,
	Metallic,
	Roughness,
	Emission,
	Alpha,
	1.0 - saturate((1.0 - AO) * _OcclusionIntensity),
	_OcclusionColor.rgb,
	Sheen, 
	SheenRange,
	#ifdef _SMASH_DISNEY_SHEEN
		_SheenTint,
	#else
		0,
	#endif
	anistroData,
	clearcoatData,
	sssData,
	shadowIntensity,
	directLightIntensity,
	giIntensity,
	giFresnel,
	#ifdef _SMASH_DISNEY_CUSTOMGISPE
		IndSpeByDifToggle, IndSpeByDifFac
	#else
		0, 0
	#endif
	#ifdef _SMASH_DISNEY_SPLIT_CHARAC
		, _GlobalDirectColor, _GlobalInDirectColor
	#else
		, 1, 1
	#endif
	#ifdef _SMASH_DISNEY_TOON
	,toonTex_var.r-1, toonTex_var.g
	#else
		,0, 1
	#endif
	);

	#if _DISSOLVE_INGAME_ON
		float4 dissolveNoise = SAMPLE_TEXTURE2D(_DissolveNoiseTexture, sampler_DissolveNoiseTexture, input.uv.xy * _DissolveNoiseTexture_ST.xy + _DissolveNoiseTexture_ST.zw);
		float pos = input.posOS.y;
		pos = _Postype < -0.001 ? input.posOS.x : pos;
		pos = _Postype > 0.001 ? input.posOS.z : pos;
		float VertexY = lerp((1.0 - pos),(1.0 + pos),_UpOrDown);
		float dissolve = 2.0 * pow(saturate(VertexY + (-1.0 + (_DissolveValue )  * 2.0)),max(0.001,_DissolvePow));
		dissolve = 1.0 - dissolve;
		dissolve = saturate(saturate(dissolveNoise.r - dissolve) / max(0.001,_EdgeWidth * dissolve));
		#ifdef _ALPHATEST_ON
			clip(dissolve- _AlphaClipThreshold);
		#endif
		color = lerp(_DissolveColor,color,dissolve);
	#endif

	#ifdef _SMASH_DISNEY_CAUSTICS
		half time = 0.01 * _Time.y;
		half3 c1 = SAMPLE_TEXTURE2D(_CausticsTex, sampler_CausticsTex, input.uv.xy * _CausticsTex_ST.xy + _CausticsTex_ST.zw + _CausticsSpeed.xy * time).rgb;
		half3 c2 = SAMPLE_TEXTURE2D(_CausticsTex, sampler_CausticsTex, input.uv.xy * _Caustics2thST.xy + _Caustics2thST.zw + _CausticsSpeed.zw * time).rgb;
		color.xyz += _GausticsAlpha * min(c1, c2);
	#endif
	#ifdef _SMASH_DISNEY_USEADVFOG
		#ifndef _SMASH_DISNEY_FOGSWITCH
			float4 fogColorAndOpacity = input.fogColorAndOpacity;
				// #ifdef _ADVANCE_UE4HEIGHTFOG_PIXEL
				if(_FogType == 2)
				{
					Light mainLight = GetMainLight();
					half3 mainLightDir = _CustomLight == 1 ? _CustomLightDir.xyz : mainLight.direction;
					fogColorAndOpacity = ComputeHeightFogColor(inputData.positionWS, float4(mainLightDir, 1));
				}
				// #endif
			color.rgb = MixHeightFog(color.rgb, fogColorAndOpacity);
		#endif
	#endif
	#ifdef XRP_OPT_DEPTH_PASS 
		#if !UNITY_REVERSED_Z
			// DX Metal Vulkan
			color.a = 1 - input.clipPos.z;
		#else
			// OpenGL
			color.a = input.clipPos.z;
		#endif
	#endif
	#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP
		color.rgb = _EnableDebugLightmap == 1 ? inputData.bakedGI + Emission : color.rgb;
	#endif

	//_Sparkle
	#if defined(_USE_SPARKLE)
	half4 sparkleNoise0 = SAMPLE_TEXTURE2D(_SparkleMap,sampler_SparkleMap,input.uv.xy * _SparkleMap_ST.xy + _SparkleMap_ST.zw);
    half4 sparkleNoise1 = SAMPLE_TEXTURE2D(_SparkleMap,sampler_SparkleMap,input.uv.xy * _SparkleMap_ST.xy + _SparkleMap_ST.zw + inputData.viewDirectionWS.xy * _SparklePow);
	//half NdotV = dot(WorldSpaceNormal, inputData.viewDirectionWS);
	half NoV = dot(WorldSpaceNormal, inputData.viewDirectionWS);
	half sparkleValue = saturate(sparkleNoise0.r * sparkleNoise1.r) * saturate(NoV - _SparkleRange);
	//color.rgb += sparkleValue * _SparkleTint.rgb;
	half3 r = input.randomColor.xyz * _SparkleTint.rgb;
	half3 randomColor = RotateAboutAxis(r,half3(1,1,1),dot(_WorldSpaceCameraPos.xyz,WorldSpaceNormal)) ;
	Light mainLight = GetMainLight(inputData.shadowCoord);
	color.rgb += lerp(sparkleValue * _SparkleTint.rgb,sparkleValue * randomColor,_SparkleTintLerpValue)  * lerp(1, mainLight.shadowAttenuation, shadowIntensity );
	//return half4(randomColor,1);
	//return half4(sparkleValue,sparkleValue,sparkleValue,1);
	#endif



	

	#ifdef _DA
	return float4(Albedo,1);
	#endif
	#ifdef _DN
	return SAMPLE_TEXTURE2D(_BumpMaskMap, sampler_BumpMaskMap, input.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw);
	#endif
	#ifdef _DM
	return Metallic;
	#endif
	#ifdef _DR
	return Roughness;
	#endif

	return color;
}

half4 furFragClipOutput(VertexOutput input) {
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	InputData inputData;
	inputData.positionWS = half3(input.tSpace0.w, input.tSpace1.w, input.tSpace2.w);
	inputData.viewDirectionWS = SafeNormalize(_WorldSpaceCameraPos.xyz - inputData.positionWS);
	inputData.shadowCoord = input.shadowCoord;
	// inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;

	half4 mainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw) * _BaseColor;
	half3 Albedo = mainTex_var.rgb;
	half Alpha = mainTex_var.a;
	
	clip(Alpha - _AlphaClipThreshold);


	half3 WorldSpaceNormal = SafeNormalize(half3(input.tSpace0.z, input.tSpace1.z, input.tSpace2.z));
	#ifdef _SMASH_DISNEY_SIMPLIFY
		half3 WorldSpaceTangent = half3(1.0, 0.0, 0.0);
		half3 WorldSpaceBiTangent = half3(0.0, 1.0, 0.0);
		half Metallic = _MixSimMetal;
		half Roughness = _MixSimRough;
		half AO = 1;
		half3 Emission = Albedo * max(0, pow(2, _EmissionIntensity) - 1);
		half Sheen = 0;
		half ClearCoatMask = 1;
		half subThickness = 1;
		half4 subsurfaceCol = half4(1.0h, 1.0h, 1.0h, 1.0h);
		inputData.normalWS = WorldSpaceNormal;
	#else
		half Metallic, Roughness, AO;
		half3 Emission;
		InitMetalRoughAOEmiss(input.uv.xy, Albedo, Metallic, Roughness, AO, Emission);

		half3 Normal;
		half SheenMask, ClearCoatMask;
		InitNormalMask(input.uv.xy, Normal, SheenMask, ClearCoatMask);
		#ifdef _SMASH_DISNEY_SHEEN
		half Sheen = _Sheen * SheenMask;
		#else
		half Sheen = 0;
		#endif

		#ifdef _DetailAlbedo_ON
			InitDetailNormalMetalRough(input.uv.xy, SheenMask, Normal, Metallic, Roughness);
		#endif

		half3 WorldSpaceTangent, WorldSpaceBiTangent;
		inputData.normalWS = NormalTSToWS(input, Normal, WorldSpaceNormal, WorldSpaceTangent, WorldSpaceBiTangent);

		//Detail albedo map
		#ifdef _DetailAlbedo_ON 
			half4 detail_albedoVar = SAMPLE_TEXTURE2D(_DetailAlbedoTex, sampler_DetailAlbedoTex, input.uv.xy * _DetailAlbedoTex_ST.xy + _DetailAlbedoTex_ST.zw);
			Albedo *= detail_albedoVar; 
			Alpha *= detail_albedoVar.a;
		#endif

		#ifdef _SMASH_DISNEY_FX_ON
			half NdotV = dot(WorldSpaceNormal, inputData.viewDirectionWS);
			float maskValue = 1.0;
			#ifdef _SMASH_DISNEY_FX_MASK_ON
				if(_UseFXSparkMask > 0.9)
				{
					maskValue = SAMPLE_TEXTURE2D(_FXSparkMask, sampler_FXSparkMask, input.uv.xy).r;
				}
			#endif
			FX_Invincible(NdotV, Emission,maskValue);
		#endif
	#endif

	//Anistropic
	AnistroData anistroData;
	InitAnistropic(input.uv.xy, inputData.normalWS, WorldSpaceTangent, WorldSpaceBiTangent, anistroData,input.tangentw);

	//clearcoat
	ClearcoatData clearcoatData;
	InitClearcoat(WorldSpaceNormal, ClearCoatMask, clearcoatData);

	//subsurface
	SubsurfaceData sssData;
	InitSubsurface(input.uv.xy, Albedo, sssData);

	inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.lightmapUVOrVertexSH.xyz, inputData.normalWS);

	#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP
		CustomlightmapData customlightmapData;
		InitCustomlightmap(customlightmapData);
		half NDL = dot(normalize(_CustomLightmapdir.xyz),inputData.normalWS);
		inputData.bakedGI = _EnbaleCustomlightmap == 1 ? Customlightmapping(customlightmapData, inputData.bakedGI, NDL,inputData.normalWS) : inputData.bakedGI ;
		#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP_MATCAP
			// #ifdef _MPN
			half2 matcapuvnormal;
			matcapuvnormal.x = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), inputData.normalWS);
			matcapuvnormal.y = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), inputData.normalWS);
			matcapuvnormal = matcapuvnormal * 0.5 + 0.5;
			// half2 matcapuv = _EnableMatcapNormal == 1 ? matcapuvnormal : input.matcapuv ;
			// #endif
			half2 matcapuv = matcapuvnormal;

			float3 matCapColor = SAMPLE_TEXTURE2D(_MatcapTexture, sampler_MatcapTexture, matcapuv).xyz;
			inputData.bakedGI += _EnbaleMatCap == 1 ? matCapColor * _Matcap_Intensity : 0;
		#endif
	#endif

	#ifdef _SMASH_DISNEY_CUSTOMLIGHTING
		half shadowIntensity = _ShadowIntensity;
		half directLightIntensity = _DirectLightIntensity;
		half giIntensity = _GIIntensity;
	#else
		half shadowIntensity = 1;
		half directLightIntensity = 1;
		half giIntensity = 1;
	#endif

	half SheenRange = 0;
	#ifdef _SMASH_DISNEY_SHEEN
		SheenRange = _SheenRange;
	#endif

	half4 color = DisneyBRDF(
	inputData,
	Albedo,
	Metallic,
	Roughness,
	Emission,
	Alpha,
	1.0 - saturate((1.0 - AO) * _OcclusionIntensity),
	_OcclusionColor.rgb,
	Sheen, 
	SheenRange,
#ifdef _SMASH_DISNEY_SHEEN
	_SheenTint,
#else
	0,
#endif
	anistroData,
	clearcoatData,
	sssData,
	shadowIntensity,
	directLightIntensity,
	giIntensity,
#ifdef _SMASH_DISNEY_CUSTOMGISPE
	IndSpeByDifToggle, IndSpeByDifFac
#else
	0, 0
#endif
	);

	#ifdef _SMASH_DISNEY_USEADVFOG
		// color.rgb = (_Fog == 2) ? MixHeightFog(color.rgb, input.fogColorAndOpacity) :
		// (_Fog == 1 ? MixFog(color.rgb, input.fogFactorAndVertexLight.x) : color.rgb);
		float4 fogColorAndOpacity = input.fogColorAndOpacity;
		// #ifdef _ADVANCE_UE4HEIGHTFOG_PIXEL
		if(_FogType == 2)
		{
			Light mainLight = GetMainLight();
			half3 mainLightDir = _CustomLight == 1 ? _CustomLightDir.xyz : mainLight.direction;
			fogColorAndOpacity = ComputeHeightFogColor(inputData.positionWS, float4(mainLightDir, 1));
		}
		// #endif

	color.rgb = MixHeightFog(color.rgb, fogColorAndOpacity);

	#endif

	#ifdef XRP_OPT_DEPTH_PASS 
		#if !UNITY_REVERSED_Z
			// DX Metal Vulkan
			color.a = 1 - input.clipPos.z;
		#else
			// OpenGL
			color.a = input.clipPos.z;
		#endif
	#endif
	#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP
		color.rgb = _EnableDebugLightmap == 1 ? inputData.bakedGI : color.rgb;
	#endif
	return color;
}
half4 furFragTransOutput(VertexOutput input) {
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	InputData inputData;
	inputData.positionWS = half3(input.tSpace0.w, input.tSpace1.w, input.tSpace2.w);
	inputData.viewDirectionWS = SafeNormalize(_WorldSpaceCameraPos.xyz - inputData.positionWS);
	inputData.shadowCoord = input.shadowCoord;
	// inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;

	half4 mainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw) * _BaseColor;
	half3 Albedo = mainTex_var.rgb;
	half Alpha = mainTex_var.a;
	
	// clip(Alpha - _AlphaClipThreshold);


	half3 WorldSpaceNormal = SafeNormalize(half3(input.tSpace0.z, input.tSpace1.z, input.tSpace2.z));
	#ifdef _SMASH_DISNEY_SIMPLIFY
		half3 WorldSpaceTangent = half3(1.0, 0.0, 0.0);
		half3 WorldSpaceBiTangent = half3(0.0, 1.0, 0.0);
		half Metallic = _MixSimMetal;
		half Roughness = _MixSimRough;
		half AO = 1;
		half3 Emission = Albedo * max(0, pow(2, _EmissionIntensity) - 1);
		half Sheen = 0;
		half ClearCoatMask = 1;
		half subThickness = 1;
		half4 subsurfaceCol = half4(1.0h, 1.0h, 1.0h, 1.0h);
		inputData.normalWS = WorldSpaceNormal;
	#else
		half Metallic, Roughness, AO;
		half3 Emission;
		InitMetalRoughAOEmiss(input.uv.xy, Albedo, Metallic, Roughness, AO, Emission);

		half3 Normal;
		half SheenMask, ClearCoatMask;
		InitNormalMask(input.uv.xy, Normal, SheenMask, ClearCoatMask);
		#ifdef _SMASH_DISNEY_SHEEN
			half Sheen = _Sheen * SheenMask;
		#else
			half Sheen = 0;
		#endif

		#ifdef _DetailAlbedo_ON
			InitDetailNormalMetalRough(input.uv.xy, SheenMask, Normal, Metallic, Roughness);
		#endif

		half3 WorldSpaceTangent, WorldSpaceBiTangent;
		inputData.normalWS = NormalTSToWS(input, Normal, WorldSpaceNormal, WorldSpaceTangent, WorldSpaceBiTangent);

		//Detail albedo map
		#ifdef _DetailAlbedo_ON 
			half4 detail_albedoVar = SAMPLE_TEXTURE2D(_DetailAlbedoTex, sampler_DetailAlbedoTex, input.uv.xy * _DetailAlbedoTex_ST.xy + _DetailAlbedoTex_ST.zw);
			Albedo *= detail_albedoVar; 
			Alpha *= detail_albedoVar.a;
		#endif

		#ifdef _SMASH_DISNEY_FX_ON
			half NdotV = dot(WorldSpaceNormal, inputData.viewDirectionWS);
			float maskValue = 1.0;
			#ifdef _SMASH_DISNEY_FX_MASK_ON
				if(_UseFXSparkMask > 0.9)
				{
					maskValue = SAMPLE_TEXTURE2D(_FXSparkMask, sampler_FXSparkMask, input.uv.xy).r;
				}
			#endif
			FX_Invincible(NdotV, Emission,maskValue);
		#endif
	#endif

	//Anistropic
	AnistroData anistroData;
	InitAnistropic(input.uv.xy, inputData.normalWS, WorldSpaceTangent, WorldSpaceBiTangent, anistroData,input.tangentw);

	//clearcoat
	ClearcoatData clearcoatData;
	InitClearcoat(WorldSpaceNormal, ClearCoatMask, clearcoatData);

	//subsurface
	SubsurfaceData sssData;
	InitSubsurface(input.uv.xy, Albedo, sssData);

	inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.lightmapUVOrVertexSH.xyz, inputData.normalWS);

	#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP
		CustomlightmapData customlightmapData;
		InitCustomlightmap(customlightmapData);
		half NDL = dot(normalize(_CustomLightmapdir.xyz),inputData.normalWS);
		inputData.bakedGI = _EnbaleCustomlightmap == 1 ? Customlightmapping(customlightmapData, inputData.bakedGI, NDL,inputData.normalWS) : inputData.bakedGI ;
		#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP_MATCAP
			// #ifdef _MPN
			half2 matcapuvnormal;
			matcapuvnormal.x = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), inputData.normalWS);
			matcapuvnormal.y = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), inputData.normalWS);
			matcapuvnormal = matcapuvnormal * 0.5 + 0.5;
			// half2 matcapuv = _EnableMatcapNormal == 1 ? matcapuvnormal : input.matcapuv ;
			half2 matcapuv = matcapuvnormal;

			// #endif
			half3 matCapColor = SAMPLE_TEXTURE2D(_MatcapTexture, sampler_MatcapTexture, matcapuv).xyz;
			inputData.bakedGI += _EnbaleMatCap == 1 ? matCapColor * _Matcap_Intensity * (1-Roughness): 0;
		#endif
	#endif

	#ifdef _SMASH_DISNEY_CUSTOMLIGHTING
		half shadowIntensity = _ShadowIntensity;
		half directLightIntensity = _DirectLightIntensity;
		half giIntensity = _GIIntensity;
	#else
		half shadowIntensity = 1;
		half directLightIntensity = 1;
		half giIntensity = 1;
	#endif

	half SheenRange = 0;
	#ifdef _SMASH_DISNEY_SHEEN
			SheenRange = _SheenRange;
	#endif

	half4 color = DisneyBRDF(
	inputData,
	Albedo,
	Metallic,
	Roughness,
	Emission,
	Alpha,
	1.0 - saturate((1.0 - AO) * _OcclusionIntensity),
	_OcclusionColor.rgb,
	Sheen, 
	SheenRange,
	#ifdef _SMASH_DISNEY_SHEEN
		_SheenTint,
	#else
		0,
	#endif
	anistroData,
	clearcoatData,
	sssData,
	shadowIntensity,
	directLightIntensity,
#ifdef _SMASH_DISNEY_CUSTOMGISPE
	IndSpeByDifToggle, IndSpeByDifFac
#else
	0, 0
#endif
	);

	#ifdef _SMASH_DISNEY_USEADVFOG
			color.rgb = MixHeightFog(color.rgb, input.fogColorAndOpacity);
	#endif

	#ifdef XRP_OPT_DEPTH_PASS 
		#if !UNITY_REVERSED_Z
			// DX Metal Vulkan
			color.a = 1 - input.clipPos.z;
		#else
			// OpenGL
			color.a = input.clipPos.z;
		#endif
	#endif
	#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP
		color.rgb = _EnableDebugLightmap == 1 ? inputData.bakedGI : color.rgb;
	#endif
	return color;
}
half4 frag(VertexOutput input) : SV_Target
{
	return fragOutput(input);
}
half4 furfragclip(VertexOutput input) : SV_Target
{
	return furFragClipOutput(input);
}
half4 furfragtrans(VertexOutput input) : SV_Target
{
	return furFragTransOutput(input);
}

