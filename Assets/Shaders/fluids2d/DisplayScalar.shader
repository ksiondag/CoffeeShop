Shader "Custom/DisplayScalar" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Bias ("Bias", Vector) = (0,0,0,0)
        _Scale ("Scale", Vector) = (1,1,1,1)
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
            float3 _Bias;
            float3 _Scale;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float scalar = tex2D(_MainTex, i.vertex).r; // use only the red channel as the scalar
                fixed4 col = fixed4(_Bias + _Scale * scalar.xxx, 1.0);
                return col;
            }
            ENDCG
        }
    }
}
