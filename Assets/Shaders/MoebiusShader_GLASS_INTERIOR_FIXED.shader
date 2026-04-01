Shader "Comic/Moebius Glass Interior Adjustable"
{
    Properties
    {
        [Header(Glass)]
        _Color ("Glass Color", Color) = (0.8, 0.95, 1, 0.7)
        _Transparency ("Transparency", Range(0, 1)) = 0.7
        _Darkness ("Darkness Reduction", Range(0, 1)) = 0.3
        
        [Header(Fresnel)]
        _FresnelPower ("Fresnel Power", Range(0.1, 5)) = 3.0
        _FresnelStrength ("Fresnel Strength", Range(0, 2)) = 1.0
        
        [Header(Rim Light)]
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 10)) = 1.5
        _RimStrength ("Rim Strength", Range(0, 2)) = 1.2
        
        [Header(Specular)]
        _SpecularStrength ("Specular Strength", Range(0, 2)) = 1.5
        _SpecularShininess ("Specular Shininess", Range(1, 256)) = 64.0
        
        [Header(Ambient)]
        _AmbientBoost ("Ambient Boost", Range(0.5, 3)) = 1.5
        _MinBrightness ("Min Brightness", Range(0, 0.5)) = 0.3
        
        [Header(Moebius Style)]
        _ColorQuantization ("Color Levels", Range(2, 16)) = 5
        _Brightness ("Brightness", Range(0.5, 2)) = 1.6
        _OutlineColor ("Outline Color", Color) = (0.1, 0.2, 0.3, 1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.03)) = 0.002
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        LOD 100
        
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
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                v.vertex.xyz += norm * _OutlineWidth;
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
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite On
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            
            float4 _Color;
            float4 _RimColor;
            float4 _OutlineColor;
            float _Transparency;
            float _Darkness;
            float _FresnelPower;
            float _FresnelStrength;
            float _RimPower;
            float _RimStrength;
            float _SpecularStrength;
            float _SpecularShininess;
            float _AmbientBoost;
            float _MinBrightness;
            float _ColorQuantization;
            float _Brightness;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.lightDir = _WorldSpaceLightPos0.xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);
                float3 lightDir = normalize(i.lightDir);
                
                float fresnel = 1.0 - abs(dot(viewDir, normal));
                fresnel = pow(fresnel, _FresnelPower);
                fresnel *= _FresnelStrength;
                
                float rim = 1.0 - abs(dot(viewDir, normal));
                rim = pow(rim, _RimPower);
                float3 rimLight = _RimColor.rgb * rim * _RimStrength;
                
                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(max(0.0, dot(normal, halfDir)), _SpecularShininess);
                float3 specular = _LightColor0.rgb * spec * _SpecularStrength;
                
                float nl = max(0.0, dot(normal, lightDir));
                float3 ambient = ShadeSH9(float4(normal, 1.0)) * _AmbientBoost;
                float3 directLight = _LightColor0.rgb * nl * 0.7;
                
                float3 finalColor = _Color.rgb;
                finalColor *= (ambient + directLight);
                finalColor += specular;
                finalColor += rimLight;
                finalColor *= _Brightness;
                
                finalColor = max(finalColor, float3(_MinBrightness, _MinBrightness, _MinBrightness));
                
                finalColor = lerp(finalColor, finalColor * 1.2, _Darkness);
                
                finalColor = floor(finalColor * _ColorQuantization) / _ColorQuantization;
                
                float alpha = _Transparency + (fresnel * 0.25);
                alpha = clamp(alpha, 0.3, 0.95);
                
                return float4(finalColor, alpha);
            }
            ENDCG
        }
        
        Pass
        {
            Name "ADD"
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            
            float4 _Color;
            float4 _RimColor;
            float _RimPower;
            float _RimStrength;
            float _SpecularStrength;
            float _SpecularShininess;
            float _ColorQuantization;
            float _Brightness;
            float _MinBrightness;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.lightDir = _WorldSpaceLightPos0.xyz - o.worldPos * _WorldSpaceLightPos0.w;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);
                float3 lightDir = normalize(i.lightDir);
                
                float rim = 1.0 - abs(dot(viewDir, normal));
                rim = pow(rim, _RimPower);
                float3 rimLight = _RimColor.rgb * rim * _RimStrength * 0.5;
                
                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(max(0.0, dot(normal, halfDir)), _SpecularShininess);
                float3 specular = _LightColor0.rgb * spec * _SpecularStrength * 0.7;
                
                float nl = max(0.0, dot(normal, lightDir));
                float3 directLight = _LightColor0.rgb * nl * 0.5;
                
                float3 finalColor = _Color.rgb;
                finalColor *= directLight;
                finalColor += specular;
                finalColor += rimLight;
                finalColor *= _Brightness;
                
                finalColor = max(finalColor, float3(_MinBrightness, _MinBrightness, _MinBrightness));
                finalColor = floor(finalColor * _ColorQuantization) / _ColorQuantization;
                
                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    
    Fallback "Transparent/VertexLit"
}
