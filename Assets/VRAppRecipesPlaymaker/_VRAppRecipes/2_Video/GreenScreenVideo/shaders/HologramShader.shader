Shader "ZefirVR/HologramShader"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_thresh ("Threshold", Range (0, 16)) = 1.0
        _slope ("Slope", Range (0, 1)) = 0.341
        _keyingColor ("Key Colour", Color) = (20,255,10,255)
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="False" "RenderType"="Transparent"}
		//Tags { "RenderType"="Opaque" }
		LOD 100

        Lighting Off
        ZWrite Off
        AlphaTest Off
        Blend SrcAlpha OneMinusSrcAlpha 

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float3 _keyingColor;
            float _thresh; // 0.8
            float _slope; // 0.2
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);

			    float3 input_color = tex2D(_MainTex, i.uv).rgb;
	            float d = abs(length(abs(_keyingColor.rgb - input_color.rgb)));
	            float edge0 = _thresh * (1.0 - _slope);
	            float alpha = smoothstep(edge0, _thresh, d);
	     
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return fixed4(input_color, alpha);
				//return col;
			}
			ENDCG
		}
	}
}
