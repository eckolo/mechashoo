// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Particles/Window" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	Blend SrcAlpha One
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t input)
			{
				v2f output = {
					float4(0,0,0,0),
					fixed4(0.0,0.0,0.0,0.0),
					float2(0,0)
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					,float4(0,0,0,0)
					#endif
					UNITY_VERTEX_OUTPUT_STEREO
				};
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				output.vertex = UnityObjectToClipPos(input.vertex);
				output.color = input.color;
				output.texcoord = input.texcoord;
				return output;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f input) : SV_Target
			{
				float diff = 0.1;
				fixed4 correct = (tex2D(_MainTex, input.texcoord)
					+ tex2D(_MainTex, input.texcoord + float2(diff,0)) 
					+ tex2D(_MainTex, input.texcoord + float2(-diff,0)) 
					+ tex2D(_MainTex, input.texcoord + float2(0,diff)) 
					+ tex2D(_MainTex, input.texcoord + float2(0,-diff)) ) / 5;
				fixed4 color = 2.0f * input.color * _TintColor * correct;
				return color;
			}
			ENDCG 
			Blend SrcColor SrcColor
			BlendOp Min
		}
	}	
}
}
