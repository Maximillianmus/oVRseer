Shader "Unlit/Lava"
{
    Properties
    {

        _TestThreshhold ("Test Threshhold", Range(0, 1)) = 0

        [Header(Maps)]
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseMap ("Noise Map", 2D) = "bump" {}
        _NoiseMapStrength("Noise Map Strength", float) = 0
        _NoiseMapSpeedX("Noise Map Speed X", float) = 0
        _NoiseMapSpeedZ("Noise Map Speed Z", float) = 0

        [Header(Color)]
        [HDR] _BaseColor("Base Color", color) = (1,1,1,1)
        _BaseColorStrength("Base Color Strength", Range(0, 2)) = 1
        [HDR] _PrimaryColor("Primary Color", color) = (1,1,1,1)
        _PrimaryColorStrength("Primary Color Strength", Range(0,2))  = 1
        [HDR] _HighlightColor("Highlight Color", color) = (1,1,1,1)
        _HighlightColorStrength("Highlight Color Strength", Range(0,2)) = 1

        [Header(Voronoi Noise)]
        _CellSize ("Cell Size", Range(0.5,5)) = 2

        _VoronoiTimeScale("Voronoi Time Scale", float) = 0
        _VoronoiPanSpeedX("Voronoi Pan Speed X", float) = 0
        _VoronoiPanSpeedZ("Voronoi Pan Speed Z", float) = 0

        [Header(Wave)]
        _WaveHeight("Wave Height", float) = 0
        _WaveSpeed("Wave Speed", float) = 0
        _WaveLength("Wave Length", float) = 1

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

            // Vert to frag. Contains info for frag function
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 worldPos : POSITION1;
            };

            float _TestThreshhold;

            // texture
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;
            float _NoiseMapStrength;
            float _NoiseMapSpeedX;
            float _NoiseMapSpeedZ;
            
            // Colors
            float4 _BaseColor;
            float _BaseColorStrength;
            float4 _PrimaryColor;
            float _PrimaryColorStrength;
            float4 _HighlightColor;
            float _HighlightColorStrength;

            // Voronoi
            float _CellSize;
            float _VoronoiTimeScale;
            float _VoronoiPanSpeedX;
            float _VoronoiPanSpeedZ;

            // Wave
            float _WaveHeight;
            float _WaveSpeed;
            float _WaveLength;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                v.vertex.y += sin((_Time.z * _WaveSpeed + o.worldPos.x * o.worldPos.z) * _WaveLength) * _WaveHeight;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            // all of these inspired by ronja-tutorials white-noise
            float rand2dTo1d(float2 value)
            {
                float2 dotDir = float2(5.989, 45.233);
                float2 smallValue = sin(value);
                float random = dot(smallValue, dotDir);
                random = frac(sin(random) * 143758.5453);
                return random;
            }

            float2 rand2dTo2d(float2 value) {
                
                float2 dotDir1 = float2(12.989, 78.233);
                float2 dotDir2 = float2(39.346, 11.135);

                float2 smallValue = sin(value);

                float random1 = dot(smallValue, dotDir1);
                float random2 = dot(smallValue, dotDir2);
                random1 = frac(sin(random1) * 143758.5453);
                random2 = frac(sin(random2) * 143758.5453);
                return float2(random1, random2);
            }
            
            float rand3dTo1d(float3 value)
            {
                float3 dotDir = float3(12.989, 78.233, 37.719);
                float3 smallValue = sin(value);
                float random = dot(smallValue, dotDir);
                random = frac(sin(random) * 143758.5453);
                return random;
            }

            float3 rand3dTo3d(float3 value)
            {
                float3 dotDir1 = float3(12.989, 78.233, 37.719);
                float3 dotDir2 = float3(39.346, 11.135, 83.155);
                float3 dotDir3 = float3(73.156, 52.235, 09.151);

                //make value smaller to avoid artefacts
                float3 smallValue = sin(value);
                //get scalar value from 3d vector
                float random1 = dot(smallValue, dotDir1);
                float random2 = dot(smallValue, dotDir2);
                float random3 = dot(smallValue, dotDir3);

                //make value more random by making it bigger and then taking the factional part
                random1 = frac(sin(random1) * 143758.5453);
                random2 = frac(sin(random2) * 143758.5453);
                random3 = frac(sin(random3) * 143758.5453);
                return float3(random1, random2, random3);
            }

            // from ronja-tutorial voronoi noise
            float3 get2DVoronoiNoise(float2 value)
            {
                float2 baseCell = floor(value);

                //first pass to find the closest cell
                float minDistToCell = 10;
                float2 toClosestCell;
                float2 closestCell;
                [unroll]
                for (int x1 = -1; x1 <= 1; x1++) {
                    [unroll]
                    for (int y1 = -1; y1 <= 1; y1++) {
                        float2 cell = baseCell + float2(x1, y1);
                        float2 cellPosition = cell + rand2dTo2d(cell);
                        float2 toCell = cellPosition - value;
                        float distToCell = length(toCell);
                        if (distToCell < minDistToCell) {
                            minDistToCell = distToCell;
                            closestCell = cell;
                            toClosestCell = toCell;
                        }
                    }
                }

                //second pass to find the distance to the closest edge
                float minEdgeDistance = 10;
                [unroll]
                for (int x2 = -1; x2 <= 1; x2++) {
                    [unroll]
                    for (int y2 = -1; y2 <= 1; y2++) {
                        float2 cell = baseCell + float2(x2, y2);
                        float2 cellPosition = cell + rand2dTo2d(cell);
                        float2 toCell = cellPosition - value;

                        float2 diffToClosestCell = abs(closestCell - cell);
                        bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y < 0.1;
                        if (!isClosestCell) {
                            float2 toCenter = (toClosestCell + toCell) * 0.5;
                            float2 cellDifference = normalize(toCell - toClosestCell);
                            float edgeDistance = dot(toCenter, cellDifference);
                            minEdgeDistance = min(minEdgeDistance, edgeDistance);
                        }
                    }
                }

                float random = rand2dTo1d(closestCell);
                return float3(minDistToCell, random, minEdgeDistance);
            }
            
            // from ronja-tutorial voronoi noise
            float3 get3DVoronoiNoise(float3 value) {
                float3 baseCell = floor(value);

                //first pass to find the closest cell
                float minDistToCell = 10;
                float3 toClosestCell;
                float3 closestCell;
                [unroll]
                for (int x1 = -1; x1 <= 1; x1++) {
                    [unroll]
                    for (int y1 = -1; y1 <= 1; y1++) {
                        [unroll]
                        for (int z1 = -1; z1 <= 1; z1++) {
                            float3 cell = baseCell + float3(x1, y1, z1);
                            float3 cellPosition = cell + rand3dTo3d(cell);
                            float3 toCell = cellPosition - value;
                            float distToCell = length(toCell);
                            if (distToCell < minDistToCell) {
                                minDistToCell = distToCell;
                                closestCell = cell;
                                toClosestCell = toCell;
                            }
                        }
                    }
                }

                //second pass to find the distance to the closest edge
                float minEdgeDistance = 10;
                [unroll]
                for (int x2 = -1; x2 <= 1; x2++) {
                    [unroll]
                    for (int y2 = -1; y2 <= 1; y2++) {
                        [unroll]
                        for (int z2 = -1; z2 <= 1; z2++) {
                            float3 cell = baseCell + float3(x2, y2, z2);
                            float3 cellPosition = cell + rand3dTo3d(cell);
                            float3 toCell = cellPosition - value;

                            float3 diffToClosestCell = abs(closestCell - cell);
                            bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y + diffToClosestCell.z < 0.1;
                            if (!isClosestCell) {
                                float3 toCenter = (toClosestCell + toCell) * 0.5;
                                float3 cellDifference = normalize(toCell - toClosestCell);
                                float edgeDistance = dot(toCenter, cellDifference);
                                minEdgeDistance = min(minEdgeDistance, edgeDistance);
                            }
                        }
                    }
                }

                float random = rand3dTo1d(closestCell);
                return float3(minDistToCell, random, minEdgeDistance);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 value = i.worldPos.xyz / _CellSize;

                // Apply time
                value.y += _Time.y * _VoronoiTimeScale;
                value.x += _Time.x * _VoronoiPanSpeedX;
                value.z += _Time.z * _VoronoiPanSpeedZ;

                float3 voronoiNoise = get3DVoronoiNoise(value);

                float4 col = _BaseColor * (_BaseColorStrength);
                col += voronoiNoise.z *_PrimaryColor* _PrimaryColorStrength;
                col += voronoiNoise.x *_HighlightColor* _HighlightColorStrength;

                float2 newUV = float2(
                    (i.uv.x) / _NoiseMap_ST.x + _NoiseMap_ST.z - _Time.x * _NoiseMapSpeedX,
                    (i.uv.y) / _NoiseMap_ST.y + _NoiseMap_ST.w - _Time.z * _NoiseMapSpeedZ
                );
                float noiseMap = tex2D(_NoiseMap, newUV);
                float val = lerp(0.2, 0.8, noiseMap) * _NoiseMapStrength;
                col -= val;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}