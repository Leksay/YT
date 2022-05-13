Shader "Leksay/ObjectHiding"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _HidingDistance("Hide Distance", Range(1,10)) = 3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float3 _PlayerWorldPos;
        float _HidingDistance;

        // UNITY_INSTANCING_BUFFER_START(Props)
        // UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            float3 toPlayer = _PlayerWorldPos - IN.worldPos;
            float xz = length(toPlayer.xz);

            float addAlpha = smoothstep(-1, 0, toPlayer.y);
            float alpha = clamp(0, 1, smoothstep(0, _HidingDistance, xz / _HidingDistance) + addAlpha);

            o.Albedo = c.rgb;
            o.Smoothness = _Glossiness;  
            o.Alpha = alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
