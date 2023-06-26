Shader "DistanceShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _Radius("Radius", Range(0.001, 500)) = 10
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 worldPos : TEXCOORD0;
            };

            float4 _Color;
            float _Radius;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _WorldSpaceCameraPos);
                fixed4 col = _Color;
                col.a *= clamp(_Radius / dist, 0, 1);
                return col;
            }
            ENDCG
        }
    }
}
