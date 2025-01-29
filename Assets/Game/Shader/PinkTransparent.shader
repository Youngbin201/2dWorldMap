Shader "CustomRenderTexture/PinkTransparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TransparentColor ("Transparent Color", Color) = (1, 0, 1, 1) // 투명 처리할 색 (핑크)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _TransparentColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                if (abs(texColor.r - _TransparentColor.r) < 0.1 &&
                    abs(texColor.g - _TransparentColor.g) < 0.1 &&
                    abs(texColor.b - _TransparentColor.b) < 0.1)
                {
                    texColor.a = 0; // 투명 처리
                }
                return texColor;
            }
            ENDCG
        }
    }
}
