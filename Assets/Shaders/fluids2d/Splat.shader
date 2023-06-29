Shader "Custom/Splat" {
    Properties {
        _Read ("Read", 2D) = "white" {}
        _GridSize ("Grid Size", Vector) = (1,1,1,1)
        _Color ("Color", Vector) = (1,1,1,1)
        _Point ("Point", Vector) = (1,1,1,1)
        _Radius ("Radius", Float) = 1.0
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

            sampler2D _Read;
            float2 _GridSize;
            float3 _Color;
            float2 _Point;
            float _Radius;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float gauss(float2 p, float r) {
                return exp(-dot(p, p) /  r);
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 base = tex2D(_Read, i.uv).xyz;
                float2 coord = _Point.xy - i.vertex.xy;
                float3 splat = _Color * gauss(coord, _GridSize.x * _Radius);

                return fixed4(base + splat, 1.0);
            }
            ENDCG
        }
    }
}
