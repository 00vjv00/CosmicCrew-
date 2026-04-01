Shader "Comic/Moebius Style"
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
        _OutlineWidth ("Outline Width", Range(0.001, 0.03)) = 0.005
        _SpecularThreshold ("Specular Highlight Threshold", Range(0, 1)) = 0.3
        _Glossiness ("Glossiness", Range(0, 1)) = 0.5
        
        [Header(Shadow Hatching)]
        _HatchPattern ("Hatch Pattern (RGB = Legacy)", 2D) = "white" {}
        _HatchScale ("Hatch Scale", Range(0.5, 4)) = 1.0
        _HatchIntensity ("Hatch Intensity", Range(0, 1)) = 0.8
        
        [Header(Contextual Hatching Screen Space MOD)]
        _HatchBrightness75 ("Hatch Dark Threshold", Range(0, 1)) = 0.55
        _HatchBrightness50 ("Hatch Darker Threshold", Range(0, 1)) = 0.45
        _HatchBrightness25 ("Hatch Darkest Threshold", Range(0, 1)) = 0.35
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
            
            // ===== MEJORA 1: HAND-DRAWN OUTLINES =====
            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                
                // Hand-drawn: sin + cos para ondulaciones naturales
                // (Sin hash/random para evitar ruido en colores)
                float wiggleX = sin(v.uv.y * 0.5 * 6.28) * 0.15;
                float wiggleY = cos(v.uv.x * 0.5 * 6.28) * 0.15 * 0.7;
                float wiggleAmount = wiggleX + wiggleY;
                
                // Expandir outline con wiggles suaves
                float totalWidth = _OutlineWidth + wiggleAmount * 0.003;
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
            float _HatchBrightness75;
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
                
                // Reconstruct TBN matrix from individual components
                float3x3 tbnMatrix = float3x3(
                    i.worldTangent,
                    i.worldBitangent,
                    i.worldNormal
                );
                
                float3 normal = normalize(mul(normalTex, tbnMatrix));
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                float nl = max(0.0, dot(normal, lightDir));
                
                // Posterized shadow step (efecto cómico)
                float shadowStep = step(_ShadowThreshold, nl);
                nl = lerp(0.3, 1.0, shadowStep);
                
                float3 ambient = ShadeSH9(float4(normal, 1.0));
                float3 diffuse = _LightColor0.rgb * nl;
                
                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(max(0.0, dot(normal, halfDir)), 64.0);
                float3 specular = spec * _Glossiness * 0.5;
                
                // Specular white highlights (Moebius style como en Heckel blog)
                // Si especular es alto, mostrar como blanco puro para efecto de brillo
                float specularBrightness = spec;
                if (specularBrightness >= _SpecularThreshold)
                {
                    // White highlight instead of tinted specular
                    specular = float3(1.0, 1.0, 1.0) * specularBrightness * 0.8;
                }
                
                float3 finalColor = albedoTex.rgb * (ambient * 0.5 + diffuse);
                finalColor += specular;
                
                // EMISSION
                float4 emissionTex = tex2D(_EmissionMap, TRANSFORM_TEX(i.uv, _EmissionMap));
                float3 emission = emissionTex.rgb * _EmissionColor.rgb * _EmissionIntensity;
                finalColor += emission;
                
                finalColor *= _Brightness;
                
                // ===== WORLD-SPACE MODULO HATCHING (Stablized - no se mueve con la cámara) =====
                float brightness = dot(finalColor, float3(0.3, 0.6, 0.1)); // Luminancia
                
                if (brightness < 0.8 && _HatchIntensity > 0.0)
                {
                    // CAMBIO CRÍTICO: Usar worldPos en lugar de screenPos
                    // Esto hace que el patrón se quede fijo en el mundo mientras se mueve la cámara
                    float invScale = 1.0 / _HatchScale; // Invertir escala: más grande = más espaciado
                    float worldY = i.worldPos.y * invScale;
                    float worldX = i.worldPos.x * invScale;
                    float worldZ = i.worldPos.z * invScale;
                    
                    // Desplazamiento procedural basado en world-space
                    float hash1 = frac(sin(dot(i.worldPos.xyz, float2(12.9898, 78.233))) * 43758.5453);
                    float hash2 = frac(sin(dot(i.worldPos.xyz, float2(45.164, 94.673))) * 94673.4391);
                    
                    float displacementX = hash1 * 0.3;
                    float displacementY = hash2 * 0.3;
                    
                    // Variables de hatching
                    float hatchingValue = 0.0;
                    float lineThickness = 0.6; // Grosor de líneas (0-1)
                    float hatchSpacing = 1.5 * _HatchScale; // Espaciado responde a _HatchScale
                    
                    // Nivel 1: Hatching horizontal (brightness < 0.55)
                    if (brightness < 0.55)
                    {
                        float hLine = fmod((worldY + displacementY), hatchSpacing);
                        if (hLine < lineThickness)
                        {
                            hatchingValue = max(hatchingValue, 0.6);
                        }
                    }
                    
                    // Nivel 2: Hatching vertical + horizontal (brightness < 0.45)
                    if (brightness < 0.45)
                    {
                        float hLine = fmod((worldY + displacementY), hatchSpacing);
                        float vLine = fmod((worldX + displacementX), hatchSpacing);
                        if (hLine < lineThickness || vLine < lineThickness)
                        {
                            hatchingValue = max(hatchingValue, 0.8);
                        }
                    }
                    
                    // Nivel 3: Hatching diagonal + horizontal + vertical (brightness < 0.35)
                    if (brightness < 0.35)
                    {
                        float hLine = fmod((worldY + displacementY), hatchSpacing * 0.9);
                        float vLine = fmod((worldX + displacementX), hatchSpacing * 0.9);
                        float dLine = fmod((worldX + worldY + displacementX + displacementY), hatchSpacing * 0.85);
                        if (hLine < lineThickness || vLine < lineThickness || dLine < lineThickness)
                        {
                            hatchingValue = 1.0;
                        }
                    }
                    
                    // Aplicar el hatching oscureciendo el color
                    if (hatchingValue > 0.0)
                    {
                        finalColor = lerp(finalColor, finalColor * 0.3, hatchingValue * _HatchIntensity);
                    }
                }
                
                // Color quantization (posterize)
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
                
                // Reconstruct TBN matrix from individual components
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
                finalColor += spec * _Glossiness * 0.5;
                
                // EMISSION (también en AddPass)
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
