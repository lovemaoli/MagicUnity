Shader "Unlit/Melt"
{
	Properties{
		_MainTex("Base(rgb)", 2D) = "white"{}
		_NoiseMap("NoiseMap", 2D) = "white"{}
		_StartColor("StarColor", Color) = (0,0,0,0)
		_EndColor("EndColor", Color) = (0,0,0,0)
		_MeltThreshold("MeltThreshold", Range(0, 1)) = 0
		_Erode("Erode", Range(0.0, 1.0)) = 0.98
		_ErodeThreshold("ErodeThreshold", Range(0.0, 1.0)) = 0.71
	}
 
 
	SubShader{
 
		CGINCLUDE
 
			#include "Lighting.cginc"
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
 
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NoiseMap;
			//消融边缘起始颜色
			fixed4 _StartColor;
			//最终颜色
			fixed4 _EndColor;
			//消融阈值
			float _MeltThreshold;
			//控制侵蚀程度
			float _Erode;
            //控制侵蚀颜色阈值
			float _ErodeThreshold;
 
			struct a2v{
				float4 vertex : POSITION;
    			float3 normal : NORMAL;
    			float4 texcoord : TEXCOORD0;
			};
 
			struct v2f{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
				SHADOW_COORDS(3)
			};
 
			v2f vert(a2v v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				TRANSFER_SHADOW(o);
				return o;
			}
 
			fixed4 frag(v2f i) : SV_Target{
 
				//使用噪声图采样
				fixed3 melt = tex2D(_NoiseMap, i.uv).rgb;
 
				//采样阈值与设定阈值比较，小于设定的阈值就裁剪掉该片元
				clip(melt.r - _MeltThreshold);
 
				//光照计算部分，使用兰伯特漫反射光照模型
 
				//纹理采样得到反射率
				fixed3 albedo = tex2D(_MainTex, i.uv).rgb;
				//世界法线
				fixed3 worldNormal = normalize(i.worldNormal);
				//入射光
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				//计算环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				//漫反射
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, -worldLightDir));
				//最终光照
				fixed3 lightColor = diffuse * atten + ambient;
				//侵蚀计算部分
				float result = _MeltThreshold / melt.r;
 
				if(result > _Erode){
					//如果结果大于消融颜色的阈值，则返回消融结束部分的颜色，否则返回初始颜色
					if(result > _ErodeThreshold) {
						return _EndColor;
					}
					
					return _StartColor;
 
				}
				//直接返回光照后颜色
				return fixed4(lightColor, 1);
			}
 
		ENDCG
 
 
		Pass{
 
			Tags{ "RenderType" = "Opaque"}
			Tags{"LigthMode" = "ForwardBase"}
			Cull off
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
 
			ENDCG
		}
	}
 
	FallBack Off
}