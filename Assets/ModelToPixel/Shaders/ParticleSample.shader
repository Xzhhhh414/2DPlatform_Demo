Shader "Smash_Client/Effect/ParticleSample"
{
	Properties
	{
		[HideInInspector] _BlendMode("BlendMode", Float) = 1
		[Enum(UnityEngine.Rendering.CullMode)][HideInInspector] _RenderFace("RenderFace", Float) = 2
		[Enum(UnityEngine.Rendering.CompareFunction)][HideInInspector]_ZTest("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.BlendMode)][HideInInspector]_SrcBlend("SrcBlend", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)][HideInInspector]_DstBlend("DstBlend", Float) = 10
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_Base("Base", 2D) = "white" {}
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_BaseU_Speed("BaseU_Speed", Float) = 0
		_BaseV_Speed("BaseV_Speed", Float) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		
		Pass
		{
			
			Name "Forward"
			
			Blend[_SrcBlend][_DstBlend]

			// Cull[_RenderFace]

			// ZWrite[_ZWrite]
			Cull Off

			ZWrite Off


			ZTest[_ZTest]


			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#pragma multi_compile_instancing

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _AlphaClip

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			uniform float _AlphaPow;


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Base_ST;
			float4 _Color;
			float _BaseU_Speed;
			float _BaseV_Speed;
			CBUFFER_END
			TEXTURE2D(_Base);
			SAMPLER(sampler_Base);


			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				o.clipPos = positionCS;
				return o;
			}

			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 appendResult44 = (float2(_BaseU_Speed , _BaseV_Speed));
				float2 uv_Base = IN.ase_texcoord3.xy * _Base_ST.xy + _Base_ST.zw;
				float2 panner33 = ( 1.0 * _Time.y * appendResult44 + uv_Base);
				float4 tex2DNode13 = SAMPLE_TEXTURE2D( _Base, sampler_Base, panner33 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( tex2DNode13 * _Color * IN.ase_color ).rgb;
				float Alpha = ( tex2DNode13.a * _Color.a * IN.ase_color.a );
				// float AlphaClipThreshold = 0.5;
				// float AlphaClipThresholdShadow = 0.5;

				if(_AlphaPow)
				{
					Alpha = pow(Alpha,0.5);
				}

				return half4( Color, Alpha );
				
			}

			ENDHLSL
		}

	}
	CustomEditor "CustomShaderGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
