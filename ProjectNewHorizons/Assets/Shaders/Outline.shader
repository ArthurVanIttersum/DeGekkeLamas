Shader "Custom/Outline"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Transparency("Transparency", Range(0, 1)) = 1
        _Emission("Emission", Float) = 0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
			"Queue" = "Transparent"
        }
        LOD 100
        ZWrite Off
        Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

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

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _Transparency;
            float _Emission;

            v2f vert (appdata v)
            {
                v2f o;
                float4 newVertex = v.vertex * 1.1;
                o.vertex = UnityObjectToClipPos(newVertex);
                o.uv = v.uv;
                // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color;
                col.w *= _Transparency;
                col.xyz *= 1 + _Emission;
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
