Shader "Splatoonity/SplatBlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	/*	_WorldPosTex("Texture", 2D) = "white" {}
		_worldTangentTex("Texture", 2D) = "white" {}
		_worldBinormalTex("Texture", 2D) = "white" {}
		_WorldNormalTex("Texture", 2D) = "white" {}*/
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	//注意：可以在顶点着色器中进行的计算就不应该放到片段着色器中去算。
	//appdata是输入，常用的输入结构：appdata_base，appdata_tan，appdata_full。

	struct v2f
	{
		fixed4 pos : SV_POSITION;
		fixed2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	fixed4 _MainTex_TexelSize;

	sampler2D _WorldPosTex;
	sampler2D _worldTangentTex;
	sampler2D _worldBinormalTex;
	sampler2D _WorldNormalTex;

	sampler2D _LastSplatTex;

	int _TotalSplats;
	fixed4x4 _SplatMatrix[10];
	fixed4 _SplatScaleBias[10];
	fixed4 _SplatChannelMask[10];

	fixed2 _SplatTexSize;
	
	v2f vert (appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}
	 
	fixed4 frag (v2f i) : COLOR
	{
		fixed4 currentSplat = tex2D(_LastSplatTex, i.uv);
		fixed4 wpos = tex2D(_WorldPosTex, i.uv);
		
		for( int i = 0; i < _TotalSplats; i++ ){
			fixed3 opos = mul(_SplatMatrix[i], fixed4(wpos.xyz,1)).xyz;

			// skip if outside of projection volume
			if( opos.x > -0.5 && opos.x < 0.5 && opos.y > -0.5 && opos.y < 0.5 && opos.z > -0.5 && opos.z < 0.5 ){
				// generate splat uvs
				fixed2 uv = saturate( opos.xz + 0.5 );  //确保值为[0 ，1）得区域之间


			//	float2 uv = float2(0.9,);
				uv *= _SplatScaleBias[i].xy;
				uv += _SplatScaleBias[i].zw;
				
				// sample the texture
				fixed newSplatTex = tex2D( _MainTex, uv ).x;
				newSplatTex  = saturate( newSplatTex - abs( opos.y ) * abs( opos.y ) );
				//将非边角数据全部改为1,
				/*if(newSplatTex > 0.5)
					newSplatTex = 1;
				else
					newSplatTex = 0;*/
				currentSplat = min( currentSplat, 1.0 - newSplatTex * ( 1.0 - _SplatChannelMask[i] ) );
				currentSplat = max( currentSplat, newSplatTex * _SplatChannelMask[i]);
				//currentSplat = fixed4(1, 0, 0, 0);
			}

		}

		// mask based on world coverage
		// needed for accurate score calculation
	/*	currentSplat = float4(1, 0, 0, 0);
		return currentSplat;*/
		return currentSplat /** wpos.w*/;
	}
	
	float4 fragClear (v2f i) : COLOR
	{
		return float4(0,0,0,0);
	}
	
	fixed4 fragBleed (v2f i) : COLOR
	{
		
		fixed2 uv = i.uv;
		fixed4 worldPos = tex2D(_MainTex, uv);
		
		if( worldPos.w < 0.5 ){
			worldPos += tex2D( _MainTex, uv + float2(-1,-1) * _MainTex_TexelSize.xy );
			worldPos += tex2D( _MainTex, uv + float2(-1,0) * _MainTex_TexelSize.xy );
			worldPos += tex2D( _MainTex, uv + float2(-1,1) * _MainTex_TexelSize.xy );
			
			worldPos += tex2D( _MainTex, uv + float2(0,-1) * _MainTex_TexelSize.xy );
			worldPos += tex2D( _MainTex, uv + float2(0,0) * _MainTex_TexelSize.xy );
			worldPos += tex2D( _MainTex, uv + float2(0,1) * _MainTex_TexelSize.xy );
			
			worldPos += tex2D( _MainTex, uv + float2(1,-1) * _MainTex_TexelSize.xy );
			worldPos += tex2D( _MainTex, uv + float2(1,0) * _MainTex_TexelSize.xy );
			worldPos += tex2D( _MainTex, uv + float2(1,1) * _MainTex_TexelSize.xy );
		
			worldPos.xyz *= 1.0 / max( worldPos.w, 0.00001 );
			worldPos.w = min( worldPos.w, 1.0 );
		}
		
		return worldPos;
	}
	
	
	fixed4 fragCompile (v2f i) : COLOR
	{
		fixed4 splatSDF = tex2D (_MainTex, i.uv);
		fixed4 splatMask = smoothstep( 0.5 - 0.01, 0.5 + 0.01, splatSDF );
		return splatMask;
	}

	ENDCG
	
	SubShader
	{
		ZTest Off
		Cull Off
		ZWrite Off
		Fog { Mode off }

		//Pass 0 decal rendering pass
		Pass
		{
			Name "Splat"
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			ENDCG
		}
		
		//Pass 1 clear splat map pass
		Pass
		{
			Name "Clear"
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragClear
			#pragma target 3.0
			ENDCG
		}
		
		//Pass 2 bleed pass
		Pass
		{
			Name "Bleed"
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragBleed
			#pragma target 3.0
			ENDCG
		}
		
		//Pass 3 compile pass
		Pass
		{
			Name "Compile"
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragCompile
			#pragma target 3.0
			ENDCG
		}
	}
}
