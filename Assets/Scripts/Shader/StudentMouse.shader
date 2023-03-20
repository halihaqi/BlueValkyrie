// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/StudentMouse"
{
	Properties
	{
        _OriEyeMouth("OriEyeMouth", 2D) = "white" {}
        _EyeMouthMask("EyeMouthMask", 2D) = "white" {}
		_EyeMouthSample("EyeMouthSample", 2D) = "white" {}
		_Threshold("Threshold", Range(0,10)) = 2
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Back
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"
			

            struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 uv : TEXCOORD0;
			};

            struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD1;
            	float dis : TEXCOORD2;
			};

            sampler2D _OriEyeMouth;
            float4 _OriEyeMouth_ST;
			sampler2D _EyeMouthMask;
            float4 _EyeMouthMask_ST;
			sampler2D _EyeMouthSample;
            float4 _EyeMouthSample_ST;
			float _Threshold;

            v2f vert (appdata v)
			{
				v2f o;
            	o.dis = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
				o.uv.xy = v.uv.xy;
				o.uv.zw = 0;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

            fixed4 frag (v2f i) : SV_Target
			{
				float2 ori = i.uv.xy * _OriEyeMouth_ST.xy + _OriEyeMouth_ST.zw;
				float2 sample = i.uv.xy * _EyeMouthSample_ST.xy + _EyeMouthSample_ST.zw;
				float2 mask = i.uv.xy * _EyeMouthMask_ST.xy + _EyeMouthMask_ST.zw;
				float4 result = lerp( tex2D(_OriEyeMouth, ori) , tex2D(_EyeMouthSample, sample) , tex2D(_EyeMouthMask, mask).r);
				float fade = smoothstep(_Threshold - 1, _Threshold, i.dis);
				result.a *= fade;
				return result;
			}
			ENDCG
		}
	}
}