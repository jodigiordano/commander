uniform extern float4x4 World;
uniform extern float4x4 View;
uniform extern float4x4 Projection;
uniform extern float4x4 WorldTransform;

uniform extern texture SpriteTexture;

sampler TextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = POINT;
   
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct vsio
{
	float4 Position : POSITION0;
	float  Size		: PSIZE0;
	float4 Colour	: COLOR0;
	float  Rotation : COLOR1;
};

struct psin
{
#ifdef XBOX
	float2 TexCoord : SPRITETEXCOORD;
#else
	float2 TexCoord : TEXCOORD0;
#endif
	float4 Colour : COLOR0;
	float Rotation : COLOR1;
};

vsio VertexShader(vsio i)
{
	float4x4 worldTransform = mul(World, WorldTransform);
	float4x4 worldView = mul(worldTransform, View);
	float4x4 worldViewProjection = mul(worldView, Projection);
	
	vsio o = (vsio)0;
	
	o.Position = mul(i.Position, worldViewProjection);
	o.Size = i.Size;
	o.Colour = i.Colour;
	o.Rotation = (i.Rotation + 3.14159) / 6.283185;
	
	return o;
};

float4 PixelShader_1_1(psin i) : COLOR0
{
	return tex2D(TextureSampler, i.TexCoord) * i.Colour;
};

float4 PixelShader_2_0(psin i) : COLOR0
{
	float r = (i.Rotation * 6.283185) - 3.141593;
	
	float c = cos(r);
	float s = sin(r);
	
	float2x2 rotationMatrix = float2x2(c, -s, s, c);
	
	float2 texCoord = mul(i.TexCoord - 0.5, rotationMatrix);
	
	return tex2D(TextureSampler, texCoord + 0.5) * i.Colour;
};

technique PointSprites_1_1
{
	pass P0
	{
		vertexShader = compile vs_1_1 VertexShader();
		pixelShader = compile ps_1_1 PixelShader_1_1();
	}
}

technique PointSprites_2_0
{
	pass p0
	{
		vertexShader = compile vs_1_1 VertexShader();
		pixelShader = compile ps_2_0 PixelShader_2_0();
	}
}