Shader "Custom/Wireframe"
{
    Properties
    {
        _WireColor ("Wire Color", Color) = (0, 1, 0, 1)
        _BackColor ("Back Color", Color) = (0.1, 0.1, 0.1, 1)
        _WireWidth ("Wire Width", Range(0.0, 0.1)) = 0.01
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            fixed4 _WireColor;
            fixed4 _BackColor;
            float _WireWidth;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 bary : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.bary = float3(0, 0, 0);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Renderizar solo wireframe - sin barycentric en vertex shader
                // Simplemente renderizar en color wireframe
                return _WireColor;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
