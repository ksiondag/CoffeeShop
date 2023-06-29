Shader "Custom/Boundary" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _GridSize ("Grid Size", Vector) = (1,1,0,0)
        _GridOffset ("Grid Offset", Vector) = (0,0,0,0)
        _Scale ("Scale", Float) = 1.0
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

            sampler2D _MainTex;
            float2 _GridSize;
            float2 _GridOffset;
            float _Scale;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uv = (i.vertex.xy + _GridOffset) / _GridSize;
                return fixed4(_Scale * tex2D(_MainTex, uv).xyz, 1.0);
            }
            ENDCG
        }
    }
}
