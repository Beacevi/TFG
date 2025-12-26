Shader "Custom/Gradient5Colors"
{
    Properties
    {
        _CycleDuration("Full Cycle Duration (seconds)", Float) = 300 // 5 minutes

        _Color1("Color 1", Color) = (1,0,0,1)
        _Color2("Color 2", Color) = (0,1,0,1)
        _Color3("Color 3", Color) = (0,0,1,1)
        _Color4("Color 4", Color) = (1,1,0,1)
        _Color5("Color 5", Color) = (1,0,1,1)
    }
    SubShader
    {
       Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _CycleDuration;
            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _Color3;
            fixed4 _Color4;
            fixed4 _Color5;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Normalized time [0,1] over the full cycle
                float t = fmod(_Time.y / _CycleDuration, 1);

                // Scale to number of segments (5 colors)
                float scaledT = t * 5.0;
                int idx = (int)scaledT;
                float blend = scaledT - idx;

                fixed3 color;
                if(idx == 0) color = lerp(_Color1.rgb, _Color2.rgb, blend);
                else if(idx == 1) color = lerp(_Color2.rgb, _Color3.rgb, blend);
                else if(idx == 2) color = lerp(_Color3.rgb, _Color4.rgb, blend);
                else if(idx == 3) color = lerp(_Color4.rgb, _Color5.rgb, blend);
                else color = lerp(_Color5.rgb, _Color1.rgb, blend);

                return fixed4(color,1);
            }
            ENDCG
        }
    }
}