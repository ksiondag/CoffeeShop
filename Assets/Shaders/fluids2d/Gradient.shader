Shader "Custom/Gradient" {
    Properties {
        _P ("P", 2D) = "white" {}
        _W ("W", 2D) = "white" {}
        _GridSize ("Grid Size", Vector) = (1,1,1,1)
        _GridScale ("Grid Scale", Float) = 1.0
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _P;
            sampler2D _W;
            float2 _GridSize;
            float _GridScale;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 xOffset = float2(1.0 / _GridSize.x, 0.0);
                float2 yOffset = float2(0.0, 1.0 / _GridSize.y);

                float pl = tex2D(_P, i.uv - xOffset).x;
                float pr = tex2D(_P, i.uv + xOffset).x;
                float pb = tex2D(_P, i.uv - yOffset).y;
                float pt = tex2D(_P, i.uv + yOffset).y;

                float scale = 0.5 / _GridScale;
                float gradient = scale * (pr - pl + pt - pb);

                float2 wc = tex2D(_W, i.uv).xy;

                return fixed4(wc - gradient, 0.0, 1.0);
            }
            ENDCG
        }
    }
}
