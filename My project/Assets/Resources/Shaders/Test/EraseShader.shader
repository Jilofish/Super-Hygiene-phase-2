Shader "Custom/EraseBrush"
{
    Properties
    {
        _MainTex ("Base (ignored for blit)", 2D) = "white" {}
        _BrushTex ("Brush Texture", 2D) = "white" {}
        _BrushUV ("Brush UV Data", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        ZWrite Off
        Blend One One // Additive blending (writing white)
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _BrushTex;
            float4 _BrushUV;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Current pixel color from _MainTex
                fixed4 baseCol = tex2D(_MainTex, i.uv);

                // Convert current UV into brush space
                float2 brushUV = (i.uv - _BrushUV.xy) / _BrushUV.zw;

                // Sample the brush texture
                fixed4 brushCol = tex2D(_BrushTex, brushUV);

                // If inside brush bounds, blend white
                if (brushUV.x >= 0.0 && brushUV.x <= 1.0 && brushUV.y >= 0.0 && brushUV.y <= 1.0)
                {
                    // Blend brush alpha (write white)
                    baseCol.rgb += brushCol.a;
                    baseCol.a += brushCol.a;
                }

                return baseCol;
            }
            ENDCG
        }
    }
}