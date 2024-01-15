Shader "Custom/Snake"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _EyeColor ("Eye Color", Color) = (0,0,0,1)
        _HornColor ("Horn Color", Color) = (0,0,0,1)
        _NormalMap ("Normal", 2D) = "bump" {}
        _NormalIntensity ("Normal Strength", Range(0,1)) = 0.5
        _NormalScale ("Normal Scale", Float) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _NormalMap;

        struct Input
        {
            float2 uv_NormalMap;
            float3 vertColors : COLOR;
            float2 screenPos : TEXCOORD2;
      	    float4 position : POSITION;
        };

        half _NormalIntensity;
        half _NormalScale;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EyeColor;
        fixed4 _HornColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            float2 screenUV = IN.screenPos.xy;
            screenUV.y *= _ScreenParams.y / _ScreenParams.x;

            float4 col = IN.vertColors.g * _Color;
            col = lerp(col, _EyeColor, IN.vertColors.r);
            col = lerp(col, _HornColor, IN.vertColors.b);
            
            o.Albedo = col;
            o.Normal = UnpackNormalWithScale(tex2D (_NormalMap, screenUV * 10.0 * _NormalScale), _NormalIntensity);
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
