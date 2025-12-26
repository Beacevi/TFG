Shader "Custom/RadialTransparency"
{
Properties
    {
        _MainColor("Main Color", Color) = (1,1,1,1)
        _OpaqueRadius("Opaque Radius", Range(0,1)) = 0.3
        _GradientRadius("Gradient Radius", Range(0,1)) = 0.6
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
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
                float4 vertex : SV_POSITION;
            };

            float4 _MainColor;
            float _OpaqueRadius;
            float _GradientRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1; // Convert UV [0,1] to [-1,1] for center
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = length(i.uv);

                float alpha = 0;

                if(dist <= _OpaqueRadius)
                {
                    alpha = 1.0; // fully opaque
                }
                else if(dist <= _GradientRadius)
                {
                    // fade smoothly from opaque to transparent
                    alpha = 1.0 - (dist - _OpaqueRadius) / (_GradientRadius - _OpaqueRadius);
                }
                else
                {
                    alpha = 0.0; // fully transparent
                }

                return fixed4(_MainColor.rgb, _MainColor.a * alpha);
            }
            ENDCG
        }
    }
}
