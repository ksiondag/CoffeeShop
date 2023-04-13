Shader "Custom/FillShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _FillColor ("Fill Color", Color) = (0, 0, 1, 1)
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.5
        _LocalMin ("Local Min", Vector) = (0, 0, 0, 1)
        _LocalMax ("Local Max", Vector) = (1, 1, 1, 1)
    }

    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass {
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _FillColor;
                float _FillAmount;
                float2 _LocalBounds;
                float4 _LocalMin;
                float4 _LocalMax;

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 localPos : TEXCOORD1;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                fixed4 frag(v2f IN) : SV_Target {
                    float3 albedo = tex2D(_MainTex, IN.uv).rgb;
                    float worldY = IN.worldPos.y;

                    // Debug worldY with different colors
                    // if (worldY < 0.75) {
                    //     return fixed4(1, 1, 0, 1); // Yellow
                    // } else if (worldY < 1) {
                    //     return fixed4(0, 0, 1, 1); // Blue
                    // } else if (worldY < 1.25) {
                    //     return fixed4(0, 1, 0, 1); // Green
                    // } else {
                    //     return fixed4(1, 0, 0, 1); // Red
                    // }

                    // Fill the mesh from bottom to top by percentage of the mesh's height up to _FillAmount
                    float fillThreshold = lerp(
                        mul(unity_ObjectToWorld, _LocalMin).xyz.y,
                        mul(unity_ObjectToWorld, _LocalMax).xyz.y, 
                        _FillAmount);

                    if (worldY <= fillThreshold) {
                        albedo = _FillColor.rgb;
                    }

                    return fixed4(albedo, 1.0);
                }
            ENDHLSL
        }
    }
}
