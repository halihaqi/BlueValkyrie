Shader "Custom/DistanceFade" 
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" { }
        _Threshold("Threshold", Range(0,10)) = 5
    }


    SubShader
    {
       Tags { "Queue" = "Transparent" }

       Pass 
        {
            Name "DistanceFade"
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

           struct appdata
           {
               float4 vertex : POSITION;
               float2 uv : TEXCOORD0;
           };

           struct v2f
           {
               float4 vertex : SV_POSITION;
               float2 uv : TEXCOORD0;
               float dis : TEXCOORD1;
           };

           sampler2D _MainTex;
           float _Threshold;

           v2f vert(appdata v)
           {
               v2f o;
               o.vertex = UnityObjectToClipPos(v.vertex);
               o.uv = v.uv;
               o.dis = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
               return o;
           }

           fixed4 frag(v2f i) : SV_Target
           {
               float fade = smoothstep(_Threshold - 1, _Threshold, i.dis);
               fixed4 col = tex2D(_MainTex, i.uv);
               col.a *= fade;
               return col;
           }
           ENDCG
       }
    }
        FallBack "Diffuse"

}