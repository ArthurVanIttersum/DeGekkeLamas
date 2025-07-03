Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("Outline color", Color) = (1,1,1,1)
        _Width ("Width", Range(0,1)) = .1
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _Width;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                bool isBG = col != float4(0,0,0,0);
                if (isBG)
                {
                    bool XPlus = tex2D(_MainTex, i.uv + float2(_Width, 0)) == float4(0,0,0,0);
                    bool XMin = tex2D(_MainTex, i.uv + float2(-_Width, 0)) == float4(0,0,0,0);
                    bool YPlus = tex2D(_MainTex, i.uv + float2(0, _Width)) == float4(0,0,0,0);
                    bool YMin = tex2D(_MainTex, i.uv + float2(0, -_Width)) == float4(0,0,0,0);
                    if (XPlus || XMin || YPlus || YMin) return _OutlineColor;
                }

                return col;
            }
            ENDCG
        }
    }
}
