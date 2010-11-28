sampler2D		image : register(s0);
float			pourcDarker;
float4			teinte;

float4 PixelShaderDarker(float2 inCoord : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(image, inCoord.xy) * teinte;
    
    color.rgb *= 1 - pourcDarker;
    
    return color;
}

technique Darker {
	pass Passe1 { PixelShader = compile ps_2_0 PixelShaderDarker(); }
}
