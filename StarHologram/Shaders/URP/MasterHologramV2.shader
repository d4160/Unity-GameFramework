Shader "StarHologram/URP/Hologram Master"
{
    Properties
    {
        [Header(Hologram Main Settings)]
        [Toggle(_TRANSPARENT_MODE_ON)] _TransparentMode("Enable Transparency", Float) = 0.0
        [Space(10)]
        _OverallTransparency("Overall Transparency", Range(0.0, 1.0)) = 0.75

        _CoreColor("Core Color", Color) = (0.13, 0.29, 0.53, 1)
        _EdgeColor("Edge Glow", Color) = (0.5, 0.9, 1, 1)
        _BlazeColor("Outer Halo", Color) = (0.92, 0.98, 1, 1)

        [Header(Glow and Fresnel Controls)]
        _GlowStrength("Internal Glow", Range(0, 10)) = 2.1
        _BlazeStrength("Outer Glow", Range(0, 10)) = 2.7
        _FresnelPower("Fresnel Edge", Range(0.1, 25)) = 8.5
        _FresnelOuter("Fresnel Outer", Range(0.1, 25)) = 8.5
        _CoreDensity("Core Intensity", Range(0, 1)) = 0.21
        _HaloRange("Halo Spread", Range(0.1, 2)) = 1.0

        [Header(Scanline and Glitch Effects)]
        _ScanSpeed("Scan Speed", Range(0, 10)) = 1.5
        _ScanDirection("Scan Direction", Range(-1, 1)) = 1.0
        _ScanFreq("Scanline Freq", Range(1, 200)) = 55
        _DiagIntensity("Diag Intensity", Range(0, 1)) = 0.07
        
        [Header(Animation and Noise)]
        _WaveAnim("Wave Speed", Range(0, 3)) = 1.0
        _PulseAmount("Core Pulse", Range(0, 1)) = 0.14
        _JitterAmount("Subtle Jitter", Range(0, 0.03)) = 0.009
        _NoiseDetail("Micro Noise", Range(0, 0.22)) = 0.07
        _TimeJump("Time Offset", Range(0, 10)) = 2.7
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 300
        
        Pass
        {
            Cull Back
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local _TRANSPARENT_MODE_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _CoreColor, _EdgeColor, _BlazeColor;
                float _GlowStrength, _BlazeStrength, _FresnelPower, _FresnelOuter, _HaloRange;
                float _CoreDensity, _ScanSpeed, _ScanFreq, _TimeJump;
                float _DiagIntensity, _WaveAnim, _PulseAmount, _JitterAmount, _NoiseDetail;
                float _ScanDirection;
                float _OverallTransparency;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS     : SV_POSITION;
                float3 worldNormal    : TEXCOORD0;
                float3 worldPos       : TEXCOORD1;
                float3 viewDir        : TEXCOORD2;
            };

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(sin(p.x * 67.11 + p.y * 13.2) * 971.76);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDir = normalize(_WorldSpaceCameraPos - OUT.worldPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float T = _Time.y + _TimeJump;
                float ndv = dot(normalize(IN.worldNormal), normalize(IN.viewDir));
                float fresnel = pow(1.0 - saturate(ndv), _FresnelPower);
                float fresnelOuter = pow(1.0 - saturate(ndv), _FresnelOuter * _HaloRange);
                float rad = length(IN.worldNormal.xy * 0.85);
                float corePulse = (1.0 - _PulseAmount) + _PulseAmount * sin(T * _WaveAnim + rad * 7.1);
                corePulse = saturate(corePulse);
                half3 core = _CoreColor.rgb * corePulse * _CoreDensity * (0.9 + fresnel * 0.19);
                float2 jitter = float2(sin(IN.worldPos.y * 8.11 + T * 0.81) * _JitterAmount, cos(IN.worldPos.x * 9.17 - T * 0.41) * _JitterAmount);
                float scanPhase = frac((IN.worldPos.y + jitter.y) * _ScanFreq + T * _ScanSpeed * _ScanDirection + sin(IN.worldPos.x * 2.1) * 0.31);
                float scan = lerp(0.89, 1.12, pow(sin(scanPhase * PI), 2.4));
                scan *= (0.97 + 0.03 * sin(T * 0.11));
                float diagLine = _DiagIntensity * pow(sin((IN.worldPos.x + jitter.x) * 2.11 + IN.worldPos.y * 2.13 + T * 0.4), 2);
                half3 edgeGlow = fresnel * _EdgeColor.rgb * _GlowStrength;
                half3 outerGlow = fresnelOuter * _BlazeColor.rgb * _BlazeStrength;
                float blendFactor = saturate(scan * 0.58 + diagLine);
                half3 finalCol = lerp(core, edgeGlow + outerGlow, blendFactor);
                float flick = lerp(0.97, 1.07, Hash21(IN.worldPos.xz * 4.7 + T * 1.3 + float2(corePulse, scan)) * _NoiseDetail);
                finalCol *= flick;

                half finalAlpha;

                #if defined(_TRANSPARENT_MODE_ON)
                    float luminance = dot(finalCol, half3(0.299, 0.587, 0.114));
                    finalAlpha = saturate(luminance + fresnel * 0.5) * _OverallTransparency;
                #else
                    finalAlpha = 1.0;
                #endif

                return half4(saturate(finalCol), finalAlpha);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Pipeline/FallbackError"
}