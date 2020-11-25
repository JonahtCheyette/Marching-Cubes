Shader "Unlit/DotShader" {
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float values[1023];
            float4 size;
            float gridSize;
            float max;
            float min;
            float4 center;

            v2f vert (appdata v) {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target{
                int x = round(((i.worldPos.x - center.x) / gridSize) + ((size.x - 1) / 2));
                int y = round(((i.worldPos.y - center.y) / gridSize) + ((size.y - 1) / 2));
                int z = round(((i.worldPos.z - center.z) / gridSize) + ((size.z - 1) / 2));
                float dotValue = (values[x + (y * size.x) + (z * size.x * size.y)] - min)/(max - min);
                return fixed4(dotValue, dotValue, dotValue,1);
            }
            ENDCG
        }
    }
}
