Shader "Custom/ShaderTransition" 
{
	Properties
	{
		_Color ("Color",Color) = (1,1,1,1)			// カラー
		_MainTex ("MainTexture",2D) = "white"{}		// テクスチャ
		_DesolveTex ("Mask",2D) = "white"{}			// マスク
		_Threshold ("Threshold",Range(0,1)) = 0.0	// マスクのしきい値
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent"
			   "Queue"		= "Transparent"	}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;		// テクスチャ
		sampler2D _DesolveTex;	// マスクテクスチャ

		struct Input
		{
			float2 uv_MainTex;
		};

		half _Threshold;		// マスクのしきい値
		fixed4 _Color;			// カラー

		// 追加したもの　(何を行っているのかがわからないため要勉強)
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// マスクテクスチャの濃さを取得する
			fixed4 m = tex2D(_DesolveTex, IN.uv_MainTex) * _Color;
			half g = m.r * 0.2 + m.g * 0.7 + m.b * 0.1;
			// 濃さがしきい値を下回る場合、描画を中断する
			if (g < _Threshold) 
			{
				// 描画を中断する
				discard;
			}
			else 
			{
				// それぞれのパラメータを適用
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex)*_Color;
				o.Emission = c.rgb;
				o.Alpha = c.a;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
