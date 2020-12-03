Shader "Custom/NormalBasedShader"
{
    Properties
    {
        _Top("Top", Color) = (1,1,1,1)
        _TopAngle("Min Angle", Range(0, 3.14159265)) = 2.0943951

        _Side("Side", Color) = (1,1,1,1)
        _SideAngle("Min Angle", Range(0, 3.14159265)) = 1.04719755
        _SideTopBlend("Blend", Range(0, 3.14159265)) = 0

        _Bottom("Bottom", Color) = (1,1,1,1)
        _BottomSideBlend("Blend", Range(0, 3.14159265)) = 0
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
            //gets the world normal of every pixel, only workse if we don't write to the world normal in the shader, there's a seperate thing for that, check the documentation.
            float3 worldNormal;
        };

        fixed4 _Top;
        float _TopAngle;

        fixed4 _Side;
        float _SideAngle;
        float _SideTopBlend;

        fixed4 _Bottom;
        float _BottomSideBlend;

        float inverseLerp(float min, float max, float val) {
            //the saturate function clamps its value between 0 and 1
            return saturate((val - min) / (max - min));
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            //I know this code is awful, but it was easy to make and worked with unity's property system
            o.Albedo = _Bottom.rgb;

            float angle = acos(dot(IN.worldNormal, float3(0, -1, 0)));

            //interpolates from 0 when the angle is half a blend below the starting angle to 1 when the anglt is half a blend above the starting angle
            float drawStrength = inverseLerp(_SideAngle - (_BottomSideBlend / 2) - epsilon, _SideAngle + (_BottomSideBlend / 2), angle);
            o.Albedo = o.Albedo * (1 - drawStrength) + (_Side.rgb) * drawStrength;

            drawStrength = inverseLerp(_TopAngle - (_SideTopBlend / 2) - epsilon, _TopAngle + (_SideTopBlend / 2), angle);
            o.Albedo = o.Albedo * (1 - drawStrength) + (_Top.rgb) * drawStrength;
        }
        ENDCG
    }
        FallBack "Diffuse"
}
