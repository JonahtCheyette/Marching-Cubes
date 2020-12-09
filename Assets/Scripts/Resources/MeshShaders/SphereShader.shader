Shader "Custom/SphereShader"
{
    Properties
    {
        _NumLayers("Number of layers", Int) = 8

        _Center("Center", Vector) = (0, 0, 0, 0)

        _Color1("Color 1", Color) = (1,1,1,1)

        _Color2("Color 2", Color) = (1,1,1,1)
        _MinDst2("Min Distance", Float) = 0
        _Blend2("Blend", Float) = 0

        _Color3("Color 3", Color) = (1,1,1,1)
        _MinDst3("Min Distance", Float) = 0
        _Blend3("Blend", Float) = 0

        _Color4("Color 4", Color) = (1,1,1,1)
        _MinDst4("Min Distance", Float) = 0
        _Blend4("Blend", Float) = 0

        _Color5("Color 5", Color) = (1,1,1,1)
        _MinDst5("Min Distance", Float) = 0
        _Blend5("Blend", Float) = 0

        _Color6("Color 6", Color) = (1,1,1,1)
        _MinDst6("Min Distance", Float) = 0
        _Blend6("Blend", Float) = 0

        _Color7("Color 7", Color) = (1,1,1,1)
        _MinDst7("Min Distance", Float) = 0
        _Blend7("Blend", Float) = 0

        _Color8("Color 8", Color) = (1,1,1,1)
        _MinDst8("Min Distance", Float) = 0
        _Blend8("Blend", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
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

        int _NumLayers;
        
        fixed4 _Color1;
        
        fixed4 _Color2;
        float _MinDst2;
        float _Blend2;

        fixed4 _Color3;
        float _MinDst3;
        float _Blend3;

        fixed4 _Color4;
        float _MinDst4;
        float _Blend4;

        fixed4 _Color5;
        float _MinDst5;
        float _Blend5;

        fixed4 _Color6;
        float _MinDst6;
        float _Blend6;

        fixed4 _Color7;
        float _MinDst7;
        float _Blend7;

        fixed4 _Color8;
        float _MinDst8;
        float _Blend8;
        
        float4 _Center;

        float inverseLerp(float min, float max, float val) {
            //the saturate function clamps its value between 0 and 1
            return saturate((val - min) / (max - min));
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            //I know this code is awful, but it was easy to make and worked with unity's property system
            o.Albedo = _Color1.rgb;
            
            float drawStrength = 0;
            float dst = distance(IN.worldPos, _Center.xyz);

            if (_NumLayers >= 2) {
                //interpolates from 0 when the height is half a blend below that color's starting height to 1 when the height is half a blend above the starting height
                drawStrength = inverseLerp(_MinDst2 - (_Blend2 / 2) - epsilon, _MinDst2 + (_Blend2 / 2), dst);
                o.Albedo = o.Albedo * (1 - drawStrength) + (_Color2.rgb) * drawStrength;
            }

            if (_NumLayers >= 3) {
                drawStrength = inverseLerp(_MinDst3 - (_Blend3 / 2) - epsilon, _MinDst3 + (_Blend3 / 2), dst);
                o.Albedo = o.Albedo * (1 - drawStrength) + (_Color3.rgb) * drawStrength;
            }

            if (_NumLayers >= 4) {
                drawStrength = inverseLerp(_MinDst4 - (_Blend4 / 2) - epsilon, _MinDst4 + (_Blend4 / 2), dst);
                o.Albedo = o.Albedo * (1 - drawStrength) + (_Color4.rgb) * drawStrength;
            }

            if (_NumLayers >= 5) {
                drawStrength = inverseLerp(_MinDst5 - (_Blend5 / 2) - epsilon, _MinDst5 + (_Blend5 / 2), dst);
                o.Albedo = o.Albedo * (1 - drawStrength) + (_Color5.rgb) * drawStrength;
            }

            if (_NumLayers >= 6) {
                drawStrength = inverseLerp(_MinDst6 - (_Blend6 / 2) - epsilon, _MinDst6 + (_Blend6 / 2), dst);
                o.Albedo = o.Albedo * (1 - drawStrength) + (_Color6.rgb) * drawStrength;
            }

            if (_NumLayers >= 7) {
                drawStrength = inverseLerp(_MinDst7 - (_Blend7 / 2) - epsilon, _MinDst7 + (_Blend7 / 2), dst);
                o.Albedo = o.Albedo * (1 - drawStrength) + (_Color7.rgb) * drawStrength;
            }

            if (_NumLayers >= 8) {
                drawStrength = inverseLerp(_MinDst8 - (_Blend8 / 2) - epsilon, _MinDst8 + (_Blend8 / 2), dst);
                o.Albedo = o.Albedo * (1 - drawStrength) + (_Color8.rgb) * drawStrength;
            }

        }
        ENDCG
    }
    FallBack "Diffuse"
}
