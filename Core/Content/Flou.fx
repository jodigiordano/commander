sampler2D		image : register(s0);
float2          tailleImage;
float			pourcFlou;
float4			teinte;

const float4 samples[9] = {
	-100.0,  -100.0,	0.0,	1.0/16.0,
	-100.0,	100.0,	0.0,	1.0/16.0,
	 100.0,  -100.0,	0.0,	1.0/16.0,
	 100.0,   100.0,	0.0,	1.0/16.0,
	-100.0,   0.0,	0.0,	2.0/16.0,
	 100.0,	0.0,	0.0,	2.0/16.0,
	 0.0,  -100.0,	0.0,	2.0/16.0,
	 0.0,   100.0,	0.0,	2.0/16.0,
	 0.0,   0.0,	0.0,	1.0/16.0
};

const float4 samplesGauss[7] = {
   -3.0,    0.0,    0.0,    1.0/64.0,
   -2.0,    0.0,    0.0,    6.0/64.0,
   -1.0,    0.0,    0.0,    15.0/64.0,
    1.0,    0.0,    0.0,    15.0/64.0,
    2.0,    0.0,    0.0,    6.0/64.0,
    3.0,    0.0,    0.0,    1.0/64.0,
    0.0,    0.0,    0.0,    20.0/64.0
};

float4 PixelShaderFlouGauss(float2 inCoord : TEXCOORD0) : COLOR0
{
    float4 col = float4(0,0,0,0);
    
    for (int i = 0; i < 6; i++)
        col += pourcFlou * samplesGauss[i].w * tex2D(image, inCoord + samplesGauss[i].xy / tailleImage);
    
    col += clamp((1.0 - pourcFlou), samplesGauss[6].w, 1.0) * tex2D(image, inCoord + samplesGauss[6].xy / tailleImage);    
    
    return col * teinte;
}

float4 PixelShaderFlou(float2 inCoord : TEXCOORD0) : COLOR0
{
    float4 col = float4(0,0,0,0);
    
    for (int i = 0; i < 8; i++)
		col += pourcFlou * samples[i].w * tex2D(image, inCoord + samples[i].xy / tailleImage);
    
    col += clamp((1.0 - pourcFlou), samples[8].w, 1.0) * tex2D(image, inCoord + samples[8].xy / tailleImage);
  
    return col * teinte;
    //return tex2D(image, inCoord.xy);
}

technique Flou {
	pass Passe1 { PixelShader = compile ps_2_0 PixelShaderFlou(); }
}

technique FlouGauss {
	pass Passe1 { PixelShader = compile ps_2_0 PixelShaderFlouGauss(); }
}