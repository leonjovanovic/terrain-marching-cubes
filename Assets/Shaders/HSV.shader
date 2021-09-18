Shader "Custom/HSV"
{
    Properties
    {
        _Color("Color", Color) = (1,0,0,1)
        _ColorLight("Directional Light", Color) = (1,0.9329268,0.75,1)
        H("Hue", Range(0.0, 6.28)) = 0
        S("Saturation", Range(0.0, 1.0)) = 1.0
        V("Value", Range(0.0, 1.0)) = 1.0
        _max("Max", Float) = 5

    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100
            Cull off
            Tags { "LightMode" = "Always" }

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"
                #include "Lighting.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 worldPos : TEXCOORD0;
                    fixed4 color : COLOR;
                    float3 normal : NORMAL;
                    float3 lightDir : TEXCOORD1;
                    float3 viewDir : TEXCOORD2;
                };

                float4 _Color;
                float4 _ColorLight;
                float _max;

                float H;
                float S;
                float V;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).yy;
                    o.color.xyz = v.normal;
                    o.color.w = 1.0;
                    // Phong lighting
                    o.normal = UnityObjectToWorldNormal(v.normal);
                    float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                    o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    //cos and sin was on (H*PI/180)
                    float VSU = float(V * S * cos( H / _max * -i.worldPos.y));
                    float VSW = float(V * S * sin(H / _max * -i.worldPos.y));


                    float retr = float((0.299 * V + 0.701 * VSU + 0.168 * VSW) * _Color.r + (0.587 * V - 0.587 * VSU + 0.330 * VSW) * _Color.g + (0.114 * V - 0.114 * VSU - 0.497 * VSW) * _Color.b);
                    float retg = float((0.299 * V - 0.299 * VSU - 0.328 * VSW) * _Color.r + (0.587 * V + 0.413 * VSU + 0.035 * VSW) * _Color.g + (0.114 * V - 0.114 * VSU + 0.292 * VSW) * _Color.b);
                    float retb = float((0.299 * V - 0.3 * VSU + 1.25 * VSW) * _Color.r + (0.587 * V - .588 * VSU - 1.05 * VSW) * _Color.g + (0.114 * V + .886 * VSU - 0.203 * VSW) * _Color.b);

                    fixed4 col = fixed4(retr, retg, retb, 1);

                    // Phong lighting
                    float3 normal = normalize(i.normal);
                    float3 lightDir = normalize(i.lightDir);
                    float lambert = max(0, dot(normal, lightDir));
                    float3 reflection = reflect(-lightDir, normal);
                    float specular = pow(max(0, dot(normalize(i.viewDir), reflection)), 25) * 0.075;
                    return fixed4(_ColorLight * col.rgb * lambert + fixed3(specular, specular, specular), 1.0);
                }
                ENDCG
            }
        }
        Fallback "Diffuse"
}
