Shader "Custom/Advection" {
    Properties {
        _VelocityTex ("Velocity Texture", 2D) = "white" {}
        _SourceTex ("Source Texture", 2D) = "white" {}
        _TexelSize ("Texel Size", Vector) = (512, 512, 0, 0)
        _DyeTexelSize ("Dye Texel Size", Vector) = (512, 512, 0, 0)
        _DeltaTime ("Delta Time", Float) = 0
        _Dissipation ("Dissipation", Float) = 0
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

            sampler2D _VelocityTex;
            sampler2D _SourceTex;
            float4 _TexelSize;
            float4 _DyeTexelSize;
            float _DeltaTime;
            float _Dissipation;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 bilerp (sampler2D sam, float2 uv, float2 tsize) {
                float2 st = uv / tsize - 0.5;

                float2 iuv = floor(st);
                float2 fuv = frac(st);

                fixed4 a = tex2D(sam, (iuv + float2(0.5, 0.5)) * tsize);
                fixed4 b = tex2D(sam, (iuv + float2(1.5, 0.5)) * tsize);
                fixed4 c = tex2D(sam, (iuv + float2(0.5, 1.5)) * tsize);
                fixed4 d = tex2D(sam, (iuv + float2(1.5, 1.5)) * tsize);

                return lerp(lerp(a, b, fuv.x), lerp(c, d, fuv.x), fuv.y);
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 coord = i.uv - _DeltaTime * bilerp(_VelocityTex, i.uv, _TexelSize.xy).xy * _TexelSize.xy;
                fixed4 result = bilerp(_SourceTex, coord, _DyeTexelSize.xy);
                float decay = 1.0 + _Dissipation * _DeltaTime;
                return result / decay;
            }
            ENDCG
        }
    } 
}
