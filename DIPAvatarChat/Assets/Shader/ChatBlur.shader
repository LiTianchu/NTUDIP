Shader "Unlit/ChatBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(1, 10)) = 3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
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

            sampler2D _MainTex;
            float _BlurSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                for (int x = -_BlurSize; x <= _BlurSize; x++)
                {
                    for (int y = -_BlurSize; y <= _BlurSize; y++)
                    {
                        col += tex2D(_MainTex, i.uv + float2(x, y) / _ScreenParams.xy);
                    }
                }
                col /= ((_BlurSize * 2 + 1) * (_BlurSize * 2 + 1));
                return col;
            }
            ENDCG
        }
    }
}