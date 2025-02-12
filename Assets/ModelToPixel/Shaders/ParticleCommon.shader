Shader "Smash_Client/Effect/ParticleCommon"
{
	Properties
	{
		[HideInInspector] _BlendMode("BlendMode", Float) = 1
		[Enum(UnityEngine.Rendering.CullMode)][HideInInspector] _RenderFace("RenderFace", Float) = 2
		[Enum(UnityEngine.Rendering.BlendMode)][HideInInspector]_SrcBlend("SrcBlend", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)][HideInInspector]_DstBlend("DstBlend", Float) = 10
		[Enum(UnityEngine.Rendering.CompareFunction)][HideInInspector]_ZTest("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("CullMode", Float) = 2.0
		[Enum(Off, 0, On, 1)]_ZWrite("ZWrite", Float) = 0
		_Offsetx("_Offsetx",float) = 0
		_Offsety("_Offsety",float) = 0
		[Toggle(_DEPTHFADE)]_Depthfade("DepthFade (Don't use Ingame)",Float) = 0
		[ShowIf(_Depthfade)]_DepthDistance("DepthDistance",float) = 0
		[HideInInspector][Toggle(_AlphaClip)] _AlphaClip("AlphaClip", Float) = 0
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[ShowIf(_AlphaClip)] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_Base("Base", 2D) = "white" {}
		_OpacityIntersity("OpacityIntersity", Float) = 1
//        [Toggle(_TWOFACE_ON)][Space(10)]_TwoFace("TwoFace",Float)=0
		[Toggle][Space(10)]_TWOFACE_ON("TwoFace",Float) = 0
//		[Toggle(_3COLOR_ON)]_3Color("3Color",Float) = 0
		[Toggle][Space(10)]_3COLOR_ON("3Color",Float) = 0
		[HDR]_ColorBlack("ColorBlack", Color) = (1,1,1,1)
		[ShowIf(_3COLOR_ON)][HDR]_ColorMiddle("ColorMiddle",Color)=(1,1,1,1)
		[HDR]_ColorLight("ColorLight", Color) = (1,1,1,1)
			//add bloom
//		[Toggle(_ADDBLOOM_ON)][Space(10)] _AddBloom("AddBloom", Float) = 0
		[Toggle][Space(10)] _ADDBLOOM_ON("AddBloom", Float) = 0
		_ColorLightIntensity("ColorLightIntensity",float) = 1
		[ShowIf(_ADDBLOOM_ON)][HDR][Gamma]_ColorLight01("ColorLight01", Color) = (1,1,1,1)
		[ShowIf(_ADDBLOOM_ON)]_GammaLerp("GammaLerp",float) = 1
		_BaseU_Speed("BaseU_Speed", Float) = 0
		_BaseV_Speed("BaseV_Speed", Float) = 0
		[Toggle(_AUTONOISE_ON)][Space(10)] _AutoNoise("AutoNoise", Float) = 0
		[ShowIf(_AutoNoise)]_Noise("Noise", 2D) = "white"{}
		[ShowIf(_AutoNoise)]_NoiseIntersity("NoiseIntersity", Float) = 0
		[ShowIf(_AutoNoise)]_NoiseU_Speed("NoiseU_Speed", Float) = 0
		[ShowIf(_AutoNoise)]_NoiseV_Speed("NoiseV_Speed", Float) = 0
		[Toggle(_NOISEMASK_ON)]_useNoiseMask("UseNoiseMask",Float) = 0
		[ShowIf(_useNoiseMask)]_NoiseMask("NoiseMask",2d)= "white"{}
			//additive
		[Toggle(_AUTOADDITIVE_ON)][Space(10)] _AutoAdditive("AutoAdditive", Float) = 0
		[ShowIf(_AutoAdditive)]_Additive("Additive", 2D) = "white" {}
		[ShowIf(_AutoAdditive)]_AddRotato("AddRotato",Range(0,1))=0
		[ShowIf(_AutoAdditive)]_AdditiveU_Speed("AdditiveU_Speed", Float) = 0
		[ShowIf(_AutoAdditive)]_AdditiveV_Speed("AdditiveV_Speed", Float) = 0
		[ShowIf(_AutoAdditive)][HDR]_AdditiveColor("AdditiveColor", Color) = (1,1,1,1)

		[Toggle(_AUTOMASK_ON)][Space(10)] _AutoMask("AutoMask", Float) = 0
//		[ShowIf(_AutoMask)][Toggle(_MASKCUS)][Space(10)]_MaskCus("UseMaskCus",Float) = 0
		[ShowIf(_AutoMask)][Toggle][Space(10)]_MASKCUS("UseMaskCus",Float) = 0

		[ShowIf(_AutoMask)]_Mask("Mask", 2D) = "white" {}
		[ShowIf(_AutoMask)]_MaskU_Speed("MaskU_Speed", Float) = 0
		[ShowIf(_AutoMask)]_MaskV_Speed("MaskV_Speed", Float) = 0
		[ShowIf(_AutoMask)]_MaskRotato("MaskRotato",Range(0,1)) = 0
			//dissolve
		[Toggle(_AUTODISSOLVE_ON)][Space(10)] _AutoDissolve("AutoDissolve", Float) = 0
		[ShowIf(_AutoDissolve)]_DissolveByBaseAlpha("DissolveByBaseAlpha", Range( 0 , 1)) = 0
		[ShowIf(_AutoDissolve)][Enum(On,0,Off,1)]_ParticleCtrlOff("ParticleCtrlOff",Int)=0
			
		[ShowIf(_AutoDissolve)]_Dissolve("Dissolve", 2D) = "white" {}
		[ShowIf(_AutoDissolve)]_Dissolve2U("Dissolve2U",Range(0,1))=0
		[ShowIf(_AutoDissolve)]_DissolveU_Speed("DissolveU_Speed", Float) = 0
		[ShowIf(_AutoDissolve)]_DissolveV_Speed("DissolveV_Speed", Float) = 0
		[ShowIf(_AutoDissolve)]_EdgeSharpness("EdgeSharpness", Float) = 5
		[ShowIf(_AutoDissolve)]_DissolvePreview("DissolvePreview", Float) = 0
		[ShowIf(_AutoDissolve)][Toggle(_AUTODISSOLVEEDGE_ON)]_AutoDissolveWidth("AutoDissolveWidth",Float) = 0
		[ShowIf(_AutoDissolveWidth)] _DissolveWidth("DissolveWidth",Float) = 0
		[ShowIf(_AutoDissolveWidth)][HDR]_DissWidthColor("DissWidthColor",Color) = (1,1,1,1)
		[ShowIf(_AutoDissolveWidth)] _DissolveWidthIntensity("DissolveWidthIntensity",Float) = 0
			//vat
//		[Toggle(_AUTOVERTEXANI_ON)][Space(10)] _AutoVertexAni("AutoVertexAni", Float) = 0
		[Toggle][Space(10)] _AUTOVERTEXANI_ON("AutoVertexAni", Float) = 0
		[ShowIf(_AUTOVERTEXANI_ON)]_VertexAni("VertexAni", 2D) = "white"{}
		[ShowIf(_AUTOVERTEXANI_ON)] _VertexV_Speed("VertexV_Speed",Float) = 0
		[ShowIf(_AUTOVERTEXANI_ON)]_VertexU_Speed("VertexU_Speed",Float)= 0
		[ShowIf(_AUTOVERTEXANI_ON)]_VertexAniScale("VertexAniIntensity",Float) = 0
			//fresnel
		[Toggle(_AUTOFRESNEL_ON)][Space(10)]_AutoFresnel("AutoFresnel",Float) = 0
        //[ShowIf(_AutoFresnel)]_FrenelBRF
		[HDR][ShowIf(_AutoFresnel)]_FresnelColor("FresnelColor", Color) = (1,1,1,0)
		[ShowIf(_AutoFresnel)]_FresnelScale("FresnelScale", Range(0 , 10)) = 1
		[ShowIf(_AutoFresnel)]_FresnelPower("FresnelPower", Float) = 5
		[ShowIf(_AutoFresnel)]_FrenelBRF("FrenelBRF",Range(-1 , 1)) = 0.04
		[ShowIf(_AutoFresnel)]_MeshFade("MeshFade",Range(0 , 1)) = 0
		[ShowIf(_AutoFresnel)]_FresnelAlpha("FresnelAlpha",Range(0 , 1)) = 0

		[Toggle(_DISTORTION_ON)][Space(10)]_Distortion("Use Distortion",Float) = 0
		[ShowIf(_Distortion)]_DistorNoiseTexture ("Distortion Noise", 2D) = "white" {}
        [ShowIf(_Distortion)]_DistorMaskTexture ("Distortion Mask", 2D) = "white" {}
        [ShowIf(_Distortion)]_DistortionIntensity ("Distortion Intensity", Range(0,2)) = 0.5
        [ShowIf(_Distortion)]_DistortionParameter("Distortion Parameter",Vector) = (1,1,1,1)

		[Toggle(_OUTLINE_ON)][Space(10)]_OutLine("Use OutLine",Float) = 0
		//[Toggle(_VERTEXOUTLINE_ON)][Space(10)]_VertexOutLine("Use VertexOutLine",Float) = 0
		[ShowIf(_OutLine)][HDR]_OutLineColor("OutLineColor", Color) = (1,1,1,1)
		[ShowIf(_OutLine)]_OutLineWidth("OutLineWidth",Range(0,5)) = 0
		[ShowIf(_OutLine)]_OutLineAlphaThreshold("OutLineAlphaThreshold",Range(0,1)) = 0
		[Toggle(_CHARACTER_DEPTH_ON)][Space(10)]_characterDepth("Use CharacterDepth Occlusion",Float) = 0
		_StencilValue("Stencil Value",float) = 0
		_StencilComp("Stencil Comp",float) = 8
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		HLSLINCLUDE
		#pragma target 2.0


		ENDHLSL

		Pass
		{
			Name "Forward"
			
			Blend[_SrcBlend][_DstBlend]
			// Cull[_RenderFace]
			// ZWrite[_ZWrite]
			Cull [_Cull]
			ZWrite [_ZWrite]
			ZTest[_ZTest]
			Offset [_Offsetx], [_Offsety]
			//ColorMask RGBA
			
			Stencil
			{
				Ref [_StencilValue]
				Comp [_StencilComp]
				Pass Keep
			}

			HLSLPROGRAM
			#pragma multi_compile_instancing
			//#pragma multi_compile _ _AlphaClip
//#ifdef _DEPTHFADE
//
//		#define REQUIRE_DEPTH_TEXTURE 1
//#endif
			
            #pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#pragma shader_feature_local _AUTONOISE_ON
			#pragma shader_feature_local _AUTODISSOLVE_ON
			#pragma shader_feature_local _AUTOMASK_ON
			#pragma shader_feature_local _AUTOFRESNEL_ON
		    #pragma shader_feature_local _AUTODISSOLVEEDGE_ON
		    #pragma shader_feature_local _AUTOADDITIVE_ON
		    #pragma shader_feature_local _AlphaClip
		    #pragma shader_feature_local _NOISEMASK_ON
		    #pragma shader_feature_local _DEPTHFADE
			#pragma shader_feature_local _DISTORTION_ON
			#pragma shader_feature_local _OUTLINE_ON
			#pragma shader_feature_local _CHARACTER_DEPTH_ON

			uniform float _AlphaPow;
			
			// variant
			// #pragma shader_feature_local _TWOFACE_ON
			// #pragma shader_feature_local _MASKCUS
			// #pragma shader_feature_local _3COLOR_ON
			// #pragma shader_feature_local _ADDBLOOM_ON
			// #pragma shader_feature_local _AUTOVERTEXANI_ON

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 uv3 : TEXCOORD5;
				float3 NormalWS:TEXCOORD1;
				float3 WorldPos:TEXCOORD2;
				float4 ase_texcoord5 : TEXCOORD6;
				float4 ase_color : COLOR;
				#ifdef _DISTORTION_ON
					float4 screenPos : TEXCOORD7;
				#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
//#ifdef _DEPTHFADE
//			uniform float4 _CameraDepthTexture_TexelSize;
//#endif
			CBUFFER_START(UnityPerMaterial)
			float4 _ColorBlack;
			float4 _VertexAni_ST;
			float4 _Dissolve_ST;
			float4 _Noise_ST;
			float4 _Base_ST;
			float4 _Mask_ST;
			float4 _ColorLight;
			float4 _ColorLight01;
			float4 _ColorMiddle;
			//fresnel
			float4 _FresnelColor;
			//dissolvewidth
			float4 _DissWidthColor;
			float4 _AdditiveColor;
			float4 _Additive_ST;
			float4 _NoiseMask_ST;
			float _AddRotato;

			float _VertexV_Speed;
			float _VertexU_Speed;
			float _VertexAniScale;
			float _ColorLightIntensity;
			float _GammaLerp;

			float _NoiseU_Speed;
			float _NoiseV_Speed;
			float _BaseU_Speed;
			float _NoiseIntersity;
			float _MaskV_Speed;

			float _Dissolve2U;
			float _DissolveU_Speed;
			float _DissolveV_Speed;
			float _DissolveByBaseAlpha;
			float _EdgeSharpness;
			float _DissolvePreview;
			float _MaskU_Speed;
			float _BaseV_Speed;
			float _OpacityIntersity;
			float _MaskRotato;

			//fresnel
			float _FrenelBRF;
			float _FresnelScale;
			float _FresnelPower;
			float _FresnelAlpha;
			float _MeshFade;

			//dissolvewidth
			float _DissolveWidth;
			float _DissolveWidthIntensity;
			float _AdditiveU_Speed;
			float _AdditiveV_Speed;
			float _AlphaCutoff;
			float _Dissovle2U;
			//variant
			float _TWOFACE_ON;
			float _3COLOR_ON;
			float _MASKCUS;
			float _ADDBLOOM_ON;
			float _AUTOVERTEXANI_ON;
			float _ParticleCtrlOff;
			//float _DEPTHFADE;
			float _DstBlend;
 

			float _DepthDistance;
			float4 _OutLineColor;
			
			float _OutLineAlphaThreshold;
			float _OutLineWidth;
            float _SmoothDelta;
			

			#ifdef _DISTORTION_ON
				float4 _DistortionParameter;
				float4 _DistorNoiseTexture_ST;
				float _DistortionIntensity;
			#endif
			CBUFFER_END
			TEXTURE2D(_Base);
			TEXTURE2D(_Noise);
			SAMPLER(sampler_Noise);
			SAMPLER(sampler_Base);
			TEXTURE2D(_Dissolve);
			SAMPLER(sampler_Dissolve);
			TEXTURE2D(_Mask);
			SAMPLER(sampler_Mask);
			TEXTURE2D(_VertexAni);
			SAMPLER(sampler_VertexAni);
			TEXTURE2D(_Additive);
			SAMPLER(sampler_Additive);
			TEXTURE2D(_NoiseMask);  //NoiseMask
			SAMPLER(sampler_NoiseMask);
			TEXTURE2D(_BumpMaskMap);
			SAMPLER(sampler_BumpMaskMap);

			TEXTURE2D_SHADOW(_CharacterDepthTexture);
			SAMPLER_CMP(sampler_CharacterDepthTexture);
		

			#ifdef _DISTORTION_ON
				TEXTURE2D(_CameraOpaqueTexture);
				SAMPLER(sampler_CameraOpaqueTexture);
				TEXTURE2D(_DistorNoiseTexture);
				SAMPLER(sampler_DistorNoiseTexture);
				TEXTURE2D(_DistorMaskTexture);
				SAMPLER(sampler_DistorMaskTexture);
			#endif

			int u_RecordMode_ON;//Only DX11
			// sRGB
	/*real Gamma22ToLinear(real c)
	{
		return PositivePow(c, 2.2);
	}

	real3 Gamma22ToLinear(real3 c)
	{
		return PositivePow(c.rgb, real3(2.2, 2.2, 2.2));
	}

	real4 Gamma22ToLinear(real4 c)
	{
		return real4(Gamma22ToLinear(c.rgb), c.a);

		
	}*/
			float aaStep(float compValue, float gradient){
			    float change = fwidth(gradient);
			    //base the range of the inverse lerp on the change over two pixels
			    float lowerEdge = compValue - change;
			    float upperEdge = compValue + change;
			    //do the inverse interpolation
			    float stepped = (gradient - lowerEdge) / (upperEdge - lowerEdge);
			    stepped = saturate(stepped);
			    //smoothstep version here would be `smoothstep(lowerEdge, upperEdge, gradient)`
			    return stepped;
			}

			VertexOutput VertexFunction ( VertexInput v  )
			{
				
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_texcoord4 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				o.uv3 = v.texcoord2;
				// #ifdef _AUTOVERTEXANI_ON
				if(_AUTOVERTEXANI_ON == 1)
				{
					float2 uv_Vertex = v.ase_texcoord.xy * _VertexAni_ST.xy + _VertexAni_ST.zw;
					float2 VertexSpeed = float2(_VertexV_Speed, _VertexU_Speed);
					float2 VertexPanner = ( _Time.y * VertexSpeed + uv_Vertex);
					float4 VertexTex = SAMPLE_TEXTURE2D_LOD(_VertexAni, sampler_VertexAni ,VertexPanner,0.0 );
					float3 vat = VertexTex.r * v.ase_color.xyz*_VertexAniScale;
					//setting value to unused interpolator channels and avoid initialization warnings
					v.vertex.xyz += vat;
				}
				// #endif
				//o.ase_texcoord3.zw = 0;
				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );
				o.WorldPos = positionWS;
				o.NormalWS = TransformObjectToWorldNormal(v.ase_normal.xyz);
				o.clipPos = positionCS;

				#ifdef _DISTORTION_ON
					o.screenPos = ComputeScreenPos(o.clipPos);
				#endif

//#ifdef _DEPTHFADE
				#ifdef _CHARACTER_DEPTH_ON
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				screenPos.z = -mul(UNITY_MATRIX_V, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0))).z;
				o.ase_texcoord5 = screenPos;
				#endif
//#endif
				return o;
			}
			
			float4 AlphaBlend(float4 top, float4 bottom) {
				float3 ca = top.xyz;
				float aa = top.w;
				float3 cb = bottom.xyz;
				float ab = bottom.w;
				float alpha = (aa + ab * (1 - aa));
				float3 color = (ca * aa + cb * ab * (1 - aa)) / alpha;
				return float4(color, alpha);
			}
			

			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}

			half4 frag ( VertexOutput IN  , half vface : VFACE) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 BaseSpeed = (float2(_BaseU_Speed , _BaseV_Speed));
				float2 uv_Base = IN.ase_texcoord3.xy * _Base_ST.xy + _Base_ST.zw;
				float4 UV2 = IN.ase_texcoord4;
					UV2 = lerp(UV2, 0, _ParticleCtrlOff);
				//UV2.xy = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );lerp(_ColorLight,(_ColorLight),_GammaLerp)
				float2 UV2ZW = (float2(UV2.z , UV2.w));
				float2 BasePanner = ( 1.0 * _Time.y * BaseSpeed + ( uv_Base + UV2ZW ));
				float2 NoiseSpeed = (float2(_NoiseU_Speed , _NoiseV_Speed));
				float2 uv_Noise = IN.ase_texcoord3.xy * _Noise_ST.xy + _Noise_ST.zw;
				float2 NoisePanner = ( 1.0 * _Time.y * NoiseSpeed + uv_Noise);
				#ifdef _AUTONOISE_ON
					float NoiseSwitch = ( SAMPLE_TEXTURE2D( _Noise, sampler_Noise, NoisePanner ).r * _NoiseIntersity );
					#ifdef _NOISEMASK_ON
						float2 uv_NoiseMask = IN.ase_texcoord3.xy * _NoiseMask_ST.xy + _NoiseMask_ST.zw;
						float NoiseMaskTex = SAMPLE_TEXTURE2D(_NoiseMask, sampler_NoiseMask, uv_NoiseMask).r;
						NoiseSwitch *= NoiseMaskTex;
					#endif
				#else
					float NoiseSwitch = 0.0;
				#endif
				float4 BaseTex = SAMPLE_TEXTURE2D( _Base, sampler_Base, ( BasePanner + NoiseSwitch ) );
				
				//record mode
				#ifdef SHADER_API_D3D11
					if(u_RecordMode_ON == 1)
					{
						if(_DstBlend > 0.9f && _DstBlend < 1.1f )//addtive render mode
						{
							float gray = BaseTex.r * 0.299 + BaseTex.g * 0.587 + BaseTex.b * 0.114;
							BaseTex.a = gray;
						}
					}
				#endif
				
				float4 ColorLight = _ColorLight * _ColorLightIntensity;
				// #ifdef _ADDBLOOM_ON
				if(_ADDBLOOM_ON == 1)
				{
					ColorLight = lerp(_ColorLight, _ColorLight01, _GammaLerp) * _ColorLightIntensity;
				}
				// #endif

				float4 ColorLerp = lerp( _ColorBlack , ColorLight, BaseTex);
				// #ifdef  _3COLOR_ON
				if(_3COLOR_ON == 1)
				{
					float4 lerpColor01 = lerp(_ColorBlack, _ColorMiddle, saturate((BaseTex * 2.0)));
					float4 lerpColor02 = lerp(_ColorMiddle, ColorLight, saturate(((BaseTex -0.5) * 2.0)));
					ColorLerp = lerp(lerpColor01, lerpColor02, BaseTex);
				}
				// #endif

				// #ifdef _TWOFACE_ON
				float4 ColorLerptemp = ColorLerp;
					ColorLerp = (_TWOFACE_ON == 1) ? BaseTex * (vface * GetOddNegativeScale() > 0 ? _ColorBlack : ColorLight) : ColorLerptemp;
				// #endif
				float fresnelalpha = 1.0;
				#ifdef _AUTOFRESNEL_ON
					float3 ViewDir = (_WorldSpaceCameraPos.xyz - IN.WorldPos);
					ViewDir = normalize(ViewDir);
					//float3 nor = ((vface > 0) ? (IN.NormalWS) : (IN.NormalWS));
					float NOV = abs(dot(IN.NormalWS, ViewDir));
					float fresnelNode = (_FrenelBRF + _FresnelScale * pow(1.0 - NOV, (_FresnelPower)));
					fresnelNode = lerp(saturate(fresnelNode), saturate(1 - saturate(fresnelNode)), _MeshFade);
					fresnelalpha = lerp(1,fresnelNode , _FresnelAlpha);
					float4 FresnelColor = clamp((_FresnelColor * fresnelNode),float4(0,0,0,0),float4(50,50,50,50));
					ColorLerp += FresnelColor;
				#endif
				float4 AdditiveColor = float4(0, 0, 0, 0);
				#ifdef _AUTOADDITIVE_ON
					float2 uv_Additive = IN.ase_texcoord3.xy * _Additive_ST.xy + _Additive_ST.zw;
					float2 AdditiveSpeed = (float2(_AdditiveU_Speed, _AdditiveV_Speed));
					float2 AdditivePanner = (1.0 * _Time.y * AdditiveSpeed + uv_Additive);

					float cosA = cos((_AddRotato * PI));
					float sinA = sin((_AddRotato * PI));
					AdditivePanner = mul(AdditivePanner - (float2(0.5, 0.5) + AdditiveSpeed + _Additive_ST.zw), float2x2(cosA, -sinA, sinA, cosA)) + float2(0.5, 0.5) + AdditiveSpeed + _Additive_ST.zw;
					
					float4 AdditiveTex = SAMPLE_TEXTURE2D(_Additive, sampler_Additive, (NoiseSwitch + AdditivePanner));
					AdditiveColor = AdditiveTex * AdditiveTex.a * _AdditiveColor;
				#endif

				
				/*float2 DissovleSpeed = (float2(_DissolveU_Speed , _DissolveV_Speed));
				float2 uv_Dissolve = IN.ase_texcoord3.xy * _Dissolve_ST.xy + _Dissolve_ST.zw;
				
				float2 DissolvePanner = ( 1.0 * _Time.y * DissovleSpeed + ( uv_Dissolve + UV2.y ));
				float DissolveTex = lerp( SAMPLE_TEXTURE2D( _Dissolve, sampler_Dissolve, ( NoiseSwitch + DissolvePanner ) ).r , BaseTex.a , _DissolveByBaseAlpha);*/
				
				#ifdef _AUTODISSOLVE_ON
					float2 DissovleSpeed = (float2(_DissolveU_Speed, _DissolveV_Speed));
					float2 uv_Dissolve = lerp(IN.ase_texcoord3.xy, IN.ase_texcoord3.zw,_Dissolve2U) * _Dissolve_ST.xy + _Dissolve_ST.zw;

					float2 DissolvePanner = (1.0 * _Time.y * DissovleSpeed + (uv_Dissolve + UV2.y));
					float DissolveTex = lerp(SAMPLE_TEXTURE2D(_Dissolve, sampler_Dissolve, (NoiseSwitch + DissolvePanner)).r, BaseTex.a, _DissolveByBaseAlpha);
					#ifdef _AUTODISSOLVEEDGE_ON
						float DissPreviewWidth = lerp(-1.0, _DissolveWidth, _DissolvePreview+UV2.x);
						float DissWidth = saturate((1.0 - saturate(((DissolveTex * _DissolveWidth) - DissPreviewWidth))));
				
						//float4 DissWidthColor = lerp(DissolveTex, (DissWidth * _DissColor * _dissIntensity), DissWidth);
						ColorLerp = lerp(ColorLerp, (DissWidth * _DissWidthColor * _DissolveWidthIntensity), DissWidth);
						//ColorLerp = _DissWidthColor;
					#endif
				#endif
				float DissPreview = lerp( -1.0 , _EdgeSharpness , ( _DissolvePreview + lerp(UV2.x,0,_ParticleCtrlOff)));
				//float3 (DissPreview).xxx = (DissPreview).xxx;
				#ifdef _AUTODISSOLVE_ON
					float3 DissolveSwitch = saturate( ( ( (DissolveTex).xxx * _EdgeSharpness ) - (DissPreview).xxx ) );
				#else
					float3 DissolveSwitch = float3(1,1,1);
				#endif
				float2 MaskSpeed = (float2(_MaskU_Speed, _MaskV_Speed));
				float2 uv_Mask = IN.ase_texcoord3.xy + _Mask_ST.zw;
				uv_Mask = (_MASKCUS == 1) ? uv_Mask + IN.uv3.zw : uv_Mask ;
				float2 MaskPanner = ( _Time.y * MaskSpeed + 
				// #ifdef _MASKCUS
				// 	uv_Mask + IN.uv3.zw
				// #else
				// 	uv_Mask
				// #endif
				uv_Mask
				);
				float cosM = cos((_MaskRotato * PI));
				float sinM = sin((_MaskRotato * PI));
				MaskPanner = mul(MaskPanner - (float2(0.5, 0.5)+ MaskSpeed+ _Mask_ST.zw), float2x2(cosM, -sinM, sinM, cosM)) + float2(0.5, 0.5)+ MaskSpeed + _Mask_ST.zw;
				#ifdef _AUTOMASK_ON
					float MaskSwitch = SAMPLE_TEXTURE2D( _Mask, sampler_Mask, ( NoiseSwitch + MaskPanner* _Mask_ST.xy) ).r;
				#else
					float MaskSwitch = 1.0;
				#endif
				float3 clampResult147 = clamp( (fresnelalpha*BaseTex.a * _OpacityIntersity * IN.ase_color.a * DissolveSwitch * MaskSwitch ) , float3( 0,0,0 ) , float3( 1,0,0 ) );
				// #ifdef _AUTOVERTEXANI_ON
				// 	float3 Color = ((ColorLerp+ AdditiveColor) ).rgb;
				// #else
				// 	float3 Color = ((ColorLerp + AdditiveColor) * IN.ase_color).rgb;
				// #endif
				float distanceDepth = 1.0;
				//if (_DEPTHFADE == 1)
				//{
