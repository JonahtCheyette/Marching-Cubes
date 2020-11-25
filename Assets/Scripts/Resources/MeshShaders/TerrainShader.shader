Shader "Custom/TerrainShader"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1,1,1,1)

        _Color2 ("Color 2", Color) = (1,1,1,1)
        _MinY2 ("Min Height", Float) = 0
        _Blend2 ("Blend", Float) = 0

        _Color3 ("Color 3", Color) = (1,1,1,1)
        _MinY3 ("Min Height", Float) = 0
        _Blend3 ("Blend", Float) = 0

        _Color4 ("Color 4", Color) = (1,1,1,1)
        _MinY4 ("Min Height", Float) = 0
        _Blend4 ("Blend", Float) = 0

        _Color5 ("Color 5", Color) = (1,1,1,1)
        _MinY5 ("Min Height", Float) = 0
        _Blend5 ("Blend", Float) = 0

        _Color6 ("Color 6", Color) = (1,1,1,1)
        _MinY6 ("Min Height", Float) = 0
        _Blend6 ("Blend", Float) = 0

        _Color7 ("Color 7", Color) = (1,1,1,1)
        _MinY7 ("Min Height", Float) = 0
        _Blend7 ("Blend", Float) = 0

        _Color8 ("Color 8", Color) = (1,1,1,1)
        _MinY8 ("Min Height", Float) = 0
        _Blend8 ("Blend", Float) = 0
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

        const static float epsilon = 1E-4;

        struct Input {
            //gets the world position of every pixel
            float3 worldPos;
        };

        fixed4 _Color1;

        fixed4 _Color2;
        float _MinY2;
        float _Blend2;

        fixed4 _Color3;
        float _MinY3;
        float _Blend3;

        fixed4 _Color4;
        float _MinY4;
        float _Blend4;

        fixed4 _Color5;
        float _MinY5;
        float _Blend5;

        fixed4 _Color6;
        float _MinY6;
        float _Blend6;

        fixed4 _Color7;
        float _MinY7;
        float _Blend7;

        fixed4 _Color8;
        float _MinY8;
        float _Blend8;

        float inverseLerp(float min, float max, float val) {
            //the saturate function clamps its value between 0 and 1
            return saturate((val - min) / (max - min));
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            //I know this code is awful, but it was easy to make and worked with unity's property system
            o.Albedo = _Color1.rgb;

            //interpolates from 0 when the height is half a baseBlends below that base's starting height to 1 when the height is half a baseBlends above the starting height
            float drawStrength = inverseLerp(_MinY2 - (_Blend2 / 2) - epsilon, _MinY2 + (_Blend2 / 2), IN.worldPos.y);
            o.Albedo = o.Albedo * (1 - drawStrength) + (_Color2.rgb) * drawStrength;

            drawStrength = inverseLerp(_MinY3 - (_Blend3 / 2) - epsilon, _MinY3 + (_Blend3 / 2), IN.worldPos.y);
            o.Albedo = o.Albedo * (1 - drawStrength) + (_Color3.rgb) * drawStrength;

            drawStrength = inverseLerp(_MinY4 - (_Blend4 / 2) - epsilon, _MinY4 + (_Blend4 / 2), IN.worldPos.y);
            o.Albedo = o.Albedo * (1 - drawStrength) + (_Color4.rgb) * drawStrength;

            drawStrength = inverseLerp(_MinY5 - (_Blend5 / 2) - epsilon, _MinY5 + (_Blend5 / 2), IN.worldPos.y);
            o.Albedo = o.Albedo * (1 - drawStrength) + (_Color5.rgb) * drawStrength;

            drawStrength = inverseLerp(_MinY6 - (_Blend6 / 2) - epsilon, _MinY6 + (_Blend6 / 2), IN.worldPos.y);
            o.Albedo = o.Albedo * (1 - drawStrength) + (_Color6.rgb) * drawStrength;

            drawStrength = inverseLerp(_MinY7 - (_Blend7 / 2) - epsilon, _MinY7 + (_Blend7 / 2), IN.worldPos.y);
            o.Albedo = o.Albedo * (1 - drawStrength) + (_Color7.rgb) * drawStrength;

            drawStrength = inverseLerp(_MinY8 - (_Blend8 / 2) - epsilon, _MinY8 + (_Blend8 / 2), IN.worldPos.y);
            o.Albedo = o.Albedo * (1 - drawStrength) + (_Color8.rgb) * drawStrength;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
