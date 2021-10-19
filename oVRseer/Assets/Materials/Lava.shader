Shader "Custom/Lava"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _PrimaryColor("PrimaryColor", Color) = (0.8, 0, 0, 1)
        //_NormalMap("NormalMap", 2D) 
        _Height("Height", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        struct Input 
        {
            float2 uv_MainTex;
        };

        // Distorts vertexes in y
        float _Height;
        void vert(inout appdata_full v)
        {
            v.vertex.y += sin(_Time.z + v.vertex.x * v.vertex.z)*_Height + _Height;
        }

        // Set color
        sampler2D _MainTex;
        float4 _PrimaryColor;
        void surf(Input IN, inout SurfaceOutput o) 
        {
            float4 resColor = _PrimaryColor;

            float4 a = tex2D(_MainTex, IN.uv_MainTex).rgb;


            o.Albedo = resColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
