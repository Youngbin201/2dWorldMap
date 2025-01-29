Shader "Unlit/MapOutLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            ZWrite Off
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                //return tex2D(_MainTex, i.uv) - tex2D(_MainTex, i.uv + float2(0.001, 0))
                fixed4 c1 = tex2D(_MainTex, i.uv + float2(0.0001, 0));
                fixed4 c2 = tex2D(_MainTex, i.uv - float2(0.0001, 0));
                fixed4 c3 = tex2D(_MainTex, i.uv + float2(0, 0.0001));
                fixed4 c4 = tex2D(_MainTex, i.uv - float2(0, 0.0001));

                if(any(c1 !=col || any(c2 != col) || any(c3 !=col) || any(c4 != col)))
                {
                    return fixed4(0.8,0.8,0.8,0.3);
                }
                return fixed4(0,0,0,0);
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDCG
        }
    }
}
