Shader "Custom/SphereShader" 
{
	Properties
	{
		_Color1 ("Color 1",Color) = (1,1,1,1)			// カラー1
		_Color2 ("Color 2",Color) = (1,1,1,1)			// カラー2
		_MainTex ("MainTexture",2D) = "white"{}			// テクスチャ1
		_SubTex ("SubTexture",2D) = "white"{}			// テクスチャ2
		_DesolveTex ("Mask",2D) = "white"{}				// マスクテクスチャ
		_Threshold("Threshold",Range(0,1)) = 0.0		// マスクのしきい値
		_Smoothness("Smoothness",Range(0,1)) = 0.5		// 滑らかさ
		_Metallic("Metallic",Range(0,1)) = 0.0			// 金属感

		_RimColor("RimRight",Color) = (0.5,0.5,0.5,1.0)	// リムライトのカラー
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;		// テクスチャ
		sampler2D _SubTex;		// テクスチャ
		sampler2D _DesolveTex;	// マスクテクスチャ

		struct Input
		{
			float2 uv_MainTex;
			float3 viewDir;
		};

		half _Smoothness;		// 滑らかさ
		half _Metallic;			// 金属感
		half _Threshold;		// マスクのしきい値
		fixed4 _Color1;			// カラー1
		fixed4 _Color2;			// カラー2
		fixed4 _RimColor;		// リムライトのカラー

		// 追加したもの　(何を行っているのかがわからないため要勉強)
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// マスクテクスチャの濃さを取得する
			fixed4 m = tex2D(_DesolveTex, IN.uv_MainTex);
			half g = m.r * 0.2 + m.g * 0.7 + m.b * 0.1;

			// しきい値によって、2つのテクスチャを描画
			if (g < _Threshold) 
			{
				fixed4 c1 = tex2D(_MainTex, IN.uv_MainTex)*_Color1;
				o.Albedo = c1.rgb;
				o.Alpha = c1.a;
				o.Metallic = _Metallic;
				o.Smoothness = _Smoothness;
			}
			else 
			{
				// それぞれのパラメータを適用
				fixed4 c2 = tex2D(_SubTex, IN.uv_MainTex)*_Color2;
				o.Albedo = c2.rgb;
				o.Alpha = c2.a;
				o.Metallic = _Metallic;
				o.Smoothness = _Smoothness;
			}

			float rim = 1 - saturate(dot(IN.viewDir, o.Normal));
			o.Emission = _RimColor * pow(rim, 2.5);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
