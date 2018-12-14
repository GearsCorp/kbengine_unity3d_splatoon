Shader "Splatoonity/SplatShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpTex ("Normal", 2D) = "bump" {}
		_SplatTileNormalTex ("Splat Tile Normal", 2D) = "bump" {}
		_SplatTileBump ("Splat Tile Bump", Range(1,10)) = 1.0
		_SplatEdgeBump ("Splat Edge Bump", Range(1,10)) = 1.0
		_SplatEdgeBumpWidth ("Splat Edge Bump Width", Range(1,10)) = 1.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}	
	SubShader {
		//Tags标签：“Queue:定义为一个整数，数值越小越容易被渲染；”；
	    //          “RenderType：“Opaque”或”Transparent”是两个常用的RenderType。如果输出中都是非透明物体，那写在Opaque里；”
		//          “如果想渲染透明或者半透明的像素，那应该写在Transparent中."
		Tags { "RenderType"="Opaque" }

		//LOD：每个subShader都可以有一个Lod,指定不同的lod,lod越大则渲染效果越好， 当然对硬件的要求也越高。
		//然后根据不同的终端硬件配置来设置 globalMaximumLOD来达到兼顾性能的最佳显示效果。
		//Diffuse = 200
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		//这些是属于属性的声明
		sampler2D _MainTex;
		sampler2D _BumpTex;
		sampler2D _SplatTileNormalTex;
		fixed _SplatEdgeBump;
		fixed _SplatEdgeBumpWidth;
		fixed _SplatTileBump;

		sampler2D _SplatTex;
		fixed4 _SplatTex_TexelSize;
		sampler2D _WorldTangentTex;
		sampler2D _WorldBinormalTex;

		//表现着色器Input的参数数值
		struct Input {
			fixed2 uv_MainTex;
			fixed2 uv2_SplatTex;
			fixed3 worldNormal;   //世界空间中的法线向量,表面着色器(surface shader)不写入法线(o.Normal)参数，将包含这个参数。
			fixed3 worldTangent;  //世界空间中的切线
			fixed3 worldBinormal; //世界空间中的次法线向量
			fixed3 worldPos;      //世界空间中的位置
			INTERNAL_DATA
		};
		//引入的outPut::SurfaceOutput、SurfaceOutputStandard、SurfaceOutputStandardSpecular

		fixed _Glossiness;
		fixed _Metallic;
		fixed4 _Color;
		
		static const float _Clip = 0.5;

		fixed3x3 cotangent_frame(fixed3 N, fixed3 p, fixed2 uv )
		{
		    // get edge vectors of the pixel triangle
			fixed3 dp1 = ddx( p );
			fixed3 dp2 = ddy( p );
			fixed2 duv1 = ddx( uv );
			fixed2 duv2 = ddy( uv );
		 
		    // solve the linear system
			fixed3 dp2perp = cross( dp2, N );
			fixed3 dp1perp = cross( N, dp1 );
			fixed3 T = dp2perp * duv1.x + dp1perp * duv2.x;
			fixed3 B = dp2perp * duv1.y + dp1perp * duv2.y;
		 
		    // construct a scale-invariant frame 
			fixed invmax = rsqrt( max( dot(T,T), dot(B,B) ) );
			fixed3 TinvMax = normalize(T * invmax);
			fixed3 BinvMax =  normalize(B * invmax);
		    return fixed3x3(fixed3( TinvMax.x, BinvMax.x, N.x ), fixed3( TinvMax.y, BinvMax.y, N.y ), fixed3( TinvMax.z, BinvMax.z, N.z ));
		    //return float3x3( TinvMax, BinvMax, N );
		}

		fixed3 perturb_normal(fixed3 localNormal, fixed3 N, fixed3 V, fixed2 uv )
		{
		    // assume N, the interpolated vertex normal and 
		    // V, the view vector (vertex to eye)
			fixed3x3 TBN = cotangent_frame( N, -V, uv );
			return normalize( mul( TBN, localNormal ) );
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			// Sample splat map texture with offsets
			fixed4 splatSDF = tex2D (_SplatTex, IN.uv2_SplatTex);
			fixed4 splatSDFx = tex2D (_SplatTex, IN.uv2_SplatTex + fixed2(_SplatTex_TexelSize.x,0) );
			fixed4 splatSDFy = tex2D (_SplatTex, IN.uv2_SplatTex + fixed2(0,_SplatTex_TexelSize.y) );

			// Use ddx ddy to figure out a max clip amount to keep edge aliasing at bay when viewing from extreme angles or distances
			fixed splatDDX = length( ddx(IN.uv2_SplatTex * _SplatTex_TexelSize.zw) );
			fixed splatDDY = length( ddy(IN.uv2_SplatTex * _SplatTex_TexelSize.zw) );
			fixed clipDist = sqrt( splatDDX * splatDDX + splatDDY * splatDDY );
			fixed clipDistHard = max( clipDist * 0.01, 0.01 );
			fixed clipDistSoft = 0.01 * _SplatEdgeBumpWidth;

			// Smoothstep to make a soft mask for the splats
			fixed4 splatMask = smoothstep( ( _Clip - 0.01 ) - clipDistHard, ( _Clip - 0.01 ) + clipDistHard, splatSDF );
			fixed splatMaskTotal = max( max( splatMask.x, splatMask.y ), max( splatMask.z, splatMask.w ) );

			// Smoothstep to make the edge bump mask for the splats
			fixed4 splatMaskInside = smoothstep( _Clip - clipDistSoft, _Clip + clipDistSoft, splatSDF );
			splatMaskInside = max( max( splatMaskInside.x, splatMaskInside.y ), max( splatMaskInside.z, splatMaskInside.w ) );

			// Create normal offset for each splat channel
			fixed4 offsetSplatX = splatSDF - splatSDFx;
			fixed4 offsetSplatY = splatSDF - splatSDFy;

			// Combine all normal offsets into single offset
			fixed2 offsetSplat = lerp(fixed2(offsetSplatX.x,offsetSplatY.x), fixed2(offsetSplatX.y,offsetSplatY.y), splatMask.y );
			offsetSplat = lerp( offsetSplat, fixed2(offsetSplatX.z,offsetSplatY.z), splatMask.z );
			offsetSplat = lerp( offsetSplat, fixed2(offsetSplatX.w,offsetSplatY.w), splatMask.w );
			offsetSplat = normalize(fixed3( offsetSplat, 0.0001) ).xy; // Normalize to ensure parity between texture sizes
			offsetSplat = offsetSplat * ( 1.0 - splatMaskInside ) * _SplatEdgeBump;

			// Add some extra bump over the splat areas
			fixed2 splatTileNormalTex = tex2D( _SplatTileNormalTex, IN.uv2_SplatTex * 10.0 ).xy;
			offsetSplat += ( splatTileNormalTex.xy - 0.5 ) * _SplatTileBump  * 0.2;

			// Create the world normal of the splats
			#if 0
				// Use tangentless technique to get world normals
				fixed3 worldNormal = WorldNormalVector (IN, fixed3(0,0,1) );
				fixed3 offsetSplatLocal2 = normalize(fixed3( offsetSplat, sqrt( 1.0 - saturate( dot( offsetSplat, offsetSplat ) ) ) ) );
				fixed3 offsetSplatWorld = perturb_normal( offsetSplatLocal2, worldNormal, normalize( IN.worldPos - _WorldSpaceCameraPos ), IN.uv2_SplatTex );
			#else
				// Sample the world tangent and binormal textures for texcoord1 (the second uv channel)
				// you could skip the binormal texture and cross the vertex normal with the tangent texture to get the bitangent
				fixed3 worldTangentTex = tex2D ( _WorldTangentTex, IN.uv2_SplatTex ).xyz * 2.0 - 1.0;
				fixed3 worldBinormalTex = tex2D ( _WorldBinormalTex, IN.uv2_SplatTex ).xyz * 2.0 - 1.0;

				// Create the world normal of the splats
				fixed3 offsetSplatWorld = offsetSplat.x * worldTangentTex + offsetSplat.y * worldBinormalTex;
			#endif

			// Get the tangent and binormal for the texcoord0 (this is just the actual tangent and binormal that comes in from the vertex shader)
				fixed3 worldTangent = WorldNormalVector (IN, fixed3(1,0,0) );
				fixed3 worldBinormal = WorldNormalVector (IN, fixed3(0,1,0) );

			// Convert the splat world normal to tangent normal for texcood0
			fixed2 offsetSplatLocal = 0;
			offsetSplatLocal.x = dot( worldTangent, offsetSplatWorld );
			offsetSplatLocal.y = dot( worldBinormal, offsetSplatWorld );

			// sample the normal map from the main material
			fixed4 normalMap = tex2D( _BumpTex, IN.uv_MainTex );
			normalMap.xyz = UnpackNormal( normalMap );
			fixed3 tanNormal = normalMap.xyz;

			// Add the splat normal to the tangent normal
			tanNormal.xy += offsetSplatLocal * splatMaskTotal;
			tanNormal = normalize( tanNormal );

			// Albedo comes from a texture tinted by color
			fixed4 MainTex = tex2D (_MainTex, IN.uv_MainTex );
			fixed4 c = MainTex * _Color;
			// Lerp the color with the splat colors based on the splat mask channels
			c.xyz = lerp( c.xyz, fixed3(1.0,0,0.0), splatMask.x );
			c.xyz = lerp( c.xyz, fixed3(0,1.0,0.0), splatMask.y );
			c.xyz = lerp( c.xyz, fixed3(0.0,0.0,1.0), splatMask.z );
			c.xyz = lerp( c.xyz, fixed3(0.0,0.0,0.0), splatMask.w );

			o.Albedo = c.rgb;
			o.Normal = tanNormal;
			o.Metallic = _Metallic;
			o.Smoothness = lerp( _Glossiness, 0.7, splatMaskTotal );
			o.Alpha = c.a;

		}
		ENDCG
	} 
	FallBack "Diffuse"
    //FallBack内置的shader，会影响其阴影的实现
	//fixed atten表示光衰减的系数
}
