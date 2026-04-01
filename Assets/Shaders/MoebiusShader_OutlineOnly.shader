Shader "Cosmic/Moebius Outline Only"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.01
        _WiggleFrequency ("Wiggle Frequency", Range(0.5, 10.0)) = 3.0
        _WiggleAmplitude ("Wiggle Amplitude", Range(0.0, 0.02)) = 0.005
        _EdgeSoftness ("Edge Softness", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        // Outline pass
        Pass
        {
            Name "OUTLINE"
            Cull Front
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            float4 _OutlineColor;
            float _OutlineWidth;
            float _WiggleFrequency;
            float _WiggleAmplitude;
            float _EdgeSoftness;

            v2f vert(appdata v)
            {
                v2f o;
                
                // Hand-drawn wiggle effect
                float wiggleX = sin(v.uv.y * _WiggleFrequency) * _WiggleAmplitude;
                float wiggleY = cos(v.uv.x * _WiggleFrequency) * _WiggleAmplitude;
                
                // Expand vertex along normal with wiggle
                float3 normal = normalize(v.normal);
                float3 expandedPos = v.vertex + normal * (_OutlineWidth + wiggleX + wiggleY);
                
                o.pos = UnityObjectToClipPos(expandedPos);
                o.uv = v.uv;
                o.normal = normal;
                
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Anti-aliased edge with smoothness control
                float edge = frac(i.uv.x * 10.0); // Creates edge pattern
                float alpha = smoothstep(0.0, _EdgeSoftness, edge) * smoothstep(1.0, 1.0 - _EdgeSoftness, edge);
                
                return float4(_OutlineColor.rgb, _OutlineColor.a * alpha);
            }
            ENDCG
        }

        // Transparent pass (empty interior)
        Pass
        {
            Name "FILL"
            Cull Back
            ZWrite On
            ColorMask 0
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }

    FallBack "Transparent/VertexLit"
}
