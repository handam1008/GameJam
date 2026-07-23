Shader "RYU/WallReveal"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite", 2D) = "white" {}
        _GlowColor("Glow Color", Color) = (0.35, 0.85, 1, 1)
        _HitPos("Hit Pos (World)", Vector) = (9999, 9999, 0, 0)
        _WallDir("Wall Dir", Vector) = (1, 0, 0, 0)
        _Strength("Strength", Range(0, 1)) = 0
        _Radius("Radius (World)", Float) = 1.5
        _Squash("Squash", Range(0.1, 1)) = 0.45
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
            float4 _WallDir;
            float _Strength;
            float _Radius;
            float _Squash;

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

                // 벽 방향 기준으로 좌표를 나눈다. along = 벽을 따라, across = 벽에 수직
                float2 rel = i.world - _HitPos.xy;
                float2 t = normalize(_WallDir.xy);
                float2 n = float2(-t.y, t.x);
                float along = dot(rel, t);
                float across = dot(rel, n);

                // 벽 방향으로는 넓게, 수직으로는 좁게 퍼지는 타원 거리
                float q = length(float2(along / _Radius, across / (_Radius * _Squash)));

                float fall = 1.0 - smoothstep(0.0, 1.0, q);

                // 가장자리 쪽이 살짝 더 밝은 링을 만든다
                float ring = smoothstep(0.4, 0.8, q) * fall;

                fixed4 col;
                col.rgb = _GlowColor.rgb + ring * 0.5;
                col.a = tex.a * fall * _Strength * _GlowColor.a;
                return col;
            }
            ENDCG
        }
    }
}
