Shader "CustomRenderTexture/MapHighLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TargetIndex ("Target Color", int) = 0
        _Tolerance ("Color Tolerance", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            int _TargetIndex;
            float _Tolerance;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                //fixed4 targetColor = _TargetColor;

                // Only check the red (r) channel
                float distance = abs(texColor.r - (float)_TargetIndex/255);

                // If the red value is close enough to the target red value, keep it
                if (distance <= _Tolerance)
                {
                    return fixed4(0, 0, 0, 0); // Keep the original color
                }
                else
                {
                    return fixed4(0,0,0,0.8); // Make it transparent
                }
            }
            ENDCG
        }
    }
}
