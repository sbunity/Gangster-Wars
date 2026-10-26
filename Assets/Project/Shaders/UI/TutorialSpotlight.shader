Shader "UI/TutorialSpotlight"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _DimColor ("Dim Color", Color) = (0,0,0,0.65)
        _FrameColor ("Frame Color", Color) = (1,0.76,0.1,1)
        _FrameWidth ("Frame Width", Float) = 6
        _RectSize ("Rect Size", Vector) = (1,1,0,0)
        _HoleCenter ("Hole Center", Vector) = (0,0,0,0)
        _HoleHalfSize ("Hole Half Size", Vector) = (0,0,0,0)
        _HoleRadius ("Hole Corner Radius", Float) = 24
        _HoleSoftness ("Hole Softness", Float) = 24
        _HoleEnabled ("Hole Enabled", Float) = 0
        _Hole2Center ("Second Hole Center", Vector) = (0,0,0,0)
        _Hole2HalfSize ("Second Hole Half Size", Vector) = (0,0,0,0)
        _Hole2Enabled ("Second Hole Enabled", Float) = 0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            fixed4 _DimColor;
            fixed4 _FrameColor;
            float _FrameWidth;
            float4 _RectSize;
            float4 _HoleCenter;
            float4 _HoleHalfSize;
            float _HoleRadius;
            float _HoleSoftness;
            float _HoleEnabled;
            float4 _Hole2Center;
            float4 _Hole2HalfSize;
            float _Hole2Enabled;
            float4 _ClipRect;

            static const float NoHoleDistance = 100000.0;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            float RoundedBoxDistance(float2 position, float2 halfSize, float radius)
            {
                float2 q = abs(position) - halfSize + radius;
                return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - radius;
            }

            float HoleDistance(float2 position, float4 center, float4 halfSize, float enabled)
            {
                float radius = min(_HoleRadius, min(halfSize.x, halfSize.y));
                float distance = RoundedBoxDistance(position - center.xy, halfSize.xy, radius);
                return lerp(NoHoleDistance, distance, enabled);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 position = IN.texcoord * _RectSize.xy;
                float distance = min(
                    HoleDistance(position, _HoleCenter, _HoleHalfSize, _HoleEnabled),
                    HoleDistance(position, _Hole2Center, _Hole2HalfSize, _Hole2Enabled));

                float outside = saturate(distance / max(_HoleSoftness, 0.0001));
                float dimAlpha = _DimColor.a * outside;

                float halfFrame = _FrameWidth * 0.5;
                float frameMask = 1.0 - smoothstep(halfFrame - 1.0, halfFrame + 1.0, abs(distance - halfFrame));
                float frameAlpha = _FrameColor.a * frameMask * step(0.001, _FrameWidth);

                float alpha = frameAlpha + dimAlpha * (1.0 - frameAlpha);
                float3 premultiplied = _FrameColor.rgb * frameAlpha + _DimColor.rgb * dimAlpha * (1.0 - frameAlpha);
                fixed4 color = fixed4(premultiplied / max(alpha, 0.0001), alpha * IN.color.a);

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}
