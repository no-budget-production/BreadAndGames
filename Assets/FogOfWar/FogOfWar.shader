// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FogOfWar" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Everything Hidden", 2D) = "white" {}
		_SecondTex("Where it is Visible", 2D) = "white" {}
		_FogRadius("FogRadius", Float) = 1.0
		_FogMaxRadius("FogMaxRadius", Float) = 0.5
		_Player1_Pos ("_Player1_Pos", Vector) = (0,0,0,1)
		_Player2_Pos ("_Player2_Pos", Vector) = (0,0,0,1)
		_Player3_Pos ("_Player3_Pos", Vector) = (0,0,0,1)
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert vertex:vert alpha:blend

		sampler2D _MainTex;
		sampler2D _SecondTex;
		fixed4 _Color;
		float _FogRadius;
		float _FogMaxRadius;
		float4 _Player1_Pos;
		float4 _Player2_Pos;
		float4 _Player3_Pos;

		struct Input {
			float2 uv_MainTex;
			float2 location;
		};


		float powerForPos(float4 pos, float2 nearVertex);

		void vert(inout appdata_full vertexData, out Input outData) {
			float4 pos = UnityObjectToClipPos(vertexData.vertex);
			float4 posWorld = mul(unity_ObjectToWorld, vertexData.vertex);
			outData.uv_MainTex = vertexData.texcoord;
			outData.location = posWorld.xz;

		}

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 baseColor = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			float alpha = (1.0 - powerForPos(_Player1_Pos, IN.location));

			o.Albedo = baseColor.rgb;
			o.Alpha = alpha;
		}

		float powerForPos(float4 pos, float2 nearVertex) {
			float atten = (_FogRadius - length(pos.xz - nearVertex.xy));
			return atten / _FogRadius;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
