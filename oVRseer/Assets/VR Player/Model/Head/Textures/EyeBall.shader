Shader "Unlit/EyeBall"
{
    Properties
    {
        [HDR]_MainColor ("Ball Color", color) = (1,1,1,1)
        [HDR]_SecondaryColor ("Eye Color", color) = (0,0,0,0)
        [HDR]_ThirdColor ("Pupil Color", color) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _Blend("Amount of a color", float) = 0
        _EyeBlendVelocityMultiplier("Speed of color change", float) = 0.5
        _PupilBlendVelocityMultiplier("Speed of Pupil color change", float) = 0.5
        _PupilTimeChangeMultiplier("Period Multiplier", float) = 1
        _PupilMin("minmum size of pupil", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100



        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 localPos : POSITION1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _MainColor;
            half4 _SecondaryColor;
            float _Blend;
            half4 _ThirdColor;

            float _EyeBlendVelocityMultiplier;
            float _PupilBlendVelocityMultiplier;
            float _PupilTimeChangeMultiplier;
            float _PupilMin;

  
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.localPos = v.vertex.z;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float interpolationValue = i.localPos.z * _Blend*_EyeBlendVelocityMultiplier;

                interpolationValue = min(max(interpolationValue,0),1);
                fixed4 col = lerp(_MainColor,_SecondaryColor, interpolationValue );

                interpolationValue =  i.localPos.z * _Blend * _PupilBlendVelocityMultiplier;


                col = lerp(col, _ThirdColor, interpolationValue * _PupilMin);

                interpolationValue = interpolationValue/1.5 + (abs(sin(_Time*_PupilTimeChangeMultiplier))+1.7)  * interpolationValue/2 - 1;
                

                interpolationValue = min(max(interpolationValue,0),1);

                col = lerp(col, _ThirdColor, interpolationValue);

           
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
