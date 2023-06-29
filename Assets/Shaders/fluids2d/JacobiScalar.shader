Shader "Custom/JacobiScalar" {
    Properties {
        _X ("X", 2D) = "white" {}
        _B ("B", 2D) = "white" {}
        _GridSize ("Grid Size", Vector) = (1,1,1,1)
        _Alpha ("Alpha", Float) = 1.0
        _Beta ("Beta", Float) = 1.0
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

            sampler2D _X;
            sampler2D _B;
            float2 _GridSize;
            float _Alpha;
            float _Beta;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 xOffset = float2(1.0 / _GridSize.x, 0.0);
                float2 yOffset = float2(0.0, 1.0 / _GridSize.y);

                float xl = tex2D(_X, i.uv - xOffset).x;
                float xr = tex2D(_X, i.uv + xOffset).x;
                float xb = tex2D(_X, i.uv - yOffset).x;
                float xt = tex2D(_X, i.uv + yOffset).x;

                float bc = tex2D(_B, i.uv).x;

                return fixed4((xl + xr + xb + xt + _Alpha * bc) / _Beta, 0.0, 0.0, 1.0);
            }
            ENDCG
        }
    }
}
