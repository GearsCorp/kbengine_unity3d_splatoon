Shader "Splatoonity/WorldPosUnwrap"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed3 worldPos : TEXCOORD3;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				//扩大两倍
				fixed3 uvWorldPos = fixed3( v.texcoord1.xy * 2.0 - 1.0, 0.5 );
				o.pos = mul( UNITY_MATRIX_VP, fixed4( uvWorldPos, 1.0 ) );
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 worldPos = i.worldPos;
				return fixed4(worldPos, 1.0);
			}
			ENDCG
		}
	}
}
