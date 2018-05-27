Shader "Custom/VertexDisplacement"
{
	Properties
	{
		_CurveLength ("Curve Length", Float) = 5
		_Thickness ("Thickness (Radius)", Float) = 0.1
		_LengthSegments ("Length Segments", Int) = 100
		_MeshIndex ("Mesh Index", Int) = 1
		_TotalMeshes ("Total Meshes", Int) = 1
		_TimeScale ("Time Scale", Range(1.0, 25.0)) = 5
		_RadiusScale ("Radius Scale", Range(1.0, 5.0)) = 1

	}

	SubShader
	{

		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
			"LightMode"="ForwardBase"
		}

		Pass 
		{
			CGPROGRAM

			#pragma vertex vert             
			#pragma fragment frag
			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // for _LightColor0
            #include "UnityShaderVariables.cginc"

			#define PI 3.1415926535897932384626433832795

			half4 _MyColor;
			float _CurveLength;
			float _LengthSegments;
			float _Thickness;
			float _Morph;
			float _MeshIndex;
			float _TotalMeshes;
			float _TimeScale;
			float _RadiusScale;

			struct vertInput 
			{
				float4 pos : POSITION; // : after the colon are the setting up by Unity
				float2 texcoord : TEXCOORD0;
				float4 normal : NORMAL;
			};  

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				half2 uv  : TEXCOORD0;
				fixed4 diff : COLOR0;
			};

			// sin function
			// float4 sample(fixed t)
			// {
			// 	fixed aa = lerp(0, 2 * PI, t);
			// 	fixed extr = sin(aa);
			// 	return float4(extr, t * _CurveLength, 0.0, 0.0);
			// }

			// float4 sample(fixed t)
			// {
			// 	float angle = t * 2.0 * PI;
			// 	return float4(cos(angle), sin(angle), 0.0, 0.0);
			// }

			// float4 sample (float t) 
			// {
			// 	t += _Time;
			// 	float angle = t * 2.0 * PI;
			// 	float z = t * 2.0 - 1.0;
			// 	return float4(cos(angle), sin(angle), z, 0.0);
			// }

			float4 spherical (float r, float phi, float theta) 
			{
				return float4(
					r * cos(phi) * cos(theta),
					r * cos(phi) * sin(theta),
					r * sin(phi),
					0.0
				);
			}

			// float4 sample (float t) 
			// {
			// 	float angle = t * 2.0 * PI;

			// 	float radius = 1.0;
			// 	float phi = t * 2.0 * PI;
			// 	float theta = (t * 2.0 - 1.0) * _Morph;

			// 	return spherical(radius, (_Time * 25) + phi, theta);
			// }

			float expoInOut (float k) 
			{
				if (k == 0) 
					return 0;
				if (k == 1) 
					return 1;

				if ((k *= 2.0) < 1.0) 
					return 0.5*pow(1024.0, k - 1.0);

				return 0.5*(-pow(2.0, -10.0*(k - 1.0)) + 2.0);
			}

			float4 sample (float t) 
			{
				float time = _Time * _TimeScale;
				float index = _MeshIndex / (_TotalMeshes - 1);

				float beta = t * PI;

				float ripple = expoInOut(sin(t * 2.0 * PI + time)) * 0.25;
  				float noise = time + index * ripple * 12.0;

				float r = sin(index * 0.75 + beta * 2.0) * 0.75 * _RadiusScale;
				float theta = 4.0 * beta + index * 0.25;
				float phi = sin(index * 2.0 + beta * 8.0 + noise);

				return spherical(r, phi, theta);
			}

			void createTube (fixed t, fixed tubeAngle, float2 volume, out fixed4 pos, out fixed4 normal) 
			{
				// find next sample along curve
				fixed nextT = t + (1.0 / _LengthSegments);

				// sample the curve in two places
				float3 cur = sample(t);
				float3 next = sample(nextT);

				// compute the Frenet-Serret frame
				float3 T = normalize(next - cur);
				float3 B = normalize(cross(T, next + cur));
				float3 N = -normalize(cross(B, T));

				// extrude outward to create a tube
				fixed circX = cos(tubeAngle);
				fixed circY = sin(tubeAngle);

				// compute position and normal
				normal.xyz = normalize(B * circX + N * circY);
				pos.xyz = cur + B * volume.x * circX + N * volume.y * circY;
				pos.w = 0.0;
				normal.w = 0.0;
			}

			v2f vert(appdata_base v) 
			{
				fixed t = v.vertex.y / _CurveLength;
				fixed angle = atan2(v.vertex.x, v.vertex.z);
				float2 volume = float2(_Thickness, _Thickness);

				// v.vertex = float4(v.vertex.x, t * _CurveLength, 0.0, 0.0);

				// build our geometry
				fixed4 transformed;
				fixed4 objectNormal;
				createTube(t, angle, volume, transformed, objectNormal);
				v.vertex = transformed;

				v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                // get vertex normal in world space
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);

                // dot product between normal and light direction for
                // standard diffuse (Lambert) lighting
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));

                // factor in the light color
                o.diff = nl * _LightColor0;
                return o;
			}

			half4 frag(v2f output) : COLOR 
			{
				return output.diff;
			}

			ENDCG
		}

	}
}
