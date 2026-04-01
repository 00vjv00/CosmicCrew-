Shader "Cosmic/Moebius Wall Fade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.01
        _WiggleFrequency ("Wiggle Frequency", Range(0.5, 10.0)) = 3.0
        _WiggleAmplitude ("Wiggle Amplitude", Range(0.0, 0.02)) = 0.005
        
        // Hatching
        _HatchTex ("Hatch Pattern", 2D) = "white" {}
        _HatchThreshold ("Hatch Threshold", Range(0.0, 1.0)) = 0.5
        _HatchIntensity ("Hatch Intensity", Range(0.0, 1.0)) = 0.8
        
        // Fade settings
        _FadeDistance ("Fade Distance", Range(0.5, 20.0)) = 5.0
        _FadeStrength ("Fade Strength", Range(0.0, 1.0)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        // Main pass - filled with hatching (normal view)
        Pass
        {
            Name "MAIN"
            Cull Back
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _HatchTex;
            float4 _MainTex_ST;
            float4 _BaseColor;
            float _HatchThreshold;
            float _HatchIntensity;
            float _FadeDistance;
            float _FadeStrength;
            
            // Global player position (set from script)
            float3 _PlayerPos;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);
                
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Base color with texture
                float4 texCol = tex2D(_MainTex, i.uv);
                float4 col = texCol * _BaseColor;
                
                // Lighting
                float3 normal = normalize(i.worldNormal);
                float3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float ndotv = abs(dot(normal, worldViewDir));
                
                // Screen-space hatching
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float hatch = frac(screenUV.x * 100.0) > 0.5 ? 0.9 : 1.0;
                
                // Apply hatching based on lighting
                float hatchAmount = smoothstep(_HatchThreshold, _HatchThreshold - 0.1, ndotv);
                col.rgb = lerp(col.rgb, col.rgb * hatch, hatchAmount * _HatchIntensity);
                
                // Calculate fade based on distance to player
                float distToPlayer = distance(i.worldPos, _PlayerPos);
                float fadeFactor = smoothstep(_FadeDistance, 0.0, distToPlayer);
                fadeFactor *= _FadeStrength;
                
                // Fade to transparency
                float alpha = lerp(1.0, 0.1, fadeFactor);
                
                return float4(col.rgb, alpha);
            }
            ENDCG
        }

        // Outline pass - only visible when close (fade view)
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
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _OutlineColor;
            float _OutlineWidth;
            float _WiggleFrequency;
            float _WiggleAmplitude;
            float _FadeDistance;
            float _FadeStrength;
            
            // Global player position
            float3 _PlayerPos;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                // Hand-drawn wiggle
                float wiggleX = sin(v.uv.y * _WiggleFrequency) * _WiggleAmplitude;
                float wiggleY = cos(v.uv.x * _WiggleFrequency) * _WiggleAmplitude;
                
                // Expand vertex along normal
                float3 normal = normalize(v.normal);
                float3 expandedPos = v.vertex + normal * (_OutlineWidth + wiggleX + wiggleY);
                
                o.pos = UnityObjectToClipPos(expandedPos);
                o.worldPos = mul(unity_ObjectToWorld, expandedPos).xyz;
                o.uv = v.uv;
                o.normal = normal;
                
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Calculate fade based on distance to player
                float distToPlayer = distance(i.worldPos, _PlayerPos);
                float fadeFactor = smoothstep(_FadeDistance, 0.0, distToPlayer);
                fadeFactor *= _FadeStrength;
                
                // Outline only becomes visible when close to player
                float alpha = _OutlineColor.a * fadeFactor;
                
                // Anti-aliased edge
                float edge = frac(i.uv.x * 10.0);
                alpha *= smoothstep(0.0, 0.3, edge) * smoothstep(1.0, 0.7, edge);
                
                return float4(_OutlineColor.rgb, alpha);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
