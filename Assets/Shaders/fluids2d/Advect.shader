Shader "Custom/Advect" {
    Properties {
        _Velocity ("Velocity", 2D) = "white" {}
        _Advected ("Advected", 2D) = "white" {}
        _GridSize ("GridSize", Vector) = (1,1,0,0)
        _GridScale ("GridScale", Float) = 1
        _TimeStep ("TimeStep", Float) = 1
        _Dissipation ("Dissipation", Float) = 1
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

            sampler2D _Velocity;
            sampler2D _Advected;
            float2 _GridSize;
            float _GridScale;
            float _TimeStep;
            float _Dissipation;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 bilerp (sampler2D d, float2 p) {
                float4 ij;
                ij.xy = floor(p - 0.5) + 0.5;
                ij.zw = ij.xy + 1.0;

                float4 uv = ij / float4(_GridSize.xy, _GridSize.xy);
                float2 d11 = tex2D(d, uv.xy).xy;
                float2 d21 = tex2D(d, uv.zy).xy;
                float2 d12 = tex2D(d, uv.xw).xy;
                float2 d22 = tex2D(d, uv.zw).xy;

                float2 a = p - ij.xy;

                return lerp(lerp(d11, d21, a.x), lerp(d12, d22, a.x), a.y);
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.vertex.xy / _GridSize.xy;
                float scale = 1.0 / _GridScale;

                // trace point back in time
                float2 p = uv * _GridSize.xy - _TimeStep * scale * tex2D(_Velocity, uv).xy;

                return fixed4(_Dissipation * bilerp(_Advected, p), 0.0, 1.0);
            }
            ENDCG
        }
    }
}
