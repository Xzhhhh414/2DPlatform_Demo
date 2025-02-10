Shader "Custom/Checkerboard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CheckerColor1 ("Checker Color 1", Color) = (0.8, 0.8, 0.8, 1)
        _CheckerColor2 ("Checker Color 2", Color) = (0.65, 0.65, 0.65, 1)
        _CheckerSize ("Checker Size", Float) = 16
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }
        Pass
        {
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _CheckerColor1;
            float4 _CheckerColor2;
            float _CheckerSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样主纹理的颜色
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = pow(col.rgb,1/2.2);

                // 检查是否是透明像素
                if (col.a < 0.5)
                {
                    // 计算棋盘格的颜色
                    float checkerX = floor(i.uv.x * _CheckerSize) % 2;
                    float checkerY = floor(i.uv.y * _CheckerSize) % 2;
                    bool isCheckerWhite = (checkerX + checkerY) % 2 == 0;
                    col.rgb = isCheckerWhite ? _CheckerColor1.rgb : _CheckerColor2.rgb;
                    col.a = 1.0; // 棋盘格背景为不透明
                }

                return col;
            }
            ENDCG
        }
    }
}