Shader "IDEA-Project-4/Sdt Surface Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SecondaryTexture ("Secondary Texture", 2D) = "white" {}
        _Normal ("Normal",  2D) = "bump" {}
        _Normal2 ("Normal 2",  2D) = "bump" {}
        _BlendRate ("Blend Rate", Float) = 0.5
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _Fresnel ("Fresnel", Range(0,5)) = 0
        _FresnelSpeed("Fresnel Speed", Float) = 10
    }
    SubShader
    {
        // Pass {
            Tags { "RenderType"="Opaque" }
            LOD 200

        

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows vertex:vert

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _SecondaryTexture;
            sampler2D _Normal;
            sampler2D _Normal2;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_SecondaryTexture;
                float3 worldNormal;
                float2 uv_Normal;
                float2 uv_Normal2;
                float3 worldPos;
                float3 customColor;
                INTERNAL_DATA
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;
            half _BlendRate;
            half _Fresnel;
            half _FresnelSpeed;
            fixed4 _FresnelColor;
        

           


            void vert (inout appdata_full v, out Input o) {
                UNITY_INITIALIZE_OUTPUT(Input,o);
                o.worldNormal = v.normal;
                o.worldPos = v.vertex.xyz;

                o.customColor = v.normal;

                float sinF = sin(_Time * 200);
                v.vertex.xyz += v.normal * ((sinF + 1) *0.1);

            }

            void surf (Input IN, inout SurfaceOutputStandard o)
            {

                float sinFunc = sin(_Time * _BlendRate);
                
                clip (frac((IN.worldPos.y+IN.worldPos.z*0.1) * 10 * sinFunc) - sinFunc);            

                // Albedo comes from a texture tinted by color
                
                fixed4 c = lerp(tex2D (_MainTex, IN.uv_MainTex), tex2D(_SecondaryTexture, IN.uv_SecondaryTexture), sinFunc) * _Color;
                o.Albedo = c.rgb * IN.customColor;

                float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
                float fresnelPower = pow(abs(dot(viewDir, IN.customColor)), _Fresnel);
                o.Emission = (1 - saturate(fresnelPower)) * _FresnelColor;

                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;

                
                // o.Albedo *= 1 - pow(abs(dot(viewDir, IN.worldNormal)), _Fresnel);

                // o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Normal));
                o.Normal = lerp(UnpackNormal(tex2D (_Normal, IN.uv_Normal)), UnpackNormal(tex2D(_Normal2, IN.uv_Normal2)), (sinFunc + 1) * 0.5);

                float worldNor = WorldNormalVector (IN, o.Normal);

                // float sinFunc2 = sin(_Time * _FresnelSpeed);
                // float fresnelPower2 = pow(abs(dot(viewDir, worldNor)), _Fresnel * (sinFunc2+1) * 0.5);
                // o.Emission += (1 - saturate(fresnelPower2)) * _FresnelColor;
                // o.Emission = fresnelPower;
            

            }
            ENDCG

        // }
        // Pass {
        //     CGPROGRAM
        //     // Physically based Standard lighting model, and enable shadows on all light types
        //     #pragma surface surf Standard fullforwardshadows vertex:vert

        //     // Use shader model 3.0 target, to get nicer looking lighting
        //     #pragma target 3.0

        
        //     struct Input
        //     {
        //         float3 worldNormal;
        //     };
        //     half _Fresnel;
        //     half _FresnelSpeed;
        //     fixed4 _FresnelColor;
        

       
        //     void vert (inout appdata_full v, out Input o) {
        //         UNITY_INITIALIZE_OUTPUT(Input,o);
        //         o.worldNormal = v.normal;
        //     }

        //     void surf (Input IN, inout SurfaceOutputStandard o)
        //     {

        //         float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;

        //         float fresnelPower = pow(abs(dot(viewDir, IN.worldNormal)), _Fresnel);
        //         o.Emission = (1 - saturate(fresnelPower)) * _FresnelColor;
            

        //     }
        //     ENDCG  
        // }
    }
    FallBack "Diffuse"
}
