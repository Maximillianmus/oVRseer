Shader "Custom/DoorLightShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Emission ("Emission", Color) = (0,0,0,1)
        
        _FresnelEffectColor ("Fresnel Effect Color", Color) = (1,1,1,1)
        [PowerSlider(4)] _FresnelEffectExponent ("Fresnel Exponent", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags {"RenderType"="Transparent" "Queue"="Transparent" "ForceNoShadowCasting" = "True"}
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        half3 _Emission;
        fixed4 _Color;
        float3 _FresnelEffectColor;
        float _FresnelEffectExponent;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 Normal = WorldNormalVector(IN, o.Normal);
            float3 viewDirection = IN.viewDir;

            float dotProduct = dot(Normal, viewDirection);
            dotProduct = saturate(1 - dotProduct);
            dotProduct = pow(dotProduct, _FresnelEffectExponent);
            float3 fresnelColor = dotProduct * _FresnelEffectColor;

            o.Emission = _Color + fresnelColor;

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
