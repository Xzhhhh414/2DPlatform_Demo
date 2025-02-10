// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "CRLuo/SpriteEffect"
{
    Properties
    {
		[NoScaleOffset]
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
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
                fixed4 baseColor = tex2D(_MainTex, i.uv);

                fixed4 finalColor = baseColor;

                //clip(finalColor.a - 0.5);
				return finalColor;
            }
            ENDCG
        }
    }
}