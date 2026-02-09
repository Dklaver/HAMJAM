Shader "Custom/HeatmapShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [Range(0.01,0.5)] _HeatRadius("Heat Radius", Float) = 0.1
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
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                half _HeatRadius; // New radius property
            CBUFFER_END

            // Data from C#
            float _Hits[MAX_HITS * 3];
            int _HitCount;

            // Compile-time constants (WebGL safe)
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
                return OUT;
            }

            // Distance-based falloff using radius
            half distsq(float2 a, float2 b)
            {
                half d = distance(a, b) / _HeatRadius; // Normalize distance by radius
                d = saturate(1.0h - d);                // Clamp to 0–1
                return d * d;                          // Smooth falloff
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

                float2 uv = IN.uv;

                half totalWeight = 0.0h;

                for (int i = 0; i < MAX_HITS; i++)
                {
                    if (i >= _HitCount) break;

                    float2 p = float2(_Hits[i * 3], _Hits[i * 3 + 1]);
                    half intensity = _Hits[i * 3 + 2];

                    totalWeight += distsq(uv, p) * intensity;
                }

                half3 heat = getHeatForPixel(totalWeight);

                return col + half4(heat, 0.5);
            }
            ENDHLSL
        }
    }
}
