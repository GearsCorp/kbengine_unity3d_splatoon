Shader "Splatoonity/WorldBinormalUnwrap"
{
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
				fixed3 worldPos : TEXCOORD0;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				fixed3 uvWorldPos = fixed3( v.texcoord1.xy * 2.0 - 1.0, 0.5 );
				o.pos = mul( UNITY_MATRIX_VP, fixed4( uvWorldPos, 1.0 ) );
				o.worldPos = mul(unity_ObjectToWorld, v.vertex ).xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 worldBinormal = normalize( ddy( i.worldPos ) ) * 0.5 + 0.5;
				return fixed4(worldBinormal, 1.0);
			}
			ENDCG
		}
	}
}
