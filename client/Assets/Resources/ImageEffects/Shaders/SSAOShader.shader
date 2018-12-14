// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SSAO" {
Properties {
	_MainTex ("", 2D) = "" {}
	_RandomTexture ("", 2D) = "" {}
	_SSAO ("", 2D) = "" {}
}
Subshader {
	ZTest Always Cull Off ZWrite Off

CGINCLUDE
// Common code used by several SSAO passes below
#include "UnityCG.cginc"
struct v2f_ao {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float2 uvr : TEXCOORD1;
};

uniform float2 _NoiseScale;
float4 _CameraDepthNormalsTexture_ST;

v2f_ao vert_ao (appdata_img v)
{
	v2f_ao o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = TRANSFORM_TEX(v.texcoord, _CameraDepthNormalsTexture);
	o.uvr = v.texcoord.xy * _NoiseScale;
	return o;
}

sampler2D _CameraDepthNormalsTexture;
sampler2D _RandomTexture;
float4 _Params; // x=radius, y=minz, z=attenuation power, w=SSAO power



// ---- Blur pass
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#include "UnityCG.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
};

float4 _MainTex_ST;

v2f vert (appdata_img v)
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = TRANSFORM_TEX (v.texcoord, _CameraDepthNormalsTexture);
	return o;
}

sampler2D _SSAO;
float3 _TexelOffsetScale;

inline half CheckSame (half4 n, half4 nn)
{
	// difference in normals
	half2 diff = abs(n.xy - nn.xy);
	half sn = (diff.x + diff.y) < 0.1;
	// difference in depth
	float z = DecodeFloatRG (n.zw);
	float zz = DecodeFloatRG (nn.zw);
	float zdiff = abs(z-zz) * _ProjectionParams.z;
	half sz = zdiff < 0.2;
	return sn * sz;
}


half4 frag( v2f i ) : SV_Target
{
	#define NUM_BLUR_SAMPLES 4
	
    float2 o = _TexelOffsetScale.xy;
    
    half sum = tex2D(_SSAO, i.uv).r * (NUM_BLUR_SAMPLES + 1);
    half denom = NUM_BLUR_SAMPLES + 1;
    
    half4 geom = tex2D (_CameraDepthNormalsTexture, i.uv);
    
    for (int s = 0; s < NUM_BLUR_SAMPLES; ++s)
    {
        float2 nuv = i.uv + o * (s+1);
        half4 ngeom = tex2D (_CameraDepthNormalsTexture, nuv.xy);
        half coef = (NUM_BLUR_SAMPLES - s) * CheckSame (geom, ngeom);
        sum += tex2D (_SSAO, nuv.xy).r * coef;
        denom += coef;
    }
    for (int s = 0; s < NUM_BLUR_SAMPLES; ++s)
    {
        float2 nuv = i.uv - o * (s+1);
        half4 ngeom = tex2D (_CameraDepthNormalsTexture, nuv.xy);
        half coef = (NUM_BLUR_SAMPLES - s) * CheckSame (geom, ngeom);
        sum += tex2D (_SSAO, nuv.xy).r * coef;
        denom += coef;
    }
    return sum / denom;
}
ENDCG
	}
	
	// ---- Composite pass
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv[2] : TEXCOORD0;
};

v2f vert (appdata_img v)
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv[0] = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);
	o.uv[1] = MultiplyUV (UNITY_MATRIX_TEXTURE1, v.texcoord);
	return o;
}

sampler2D _MainTex;
sampler2D _SSAO;

half4 frag( v2f i ) : SV_Target
{
	half4 c = tex2D (_MainTex, i.uv[0]);
	half ao = tex2D (_SSAO, i.uv[1]).r;
	ao = pow (ao, _Params.w);
	c.rgb *= ao;
	return c;
}
ENDCG
	}

}

Fallback off
}
