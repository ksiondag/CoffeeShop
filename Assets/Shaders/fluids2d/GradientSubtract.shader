Shader "Custom/GradientSubtract" {
    Properties {
        _PressureTex ("Pressure Texture", 2D) = "white" {}
        _VelocityTex ("Velocity Texture", 2D) = "white" {}
        _TexelSize ("Texel Size", Vector) = (512, 512, 0, 0)
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
                float2 uvL : TEXCOORD1;
                float2 uvR : TEXCOORD2;
                float2 uvT : TEXCOORD3;
                float2 uvB : TEXCOORD4;
                float4 vertex : SV_POSITION;
            };

            sampler2D _PressureTex;
            sampler2D _VelocityTex;
            float4 _TexelSize;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uvL = v.uv + float2(-_TexelSize.x, 0);
                o.uvR = v.uv + float2(_TexelSize.x, 0);
                o.uvT = v.uv + float2(0, _TexelSize.y);
                o.uvB = v.uv + float2(0, -_TexelSize.y);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float L = tex2D(_PressureTex, i.uvL).r;
                float R = tex2D(_PressureTex, i.uvR).r;
                float T = tex2D(_PressureTex, i.uvT).r;
                float B = tex2D(_PressureTex, i.uvB).r;
                float2 velocity = tex2D(_VelocityTex, i.uv).rg;
                velocity -= float2(R - L, T - B);
                return fixed4(velocity, 0.0, 1.0);
            }
            ENDCG
        }
    } 
}
