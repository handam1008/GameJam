Shader "RYU/WallReveal"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite", 2D) = "white" {}
        _GlowColor("Glow Color", Color) = (0.35, 0.85, 1, 1)
        _HitPos("Hit Pos (World)", Vector) = (9999, 9999, 0, 0)
        _Strength("Strength", Range(0, 1)) = 0
        _Radius("Radius (World)", Float) = 1.5
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _GlowColor;
            float4 _HitPos;
            float _Strength;
            float _Radius;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 world : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // 픽셀마다 맞은 지점과의 거리를 재려고 월드 좌표를 넘긴다
                o.world = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                // 맞은 지점에서 멀수록 0에 가까워진다
                float dist = distance(i.world, _HitPos.xy);
                float fall = 1.0 - smoothstep(0.0, _Radius, dist);

                // 가장자리 쪽이 살짝 더 밝은 링을 만든다
                float ring = smoothstep(_Radius * 0.4, _Radius * 0.8, dist) * fall;

                fixed4 col;
                col.rgb = _GlowColor.rgb + ring * 0.5;
                col.a = tex.a * fall * _Strength * _GlowColor.a;
                return col;
            }
            ENDCG
        }
    }
}
