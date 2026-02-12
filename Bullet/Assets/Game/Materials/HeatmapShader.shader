Shader "Custom/HeatmapShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [Range(0.01,2.0)] _HeatRadius("Heat Radius", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define MAX_HITS 32

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                half _HeatRadius;
            CBUFFER_END

            float _Hits[128]; // 32 * 4 (x, y, z, intensity)
            int _HitCount;

            static const half4 colors[5] =
            {
                half4(0,0,0,0),
                half4(0,0.9,0.2,1),
                half4(0.9,1,0.3,1),
                half4(0.9,0.7,0.1,1),
                half4(1,0,0,1)
            };

            static const half pointranges[5] =
            {
                0.0, 0.25, 0.5, 0.75, 1.0
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half distsq(float3 pixelPos, float3 heatPoint)
            {
                half d = distance(pixelPos, heatPoint) / _HeatRadius;
                d = saturate(1.0h - d);
                return d * d;
            }

            half3 getHeatForPixel(half weight)
            {
                if (weight <= pointranges[0]) return colors[0].rgb;
                if (weight >= pointranges[4]) return colors[4].rgb;

                for (int i = 1; i < 5; i++)
                {
                    if (weight < pointranges[i])
                    {
                        half t = (weight - pointranges[i - 1]) / (pointranges[i] - pointranges[i - 1]);
                        return lerp(colors[i - 1].rgb, colors[i].rgb, t);
                    }
                }

                return colors[0].rgb;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

                float3 worldPos = IN.positionWS;
                half totalWeight = 0.0h;

                for (int i = 0; i < MAX_HITS; i++)
                {
                    if (i >= _HitCount) break;

                    float3 heatPoint = float3(
                        _Hits[i * 4],
                        _Hits[i * 4 + 1],
                        _Hits[i * 4 + 2]
                    );
                    half intensity = _Hits[i * 4 + 3];

                    totalWeight += distsq(worldPos, heatPoint) * intensity;
                }

                half3 heat = getHeatForPixel(totalWeight);

                return col + half4(heat, 0.5);
            }
            ENDHLSL
        }
    }
}