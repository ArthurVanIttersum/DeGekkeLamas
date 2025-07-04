Shader "Custom/Outline"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Map("Mask map", 2D) = "white" {}
        _Transparency("Transparency", Range(0, 1)) = 1
        _Emission("Emission", Float) = 0
        _Radius("Highlight radius", Float) = 1
        [Toggle] _Blinking("Does blink", Float) = 0
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
				float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 normal : NORMAL;
            };

            float4 _Color;
            sampler2D _Map;
            float _Transparency;
            float _Emission;
            float _Radius;
            bool _Blinking;

            v2f vert (appdata v)
            {
                v2f o;
                float4 newVertex = v.vertex + v.normal * _Radius;
                o.vertex = UnityObjectToClipPos(newVertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (tex2D(_Map, i.uv).a == 0) return float4(0,0,0,0);
                fixed4 col = _Color;
                col.w *= _Transparency;
                col.xyz *= 1 + _Emission;

                if (_Blinking) col.w *= round( fmod(_Time.y, 1) );
                
                return col;
            }
            ENDCG
        }
    }
}
