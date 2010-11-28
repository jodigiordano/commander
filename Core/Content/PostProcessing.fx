float			offsetLuminosite = 0.5f;
float			offsetContraste = -1.0f;
sampler2D		image : register(s0);
sampler2D		masque : register(s1);
sampler2D       lumieres : register(s2);
float4			noir = float4(0, 0, 0, 1);
float4          inverse = float4(1, 1, 1, 1);
 
float4 PixerShaderLuminositeContraste(float2 inCoord : TEXCOORD0) : COLOR0
{
	return (tex2D(image, inCoord.xy) + offsetLuminosite) * (1.0 + offsetContraste);
}

float4 PixerShaderMasque(float2 inCoord : TEXCOORD0) : COLOR0
{
	return tex2D(image, inCoord.xy) * tex2D(masque, inCoord.xy);
}

float4 PixerShaderNormal(float2 inCoord : TEXCOORD0) : COLOR0
{
	return tex2D(image, inCoord.xy);
}

float4 PixerShaderLumieres(float2 inCoord : TEXCOORD0) : COLOR0
{
    return tex2D(image, inCoord.xy) + tex2D(lumieres, inCoord.xy);
}

float4 PixerShaderLumieresEtMasque(float2 inCoord : TEXCOORD0) : COLOR0
{
    return (tex2D(image, inCoord.xy) + tex2D(lumieres, inCoord.xy)) * tex2D(masque, inCoord.xy);
}

technique LuminositeContraste { pass Passe1 { PixelShader = compile ps_2_0 PixerShaderLuminositeContraste(); } }
technique Masque { pass Passe1 { PixelShader = compile ps_2_0 PixerShaderMasque(); } }
technique Aucune { pass Passe1 { PixelShader = compile ps_2_0 PixerShaderNormal(); } }
technique Lumieres { pass Passe1 { PixelShader = compile ps_2_0 PixerShaderLumieres(); } }
technique LumieresEtMasque { pass Passe1 { PixelShader = compile ps_2_0 PixerShaderLumieresEtMasque(); } }