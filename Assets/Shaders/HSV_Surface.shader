Shader "Custom/HSV_Surface"
{
    Properties
    {
        _Color("Color", Color) = (1,0,0,1)
        H("Hue", Range(0.0, 6.28)) = 0
        S("Saturation", Range(0.0, 1.0)) = 1.0
        V("Value", Range(0.0, 1.0)) = 1.0
        _min("Min", Float) = 0
        _max("Max", Float) = 5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Cull off
        Tags { "LightMode" = "Always" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input {
            float3 customColor;
            float3 worldPos;
        };
        void vert(inout appdata_base v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.customColor = abs(v.normal.y);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        }

        fixed4 _Color;
        float _min;
        float _max;

        float H;
        float S;
        float V;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input i, inout SurfaceOutput o)
        {
            float VSU = float(V * S * cos(H / _max * -i.worldPos.y));
            float VSW = float(V * S * sin(H / _max * -i.worldPos.y));


            o.Albedo.r = float((0.299 * V + 0.701 * VSU + 0.168 * VSW) * _Color.r + (0.587 * V - 0.587 * VSU + 0.330 * VSW) * _Color.g + (0.114 * V - 0.114 * VSU - 0.497 * VSW) * _Color.b);
            o.Albedo.g = float((0.299 * V - 0.299 * VSU - 0.328 * VSW) * _Color.r + (0.587 * V + 0.413 * VSU + 0.035 * VSW) * _Color.g + (0.114 * V - 0.114 * VSU + 0.292 * VSW) * _Color.b);
            o.Albedo.b = float((0.299 * V - 0.3 * VSU + 1.25 * VSW) * _Color.r + (0.587 * V - .588 * VSU - 1.05 * VSW) * _Color.g + (0.114 * V + .886 * VSU - 0.203 * VSW) * _Color.b);
            //o.Albedo *= saturate(i.customColor);
            // Albedo comes from a texture tinted by color
            // Metallic and smoothness come from slider variables
        }
        ENDCG
    }
    FallBack "Diffuse"
}
