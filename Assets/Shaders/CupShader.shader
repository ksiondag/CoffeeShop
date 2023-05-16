Shader "Custom/CupShader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _CupColor("Cup Color", Color) = (1, 1, 1, 1)
        _FillPlaneY("Fill Plane Y", Range(0, 1)) = 0.5
    }
    /*
    SubShader{
        // This is the fill shader
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass {
            ColorMask 0 // don't write any color data
            ZWrite On   // write to the depth buffer

            Stencil {
                Ref 1
                Comp always
                Pass replace
            }
        }
    }
    */
    SubShader{
        // This is a standard shader
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert
        fixed4 _CupColor;

        sampler2D _MainTex;
        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = _CupColor.rgb;
            o.Alpha = _CupColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