//#ifdef _DEPTHFADE
//					float4 screenPos = IN.ase_texcoord5;
//					float4 ase_screenPosNorm = screenPos / screenPos.w;
//					ase_screenPosNorm.z = (UNITY_NEAR_CLIP_VALUE >= 0) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
//
//					float screenDepth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(ase_screenPosNorm.xy), _ZBufferParams);
//					distanceDepth = abs((screenDepth - LinearEyeDepth(ase_screenPosNorm.z, _ZBufferParams)) / _DepthDistance);
//					distanceDepth = saturate(distanceDepth);
//#endif
				//}
				float3 Color = _AUTOVERTEXANI_ON == 1 ? (ColorLerp + AdditiveColor).rgb : ((ColorLerp + AdditiveColor) * IN.ase_color).rgb;
				float Alpha = clampResult147.x* distanceDepth;
				#ifdef _AlphaClip
					clip(Alpha - _AlphaCutoff);
				#endif
				
				float4 particleColor = float4( Color, Alpha );

				#ifdef _DISTORTION_ON
					float4 noise = SAMPLE_TEXTURE2D(_DistorNoiseTexture,sampler_DistorNoiseTexture,uv_Base * _DistorNoiseTexture_ST.xy + _DistorNoiseTexture_ST.zw + fmod(_Time.xx,10) * _DistortionParameter.xy * _DistortionParameter.zw);
					float4 mask = SAMPLE_TEXTURE2D(_DistorMaskTexture,sampler_DistorMaskTexture,( BasePanner + NoiseSwitch ));
					float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
					float distortionIntensity = _DistortionIntensity * 0.1 * mask.r  * IN.ase_texcoord4.y;
                	distortionIntensity = lerp(distortionIntensity * 0.2, distortionIntensity,Alpha);
					float4 screenColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + noise.xx * distortionIntensity);
					screenColor.a *= mask.r * IN.ase_color.a;
					float4 finalColor = AlphaBlend(particleColor,screenColor);
				#else
					float4 finalColor = particleColor;
				#endif
				//return finalColor;
				#if _OUTLINE_ON
				float sdf = finalColor.a ;

				float padding = length(float2(ddx(sdf),ddy(sdf))) * _OutLineWidth;
				float line1px_blocky = 1.0 - step(padding, abs(sdf-_OutLineAlphaThreshold));
				float line2px_smooth = 1.0 - smoothstep(0.0, padding, abs(sdf-_OutLineAlphaThreshold));
				float corners = sqrt(max(0.0, line2px_smooth ));
    			float aa = line1px_blocky + corners;
				finalColor = lerp(finalColor,_OutLineColor,step(0.1,aa));
				//finalColor = ;
				#endif
				#ifdef _CHARACTER_DEPTH_ON
				float4 screenPos = IN.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = (UNITY_NEAR_CLIP_VALUE >= 0) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				#if UNITY_REVERSED_Z
					float depth  = IN.clipPos.z ;
				#else
					float depth  = (IN.clipPos.z ) * 0.5 + 0.5;
				#endif
				float oc =  SAMPLE_TEXTURE2D_SHADOW(_CharacterDepthTexture, sampler_CharacterDepthTexture, float3(screenPos.xy / screenPos.w,depth));
				finalColor.a *= oc;
				#endif
				//return float4(screenDepth,screenDepth,screenDepth,1);
				if(_AlphaPow)
				{
					finalColor.a = pow(finalColor.a,0.5);
				}

				return finalColor;

				

				//return  lerp(finalColor,_OutLineColor,aa);
				//return half4(distanceDepth, distanceDepth, distanceDepth, distanceDepth);

			}
			ENDHLSL
		}


	


		

	}
	//CustomEditor "CustomShaderGUI"
	Fallback "Hidden/InternalErrorShader"
		//Fallback "VertexLit"
}
