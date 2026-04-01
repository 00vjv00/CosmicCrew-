Shader "Comic/Moebius Style Environment"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Strength", Range(0, 2)) = 1.0
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _EmissionIntensity ("Emission Intensity", Range(0, 3)) = 1.0
        _ColorQuantization ("Color Levels", Range(2, 16)) = 6
        _Brightness ("Brightness", Range(0.5, 2)) = 1.2
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.3
        _OutlineColor ("Outline Color", Color) = (0.1, 0.2, 0.3, 1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.03)) = 0.003
        _SpecularThreshold ("Specular Highlight Threshold", Range(0, 1)) = 0.3
        _Glossiness ("Glossiness", Range(0, 1)) = 0.5
        
        [Header(Environment Hatching Simplified)]
        _HatchPattern ("Hatch Pattern (RGB = Legacy)", 2D) = "white" {}
        _HatchScale ("Hatch Scale", Range(0.5, 4)) = 1.0
        _HatchIntensity ("Hatch Intensity", Range(0, 1)) = 0.4
        
        [Header(Contextual Hatching Screen Space MOD)]
        _HatchBrightness50 ("Hatch Medium Threshold", Range(0, 1)) = 0.50
        _HatchBrightness25 ("Hatch Dark Threshold", Range(0, 1)) = 0.35
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass
        {
            Name "OUTLINE"
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            float _OutlineWidth;
            float4 _OutlineColor;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                
                // Minimal outline wiggle for environment
                float wiggleX = sin(v.uv.y * 0.5 * 6.28) * 0.08;
                float wiggleY = cos(v.uv.x * 0.5 * 6.28) * 0.08 * 0.7;
                float wiggleAmount = wiggleX + wiggleY;
                
                float totalWidth = _OutlineWidth + wiggleAmount * 0.002;
                v.vertex.xyz += norm * totalWidth;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
        
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityStandardUtils.cginc"
            
            sampler2D _MainTex;
            sampler2D _BumpMap;
            sampler2D _EmissionMap;
            sampler2D _HatchPattern;
            
            float4 _MainTex_ST;
            float4 _BumpMap_ST;
            float4 _EmissionMap_ST;
            float4 _HatchPattern_ST;
            
            float4 _Color;
            float _BumpScale;
            float _Glossiness;
            float4 _EmissionColor;
            float _EmissionIntensity;
            float _ColorQuantization;
            float _Brightness;
            float _ShadowThreshold;
            float4 _OutlineColor;
            float _SpecularThreshold;
            float _HatchScale;
            float _HatchIntensity;
            float _HatchBrightness50;
            float _HatchBrightness25;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                float3 worldBitangent : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screenPos = ComputeScreenPos(o.vertex);
                
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 worldBitangent = cross(worldNormal, worldTangent) * v.tangent.w;
                
                o.worldNormal = worldNormal;
                o.worldTangent = worldTangent;
                o.worldBitangent = worldBitangent;
                
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                float4 albedoTex = tex2D(_MainTex, i.uv);
                albedoTex *= _Color;
                
                float3 normalTex = UnpackNormal(tex2D(_BumpMap, TRANSFORM_TEX(i.uv, _BumpMap)));
                normalTex.xy *= _BumpScale;
                
                float3x3 tbnMatrix = float3x3(
                    i.worldTangent,
                    i.worldBitangent,
                    i.worldNormal
                );
                
                float3 normal = normalize(mul(normalTex, tbnMatrix));
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                float nl = max(0.0, dot(normal, lightDir));
                
                // Posterized shadow step
                float shadowStep = step(_ShadowThreshold, nl);
                nl = lerp(0.3, 1.0, shadowStep);
                
                float3 ambient = ShadeSH9(float4(normal, 1.0));
                float3 diffuse = _LightColor0.rgb * nl;
                
                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(max(0.0, dot(normal, halfDir)), 64.0);
                float3 specular = spec * _Glossiness * 0.5;
                
                // Reduced specular highlights for environment
                float specularBrightness = spec;
                if (specularBrightness >= _SpecularThreshold)
                {
                    specular = float3(1.0, 1.0, 1.0) * specularBrightness * 0.5;
                }
                
                float3 finalColor = albedoTex.rgb * (ambient * 0.5 + diffuse);
                finalColor += specular;
                
                // EMISSION
                float4 emissionTex = tex2D(_EmissionMap, TRANSFORM_TEX(i.uv, _EmissionMap));
                float3 emission = emissionTex.rgb * _EmissionColor.rgb * _EmissionIntensity;
                finalColor += emission;
                
                finalColor *= _Brightness;
                
                // ===== SIMPLIFIED HATCHING FOR ENVIRONMENT (2 LEVELS ONLY) =====
                float brightness = dot(finalColor, float3(0.3, 0.6, 0.1));
                
                if (brightness < 0.8 && _HatchIntensity > 0.0)
                {
                    // Minimal displacement for environment
                    float hash = frac(sin(dot(i.screenPos.xy, float2(12.9898, 78.233))) * 43758.5453);
                    float displacementX = hash * sin(i.screenPos.y * 0.008) * 1.5;
                    float displacementY = hash * cos(i.screenPos.x * 0.008) * 1.5;
                    
                    // Screen-space hatching variables
                    float screenY = i.screenPos.y * 100.0;
                    float screenX = i.screenPos.x * 100.0;
                    float hatchingValue = 0.0;
                    float lineThickness = 1.2;  // Thicker lines for environment
                    
                    // Nivel 1: Hatching horizontal (brightness < 0.50)
                    if (brightness < _HatchBrightness50)
                    {
                        float hLine = fmod((screenY + displacementY), 5.0);
                        if (hLine < lineThickness)
                        {
                            hatchingValue = max(hatchingValue, 0.6);
                        }
                    }
                    
                    // Nivel 2: Hatching vertical + horizontal (brightness < 0.35)
                    if (brightness < _HatchBrightness25)
                    {
                        float hLine = fmod((screenY + displacementY), 4.0);
                        float vLine = fmod((screenX + displacementX), 4.0);
                        if (hLine < lineThickness || vLine < lineThickness)
                        {
                            hatchingValue = max(hatchingValue, 0.85);
                        }
                    }
                    
                    // Apply hatching
                    if (hatchingValue > 0.0)
                    {
                        finalColor = lerp(finalColor, finalColor * 0.4, hatchingValue * _HatchIntensity);
                    }
                }
                
                // Color quantization
                finalColor = floor(finalColor * _ColorQuantization) / _ColorQuantization;
                
                return float4(finalColor, albedoTex.a);
            }
            ENDCG
        }
        
        Pass
        {
            Name "FORWARD_ADD"
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            
            sampler2D _MainTex;
            sampler2D _BumpMap;
            sampler2D _EmissionMap;
            
            float4 _MainTex_ST;
            float4 _BumpMap_ST;
            float4 _EmissionMap_ST;
            
            float4 _Color;
            float _BumpScale;
            float _Glossiness;
            float4 _EmissionColor;
            float _EmissionIntensity;
            float _ColorQuantization;
            float _Brightness;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                float3 worldBitangent : TEXCOORD4;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 worldBitangent = cross(worldNormal, worldTangent) * v.tangent.w;
                
                o.worldNormal = worldNormal;
                o.worldTangent = worldTangent;
                o.worldBitangent = worldBitangent;
                
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                float4 albedoTex = tex2D(_MainTex, i.uv);
                albedoTex *= _Color;
                
                float3 normalTex = UnpackNormal(tex2D(_BumpMap, TRANSFORM_TEX(i.uv, _BumpMap)));
                
                float3x3 tbnMatrix = float3x3(
                    i.worldTangent,
                    i.worldBitangent,
                    i.worldNormal
                );
                
                float3 normal = normalize(mul(normalTex, tbnMatrix));
                
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos * _WorldSpaceLightPos0.w);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                float nl = max(0.0, dot(normal, lightDir));
                float3 finalColor = albedoTex.rgb * _LightColor0.rgb * nl;
                
                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(max(0.0, dot(normal, halfDir)), 64.0);
                finalColor += spec * _Glossiness * 0.3;
                
                float4 emissionTex = tex2D(_EmissionMap, TRANSFORM_TEX(i.uv, _EmissionMap));
                float3 emission = emissionTex.rgb * _EmissionColor.rgb * _EmissionIntensity;
                finalColor += emission;
                
                finalColor *= _Brightness;
                finalColor = floor(finalColor * _ColorQuantization) / _ColorQuantization;
                
                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    
    Fallback "Standard"
}
