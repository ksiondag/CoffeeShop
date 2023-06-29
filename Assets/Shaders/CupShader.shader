Shader "Custom/CupShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _CutoffHeight ("Cutoff Height", Range(0, 2)) = 1.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        sampler2D _MainTex;
        fixed4 _Color;
        float _CutoffHeight;

        void surf (Input IN, inout SurfaceOutput o)
        {
            // If the world position is above the cutoff height, discard this fragment
            if (IN.worldPos.y > _CutoffHeight) {
                discard;
            }

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}