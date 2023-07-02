Shader "Custom/XYColor" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _InteractionPoint ("InteractionPoint", Vector) = (0,0,0)
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
                float4 screenPos : SV_POSITION;
                float3 vertex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _InteractionPoint;

            v2f vert (appdata v) {
                v2f o;
                o.screenPos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.vertex = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.r = i.uv.y;
                col.g = i.uv.x;
                col.b = 0;

                float dist = distance(_InteractionPoint.xy, i.uv.xy);
                float radius = 0.25;
                if(dist < radius) {
                    col.b = (radius - dist) * 10;
                }
                return col;
            }
            ENDCG
        }
    } 
}
